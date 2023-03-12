using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console.Cli;

namespace GraphQLToKarate.CommandLine.Infrastructure;

/// <inheritdoc cref="ITypeRegistrar"/>
internal sealed class TypeRegistrar : ITypeRegistrar
{
    private readonly IHostBuilder _hostBuilder;

    public TypeRegistrar(IHostBuilder hostBuilder) => _hostBuilder = hostBuilder;

    public ITypeResolver Build() => new TypeResolver(_hostBuilder.Build());

    public void Register(Type serviceType, Type implementationType) => _hostBuilder.ConfigureServices(
        (_, serviceCollection) => serviceCollection.AddSingleton(serviceType, implementationType)
    );

    public void RegisterInstance(Type serviceType, object implementation) => _hostBuilder.ConfigureServices(
        (_, serviceCollection) => serviceCollection.AddSingleton(serviceType, implementation)
    );

    public void RegisterLazy(Type serviceType, Func<object>? implementationFactory)
    {
        if (implementationFactory is null)
        {
            throw new ArgumentNullException(nameof(implementationFactory));
        }

        _hostBuilder.ConfigureServices((_, serviceCollection) => serviceCollection.AddSingleton(serviceType, _ => implementationFactory()));
    }
}