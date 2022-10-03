using Microsoft.Extensions.DependencyInjection;

namespace Codemancer.Extensions.DependencyInjection
{
    public class WcfServiceBuilder: IWcfServiceBuilder
    {
        public WcfServiceBuilder(string name, IServiceCollection services)
        {
            Name = name;
            Services = services;
        }

        public string Name { get; }

        public IServiceCollection Services { get; }
    }
}