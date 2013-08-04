using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using Authentication.Federated.Models;
using Authentication.Federated.Services;
using Orchard;
using Orchard.ContentManagement;

namespace Authentication.Federated.Routing
{
    public class FederationEnabledRouteConstraint
        : IFederationEnabledRouteConstraint
    {
        protected IFederationSettingsService Settings { get; private set; }

        public FederationEnabledRouteConstraint(IFederationSettingsService settings)
        {
            this.Settings = settings;
        }    

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            return Settings.Retrieve().EnableFederatedAuthentication;
        }
    }
}