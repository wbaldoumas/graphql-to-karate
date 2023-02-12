using Microsoft.Extensions.Hosting;
using Spectre.Console.Cli;

namespace GraphQLToKarate.CommandLine.Infrastructure;

internal sealed class TypeResolver : ITypeResolver, IDisposable
{
    private readonly IHost _host;

    public TypeResolver(IHost? host) => _host = host ?? throw new ArgumentNullException(nameof(host));

    public object? Resolve(Type? type) => type is not null ? _host.Services.GetService(type) : null;

    public void Dispose() => _host.Dispose();
}