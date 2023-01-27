using GraphQLToKarate.CommandLine.Settings;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;

namespace GraphQLToKarate.CommandLine.Infrastructure;

internal static class TypeRegistrarConfigurator
{
    public static ITypeRegistrar ConfigureTypeRegistrar()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddSingleton<IFileSystem, FileSystem>();
        serviceCollection.AddSingleton<IFile, FileWrapper>();
        serviceCollection.AddTransient<ConvertCommandSettings>();

        return new TypeRegistrar(serviceCollection);
    }
}