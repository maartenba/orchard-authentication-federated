using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Authentication.Federated.Models;
using Orchard.Caching;
using Orchard.ContentManagement;

namespace Authentication.Federated.Services
{
    public class FederationSettingsService
        : IFederationSettingsService
    {
        protected IContentManager ContentManager { get; private set; }
        protected ICacheManager CacheManager { get; private set; }
        protected ISignals Signals { get; private set; }

        public FederationSettingsService(IContentManager contentManager, ICacheManager cacheManager, ISignals signals)
        {
            this.ContentManager = contentManager;
            this.CacheManager = cacheManager;
            this.Signals = signals;
        }

        public FederationSettingsPart Retrieve()
        {
            return CacheManager.Get("Authentication.Federated.Settings",
                                     ctx =>
                                     {
                                         ctx.Monitor(Signals.When("Authentication.Federated.SettingsChanged"));
                                         FederationSettingsPart settings =
                                             ContentManager.Query<FederationSettingsPart, FederationSettingsPartRecord>().List().FirstOrDefault();
                                         if (settings != null)
                                         {
                                             return settings;
                                         }

                                         settings = new FederationSettingsPart();
                                         settings.Record = new FederationSettingsPartRecord();
                                         return settings;
                                     });
        }
    }
}