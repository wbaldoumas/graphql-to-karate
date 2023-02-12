using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console.Cli;

namespace GraphQLToKarate.CommandLine.Infrastructure;

internal sealed class TypeRegistrar : ITypeRegistrar
{
    private readonly IHostBuilder _hostBuilder;

    public TypeRegistrar(IHostBuilder hostBuilder) => _hostBuilder = hostBuilder;

    public ITypeResolver Build() => new TypeResolver(_hostBuilder.Build());

    public void Register(Type service, Type implementation) => _hostBuilder.ConfigureServices(
        (_, services) => services.AddSingleton(service, implementation)
    );

    public void RegisterInstance(Type type, object implementation) => _hostBuilder.ConfigureServices(
        (_, services) => services.AddSingleton(type, implementation)
    );

    public void RegisterLazy(Type type, Func<object>? func)
    {
        if (func is null)
        {
            throw new ArgumentNullException(nameof(func));
        }

        _hostBuilder.ConfigureServices((_, services) => services.AddSingleton(type, _ => func()));
    }
}