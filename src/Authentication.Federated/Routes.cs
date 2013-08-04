using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Authentication.Federated.Routing;
using Orchard.Mvc.Routes;

namespace Authentication.Federated
{
    public class Routes : IRouteProvider
    {
        private readonly IFederationEnabledRouteConstraint federationRouteConstraint;

        public Routes(IFederationEnabledRouteConstraint federationRouteConstraint)
        {
            this.federationRouteConstraint = federationRouteConstraint;
        }

        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (var routeDescriptor in GetRoutes())
            {
                routes.Add(routeDescriptor);
            }
        }

        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[] {
                             // Override standard Users/Account/LogOn
                             new RouteDescriptor {   Priority = 1,
                                                     Route = new Route(
                                                         "Users/Account/LogOn",
                                                         new RouteValueDictionary {
                                                                                      {"area", "Authentication.Federated"},
                                                                                      {"controller", "FederatedAuthentication"},
                                                                                      {"action", "LogOn"}
                                                         },
                                                         new RouteValueDictionary {{ "controller", this.federationRouteConstraint }},
                                                         new RouteValueDictionary {
                                                                                      {"area", "Authentication.Federated"}
                                                         },
                                                         new MvcRouteHandler())
                             },

                             // Override standard Users/Account/AccessDenied
                             new RouteDescriptor {   Priority = 1,
                                                     Route = new Route(
                                                         "Users/Account/AccessDenied",
                                                         new RouteValueDictionary {
                                                                                      {"area", "Authentication.Federated"},
                                                                                      {"controller", "FederatedAuthentication"},
                                                                                      {"action", "LogOn"}
                                                         },
                                                         new RouteValueDictionary {{ "controller", this.federationRouteConstraint }},
                                                         new RouteValueDictionary {
                                                                                      {"area", "Authentication.Federated"}
                                                         },
                                                         new MvcRouteHandler())
                             },

                             // Return URL for STS
                             new RouteDescriptor {   Priority = 5,
                                                     Route = new Route(
                                                         "FederatedAuthentication/ProcessToken",
                                                         new RouteValueDictionary {
                                                                                      {"area", "Authentication.Federated"},
                                                                                      {"controller", "FederatedAuthentication"},
                                                                                      {"action", "Authenticate"}
                                                         },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", "Authentication.Federated"}
                                                         },
                                                         new MvcRouteHandler())
                             },

                             // Test sign-in URL
                             new RouteDescriptor {   Priority = 5,
                                                     Route = new Route(
                                                         "FederatedAuthentication/TestLogOn",
                                                         new RouteValueDictionary {
                                                                                      {"area", "Authentication.Federated"},
                                                                                      {"controller", "FederatedAuthentication"},
                                                                                      {"action", "TestLogOn"}
                                                         },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", "Authentication.Federated"}
                                                         },
                                                         new MvcRouteHandler())
                             }
                         };
        }
    }
}