using System;
using System.Collections.Generic;
using System.ServiceModel.Description;

namespace Codemancer.Extensions.DependencyInjection
{
    public class ServiceEndpointOptions
    {
        public IList<Action<ServiceEndpoint>> ServiceEndpointActions { get; } = new List<Action<ServiceEndpoint>>();
    }
}