using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace GraphQLToKarate.CommandLine.Infrastructure;

internal sealed class TypeRegistrar : ITypeRegistrar
{
    private readonly IServiceCollection _serviceCollection;

    public TypeRegistrar(IServiceCollection serviceCollection) =>
        _serviceCollection = serviceCollection;

    public ITypeResolver Build() =>
        new TypeResolver(_serviceCollection.BuildServiceProvider());

    public void Register(Type service, Type implementation) =>
        _serviceCollection.AddSingleton(service, implementation);

    public void RegisterInstance(Type service, object implementation) =>
        _serviceCollection.AddSingleton(service, implementation);

    public void RegisterLazy(Type service, Func<object>? func)
    {
        if (func is null)
        {
            throw new ArgumentNullException(nameof(func));
        }

        _serviceCollection.AddSingleton(service, _ => func());
    }
}