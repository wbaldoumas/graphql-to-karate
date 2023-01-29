using Spectre.Console.Cli;

namespace GraphQLToKarate.CommandLine.Infrastructure;

internal sealed class TypeResolver : ITypeResolver, IDisposable
{
    private readonly IServiceProvider _serviceProvider;

    public TypeResolver(IServiceProvider? provider) =>
        _serviceProvider = provider ?? throw new ArgumentNullException(nameof(provider));

    public object? Resolve(Type? type) => type is null ? null : _serviceProvider.GetService(type);

    public void Dispose()
    {
        if (_serviceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}