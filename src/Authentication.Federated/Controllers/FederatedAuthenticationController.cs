using System;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using Authentication.Federated.Services;
using Microsoft.IdentityModel.Claims;
using Microsoft.IdentityModel.Protocols.WSFederation;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Saml11;
using Microsoft.IdentityModel.Tokens.Saml2;
using Orchard.Data;
using Orchard.Roles.Models;
using Orchard.Roles.Services;
using Orchard.Security;

namespace Authentication.Federated.Controllers
{
    public class FederatedAuthenticationController
        : Controller
    {
        protected IFederationSettingsService SettingsService { get; private set; }
        protected IAuthenticationService AuthenticationService { get; private set; }
        protected IMembershipService MembershipService { get; private set; }
        protected IRoleService RoleService { get; private set; }
        protected IRepository<UserRolesPartRecord> UserRolesRepository { get; private set; }

        public FederatedAuthenticationController(
            IFederationSettingsService settingsService, IAuthenticationService authenticationService, IMembershipService membershipService, IRoleService roleService, IRepository<UserRolesPartRecord> userRolesRepository)
        {
            this.SettingsService = settingsService;
            this.AuthenticationService = authenticationService;
            this.MembershipService = membershipService;
            this.RoleService = roleService;
            this.UserRolesRepository = userRolesRepository;
        }


        public ActionResult TestLogOn(string ReturnUrl)
        {
            return LogOn(ReturnUrl);
        }

        public ActionResult LogOn(string ReturnUrl)
        {
            // Redirect to sign-in URL, append ReturnUrl as well...
            var settings = this.SettingsService.Retrieve();

            if (string.IsNullOrEmpty(ReturnUrl))
            {
                ReturnUrl = "/";
            }
            ReturnUrl = "/FederatedAuthentication/ProcessToken?ReturnUrl=" + ReturnUrl;

            SignInRequestMessage req = new SignInRequestMessage(
                new Uri(settings.StsLoginUrl), settings.Realm, settings.ReturnUrlBase + ReturnUrl);

            return new RedirectResult(req.RequestUrl);
        }

        [ValidateInput(false)]
        public ActionResult Authenticate(string ReturnUrl)
        {
            if (Request.Form.Get(WSFederationConstants.Parameters.Result) != null)
            {
                var settings = this.SettingsService.Retrieve();

                // Parse sign-in response
                SignInResponseMessage message =
                    WSFederationMessage.CreateFromFormPost(System.Web.HttpContext.Current.Request) as
                    SignInResponseMessage;

                XmlTextReader xmlReader = new XmlTextReader(
                    new StringReader(message.Result));
                XDocument xDoc = XDocument.Load(xmlReader);
                XNamespace xNs = "http://schemas.xmlsoap.org/ws/2005/02/trust";
                var rst = xDoc.Descendants(xNs + "RequestedSecurityToken").FirstOrDefault();
                if (rst == null)
                {
                    throw new ApplicationException("No RequestedSecurityToken element was found in the returned XML token. Ensure an unencrypted SAML 2.0 token is issued.");
                }
                var rstDesc = rst.Descendants().FirstOrDefault();
                if (rstDesc == null)
                {
                    throw new ApplicationException("No valid RequestedSecurityToken element was found in the returned XML token. Ensure an unencrypted SAML 2.0 token is issued.");
                }

                var config = new SecurityTokenHandlerConfiguration();
                config.AudienceRestriction.AllowedAudienceUris.Add(new Uri(settings.AudienceUrl));
                config.CertificateValidator = X509CertificateValidator.None;
                config.IssuerNameRegistry = new AccessControlServiceIssuerRegistry(
                    settings.StsIssuerUrl, settings.X509CertificateThumbprint);

                var securityTokenHandlers = new SecurityTokenHandlerCollection(config);
                securityTokenHandlers.Add(new Saml11SecurityTokenHandler());
                securityTokenHandlers.Add(new Saml2SecurityTokenHandler());
                securityTokenHandlers.Add(new EncryptedSecurityTokenHandler());

                var token = securityTokenHandlers.ReadToken(rstDesc.CreateReader());

                ClaimsIdentityCollection claims = securityTokenHandlers.ValidateToken(token);
                IPrincipal principal = new ClaimsPrincipal(claims);

                // Map claims to local users
                string roleClaimValue = "";
                string usernameClaimValue = "";
                string emailClaimValue = "";
                foreach (var claimsIdentity in claims)
                {
                    foreach (var claim in claimsIdentity.Claims)
                    {
                        if (claim.ClaimType == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" && settings.TranslateClaimsToOrchardRoles)
                        {
                            roleClaimValue = claim.Value;
                        }
                        else if (claim.ClaimType == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress" && settings.TranslateClaimsToOrchardUserProperties)
                        {
                            emailClaimValue = claim.Value;
                        }
                        else if (claim.ClaimType == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")
                        {
                            usernameClaimValue = claim.Value;
                        }
                        else if (claim.ClaimType == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier" && string.IsNullOrEmpty(usernameClaimValue))
                        {
                            usernameClaimValue = claim.Value;
                        }
                    }
                }

                if (string.IsNullOrEmpty(usernameClaimValue))
                {
                    throw new SecurityException("Could not determine username from input claims. Ensure a \"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name\" or \"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier\" claim is issued by the STS.");
                }
                
                IUser user = MembershipService.GetUser(settings.FederatedUsernamePrefix + usernameClaimValue);
                if (user == null)
                {
                    user = MembershipService.CreateUser(new CreateUserParams(settings.FederatedUsernamePrefix + usernameClaimValue,
                                                                          Guid.NewGuid().ToString(), emailClaimValue,
                                                                          Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), true));
                }

                AuthenticationService.SignIn(user, false);

                if (!string.IsNullOrEmpty(roleClaimValue))
                {
                    var role = RoleService.GetRoleByName(roleClaimValue);
                    if (role != null)
                    {
                        UserRolesPartRecord currentRole =
                            UserRolesRepository.Get(r => r.UserId == user.Id && r.Role == role);
                        if (currentRole == null)
                        {
                            UserRolesRepository.Create(new UserRolesPartRecord {UserId = user.Id, Role = role});
                        }
                    }
                }
            }

            return new RedirectResult(ReturnUrl);
        }

        class AccessControlServiceIssuerRegistry : IssuerNameRegistry
        {
            private string stsIssuerUrl = "";
            private string x509CertificateThumbprint = "";

            public AccessControlServiceIssuerRegistry(string stsIssuerUrl, string x509CertificateThumbprint)
            {
                this.stsIssuerUrl = stsIssuerUrl;
                this.x509CertificateThumbprint = x509CertificateThumbprint;
            }

            public override string GetIssuerName(SecurityToken securityToken)
            {
                X509SecurityToken token = securityToken as X509SecurityToken;
                if (token == null)
                {
                    throw new SecurityTokenException("Token is not a X509 Security Token.");
                }

                var cert = token.Certificate;
                if (cert.Thumbprint.Equals(this.x509CertificateThumbprint, StringComparison.OrdinalIgnoreCase))
                {
                    return this.stsIssuerUrl;
                }

                throw new SecurityTokenException("Token not issued by access control service. Ensure thumbprint and STS issuer URK are configured correctly.");
            }
        }
    }
}