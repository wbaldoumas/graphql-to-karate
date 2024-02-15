using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console.Cli;

namespace GraphQLToKarate.CommandLine.Infrastructure;

/// <inheritdoc cref="ITypeRegistrar"/>
internal sealed class TypeRegistrar(IHostBuilder hostBuilder) : ITypeRegistrar
{
    public ITypeResolver Build() => new TypeResolver(hostBuilder.Build());

    public void Register(Type serviceType, Type implementationType) => hostBuilder.ConfigureServices(
        (_, serviceCollection) => serviceCollection.AddSingleton(serviceType, implementationType)
    );

    public void RegisterInstance(Type serviceType, object implementation) => hostBuilder.ConfigureServices(
        (_, serviceCollection) => serviceCollection.AddSingleton(serviceType, implementation)
    );

    public void RegisterLazy(Type serviceType, Func<object>? implementationFactory)
    {
        if (implementationFactory is null)
        {
            throw new ArgumentNullException(nameof(implementationFactory));
        }

        hostBuilder.ConfigureServices((_, serviceCollection) => serviceCollection.AddSingleton(serviceType, _ => implementationFactory()));
    }
}