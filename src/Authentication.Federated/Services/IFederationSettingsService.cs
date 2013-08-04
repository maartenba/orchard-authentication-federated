using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Authentication.Federated.Models;
using Orchard;

namespace Authentication.Federated.Services
{
    public interface IFederationSettingsService
        : IDependency
    {
        FederationSettingsPart Retrieve();
    }
}