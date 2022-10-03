using Microsoft.Extensions.DependencyInjection;

namespace Codemancer.Extensions.DependencyInjection
{
    public interface IWcfServiceBuilder
    {
        string Name { get; }

        IServiceCollection Services { get; }
    }
}