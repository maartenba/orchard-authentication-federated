using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Authentication.Federated.Models;
using Authentication.Federated.Services;
using Orchard.Caching;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Authentication.Federated.ViewModels;
using Orchard.Localization;

namespace Authentication.Federated.Drivers
{
    public class FederationSettingsPartDriver
        : ContentPartDriver<FederationSettingsPart>
    {
        protected ISignals Signals { get; private set; }

        public FederationSettingsPartDriver(ISignals signals)
        {
            T = NullLocalizer.Instance;
            this.Signals = signals;
        }

        public Localizer T { get; set; }

        protected override string Prefix { get { return "FederationSettings"; } }

        // GET
        protected override DriverResult Editor(FederationSettingsPart part, dynamic shapeHelper)
        {
            return ContentShape("Parts_FederationSettings_Edit",
                                () => shapeHelper.EditorTemplate(
                                    TemplateName: "FederationSettings",
                                    Model: new FederationSettingsPartViewModel
                                               {
                                                   EnableFederatedAuthentication = part.Record.EnableFederatedAuthentication,
                                                   FederatedUsernamePrefix = part.Record.FederatedUsernamePrefix,
                                                   TranslateClaimsToOrchardUserProperties = part.Record.TranslateClaimsToOrchardUserProperties,
                                                   TranslateClaimsToOrchardRoles = part.Record.TranslateClaimsToOrchardRoles,
                                                   StsIssuerUrl = part.Record.StsIssuerUrl,
                                                   StsLoginUrl = part.Record.StsLoginUrl,
                                                   Realm = part.Record.Realm,
                                                   ReturnUrlBase = part.Record.ReturnUrlBase,
                                                   AudienceUrl = part.Record.AudienceUrl,
                                                   X509CertificateThumbprint = part.Record.X509CertificateThumbprint
                                               },
                                    Prefix: Prefix));
        }

        // POST
        protected override DriverResult Editor(
            FederationSettingsPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            Signals.Trigger("Authentication.Federated.SettingsChanged");
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }
    }
}