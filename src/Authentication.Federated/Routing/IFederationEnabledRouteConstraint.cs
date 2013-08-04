using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using Orchard;

namespace Authentication.Federated.Routing
{
    public interface IFederationEnabledRouteConstraint
        : IRouteConstraint, IDependency
    {
    }
}