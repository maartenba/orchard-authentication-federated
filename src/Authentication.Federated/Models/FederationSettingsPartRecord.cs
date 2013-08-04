using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Orchard.ContentManagement.Records;

namespace Authentication.Federated.Models
{
    public class FederationSettingsPartRecord
        : ContentPartRecord
    {
        public virtual bool EnableFederatedAuthentication { get; set; }
        public virtual string FederatedUsernamePrefix { get; set; }
        public virtual bool TranslateClaimsToOrchardUserProperties { get; set; }
        public virtual bool TranslateClaimsToOrchardRoles { get; set; }

        [Required]
        public virtual string StsIssuerUrl { get; set; }

        [Required]
        public virtual string StsLoginUrl { get; set; }

        [Required]
        public virtual string Realm { get; set; }

        [Required]
        public virtual string ReturnUrlBase { get; set; }

        [Required]
        public virtual string AudienceUrl { get; set; }

        [Required]
        public virtual string X509CertificateThumbprint { get; set; }
    }
}