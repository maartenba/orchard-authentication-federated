using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Authentication.Federated.Models;
using JetBrains.Annotations;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;

namespace Authentication.Federated.Handlers
{
    [UsedImplicitly]
    public class FederationSettingsPartHandler 
        : ContentHandler
    {
        public FederationSettingsPartHandler(IRepository<FederationSettingsPartRecord> repository)
        {
            Filters.Add(new ActivatingFilter<FederationSettingsPart>("Site"));
            Filters.Add(StorageFilter.For(repository));
        }
    }
}