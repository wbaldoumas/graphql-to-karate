using FluentAssertions;
using GraphQLToKarate.CommandLine.Infrastructure;
using GraphQLToKarate.CommandLine.Settings;
using NSubstitute;
using NUnit.Framework;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Testing;
using System.IO.Abstractions;
using GraphQLToKarate.CommandLine.Commands;
using GraphQLToKarate.Library.Builders;

namespace GraphQLToKarate.CommandLine.Tests.Infrastructure;

[TestFixture]
internal sealed class CommandAppConfiguratorTests
{
    private IFile? _mockFile;
    private IFileSystem? _mockFileSystem;
    private IAnsiConsole? _mockAnsiConsole;
    private ConvertCommandSettings? _mockConvertCommandSettings;
    private IGraphQLToKarateConverterBuilder? _mockGraphQLToKarateConverterBuilder;
    private ConvertCommand? _mockConvertCommand;
    private ITypeResolver? _mockTypeResolver;
    private ITypeRegistrar? _mockTypeRegistrar;
    
    [SetUp]
    public void SetUp()
    {
        _mockFile = Substitute.For<IFile>();
        _mockFileSystem = Substitute.For<IFileSystem>();

        _mockFileSystem.File.Returns(_mockFile);

        _mockConvertCommandSettings = new ConvertCommandSettings(_mockFile);
        _mockAnsiConsole = new TestConsole();
        _mockGraphQLToKarateConverterBuilder = Substitute.For<IGraphQLToKarateConverterBuilder>();
        _mockConvertCommand = new ConvertCommand(_mockAnsiConsole, _mockFileSystem, _mockGraphQLToKarateConverterBuilder);
        _mockTypeResolver = Substitute.For<ITypeResolver>();
        _mockTypeRegistrar = Substitute.For<ITypeRegistrar>();
    }

    [Test]
    public async Task CommandAppConfigurator_configures_expected_CommandApp()
    {
        // arrange
        _mockFile!.Exists(Arg.Any<string>()).Returns(true);

        _mockTypeResolver!.Resolve(typeof(ConvertCommandSettings)).Returns(_mockConvertCommandSettings);
        _mockTypeResolver.Resolve(typeof(ConvertCommand)).Returns(_mockConvertCommand);

        _mockTypeRegistrar!
            .Build()
            .Returns(_mockTypeResolver);

        // act
        var result = await CommandAppConfigurator
            .ConfigureCommandApp(_mockTypeRegistrar!)
            .RunAsync(new[] { "convert", "schema.gql" });

        // assert
        result.Should().Be(0);

        (_mockAnsiConsole as TestConsole)!.Output.Should().NotBeEmpty();
    }
}