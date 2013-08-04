using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;

namespace Authentication.Federated.Models
{
    public class FederationSettingsPart
         : ContentPart<FederationSettingsPartRecord>
    {
        public bool EnableFederatedAuthentication
        {
            get { return Record.EnableFederatedAuthentication; }
            set { Record.EnableFederatedAuthentication = value; }
        }

        public string FederatedUsernamePrefix
        {
            get { return Record.FederatedUsernamePrefix; }
            set { Record.FederatedUsernamePrefix = value; }
        }

        public bool TranslateClaimsToOrchardUserProperties
        {
            get { return Record.TranslateClaimsToOrchardUserProperties; }
            set { Record.TranslateClaimsToOrchardUserProperties = value; }
        }

        public bool TranslateClaimsToOrchardRoles
        {
            get { return Record.TranslateClaimsToOrchardRoles; }
            set { Record.TranslateClaimsToOrchardRoles = value; }
        }

        public string StsIssuerUrl
        {
            get { return Record.StsIssuerUrl; }
            set { Record.StsIssuerUrl = value; }
        }

        public string StsLoginUrl
        {
            get { return Record.StsLoginUrl; }
            set { Record.StsLoginUrl = value; }
        }

        public string Realm
        {
            get { return Record.Realm; }
            set { Record.Realm = value; }
        }

        public string ReturnUrlBase
        {
            get { return Record.ReturnUrlBase; }
            set { Record.ReturnUrlBase = value; }
        }

        public string AudienceUrl
        {
            get { return Record.AudienceUrl; }
            set { Record.AudienceUrl = value; }
        }

        public string X509CertificateThumbprint
        {
            get { return Record.X509CertificateThumbprint; }
            set { Record.X509CertificateThumbprint = value; }
        }
    }
}