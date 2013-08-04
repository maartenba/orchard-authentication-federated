using System.ComponentModel.DataAnnotations;

namespace Authentication.Federated.ViewModels
{
    public class FederationSettingsPartViewModel
    {
        public bool EnableFederatedAuthentication { get; set; }
        public string FederatedUsernamePrefix { get; set; }
        public bool TranslateClaimsToOrchardUserProperties { get; set; }
        public bool TranslateClaimsToOrchardRoles { get; set; }

        [Required]
        public string StsIssuerUrl { get; set; }

        [Required]
        public string StsLoginUrl { get; set; }

        [Required]
        public string Realm { get; set; }

        [Required]
        public string ReturnUrlBase { get; set; }

        [Required(ErrorMessage = "BLAAAA")]
        public string AudienceUrl { get; set; }

        [Required]
        public string X509CertificateThumbprint { get; set; }
    }
}