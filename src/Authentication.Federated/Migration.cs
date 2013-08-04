using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Data.Migration;
using Orchard.Data.Migration.Schema;

namespace Authentication.Federated
{
    public class Migration
        : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable("FederationSettingsPartRecord",
                table => table
                    .ContentPartRecord()
                    .Column<bool>("EnableFederatedAuthentication", c => c.WithDefault(false))
                    .Column<string>("FederatedUsernamePrefix", c => c.WithDefault("federated_"))
                    .Column<bool>("TranslateClaimsToOrchardUserProperties", c => c.WithDefault(true))
                    .Column<bool>("TranslateClaimsToOrchardRoles", c => c.WithDefault(true))
                    .Column<string>("StsIssuerUrl", c => c.Unlimited())
                    .Column<string>("StsLoginUrl", c => c.Unlimited())
                    .Column<string>("Realm", c => c.Unlimited())
                    .Column<string>("ReturnUrlBase", c => c.Unlimited())
                    .Column<string>("AudienceUrl", c => c.Unlimited())
                    .Column<string>("X509CertificateThumbprint", c => c.Unlimited())
                );

            return 1;
        }
    }
}