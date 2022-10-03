using Polly;

using System;
using System.Collections.Generic;

using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;

using Codemancer.Extensions.Wcf.Polly;

namespace Codemancer.Extensions.DependencyInjection
{
    public static class WcfServiceBuilderExtensions
    {
        public static IWcfServiceBuilder AddWcf<TService>(this IServiceCollection services) where TService : class
        {
            // Register Channel Factory
            services.AddSingleton(serviceProvider =>
            {
                return BuildChannelFactory<TService>(serviceProvider, typeof(TService).FullName);
            });

            // Register WCF Service
            services.AddTransient(typeof(TService), serviceProvider =>
            {
                return serviceProvider.GetRequiredService<ChannelFactory<TService>>().CreateChannel();
            });

            return new WcfServiceBuilder(typeof(TService).FullName, services);
        }

        public static IWcfServiceBuilder ConfigureServiceEndpoint(this IWcfServiceBuilder builder, Action<ServiceEndpoint> configureEndpoint)
        {
            builder.Services.AddTransient<IConfigureOptions<ServiceEndpointOptions>>(services =>
            {
                return new ConfigureNamedOptions<ServiceEndpointOptions>(builder.Name, (options) =>
                {
                    options.ServiceEndpointActions.Add(configureEndpoint);
                });
            });

            return builder;
        }

        public static IWcfServiceBuilder AddPolicyHandler(this IWcfServiceBuilder builder, IAsyncPolicy<Message> policy)
        {
            builder.ConfigureServiceEndpoint(serviceEndpoint =>
            {             
                var bindingCollection = serviceEndpoint.Binding.CreateBindingElements();

                var bindingList = new List<BindingElement>();
                foreach (var binding in bindingCollection)
                {
                    // Wrap current transport binding with policy transport
                    if (binding.GetType().IsSubclassOf(typeof(TransportBindingElement)))
                    {                      
                        bindingList.Add(new HttpPolicyBinding((TransportBindingElement)binding, policy));
                    }
                    else
                    {
                        bindingList.Add(binding);
                    }
                }

               serviceEndpoint.Binding = new CustomBinding(bindingList);              
            });

            return builder;
        }

        private static ChannelFactory<TServiceContract> BuildChannelFactory<TServiceContract>(IServiceProvider services, string name) where TServiceContract : class
        {
            // Create channel factory
            var contractDescription = ContractDescription.GetContract(typeof(TServiceContract));
            var factory = new ChannelFactory<TServiceContract>(new ServiceEndpoint(contractDescription));

            factory.Endpoint.Binding = new BasicHttpBinding(); // Default

            // Configure channel factory
            var options = services.GetService<IOptionsMonitor<ServiceEndpointOptions>>()!.Get(name);
            foreach (var configureEndpoint in options.ServiceEndpointActions)
            {
                configureEndpoint(factory.Endpoint);
            }

            // Validate required configuration
            if (factory.Endpoint.Address == null)
            {
                throw new InvalidOperationException($"The Address property on ServiceEndpoint must be specified. Please configure the service endpoint address using '{nameof(WcfServiceBuilderExtensions.ConfigureServiceEndpoint)}' on {nameof(IWcfServiceBuilder)}. ");
            }

            return factory;
        }
    }
}
