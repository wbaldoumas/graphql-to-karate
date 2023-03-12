using Microsoft.Extensions.Hosting;
using Spectre.Console.Cli;

namespace GraphQLToKarate.CommandLine.Infrastructure;

/// <inheritdoc cref="ITypeResolver"/>
internal sealed class TypeResolver : ITypeResolver, IDisposable
{
    private readonly IHost _host;

    public TypeResolver(IHost? host) => _host = host ?? throw new ArgumentNullException(nameof(host));

    public object? Resolve(Type? serviceType) => serviceType is not null ? _host.Services.GetService(serviceType) : null;

    public void Dispose() => _host.Dispose();
}