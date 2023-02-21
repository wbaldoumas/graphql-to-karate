using FluentAssertions;
using GraphQLToKarate.CommandLine.Commands;
using GraphQLToKarate.CommandLine.Infrastructure;
using GraphQLToKarate.CommandLine.Settings;
using GraphQLToKarate.Library.Builders;
using GraphQLToKarate.Library.Mappings;
using GraphQLToKarate.Library.Tokens;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using Spectre.Console.Cli;
using System.IO.Abstractions;

namespace GraphQLToKarate.CommandLine.Tests.Infrastructure;

[TestFixture]
internal sealed class CommandAppConfiguratorTests
{
    private IFile? _mockFile;
    private IFileSystem? _mockFileSystem;
    private ILogger<ConvertCommand>? _mockLogger;
    private IConvertCommandSettingsLoader? _mockConvertCommandSettingsLoader;
    private ConvertCommandSettings? _mockConvertCommandSettings;
    private IGraphQLToKarateConverterBuilder? _mockGraphQLToKarateConverterBuilder;
    private ICustomScalarMappingValidator? _mockCustomScalarMappingValidator;
    private ConvertCommand? _mockConvertCommand;
    private ITypeResolver? _mockTypeResolver;
    private ITypeRegistrar? _mockTypeRegistrar;

    [SetUp]
    public void SetUp()
    {
        _mockFile = Substitute.For<IFile>();
        _mockFileSystem = Substitute.For<IFileSystem>();
        _mockFileSystem.File.Returns(_mockFile);
        _mockCustomScalarMappingValidator = Substitute.For<ICustomScalarMappingValidator>();
        _mockConvertCommandSettings = new ConvertCommandSettings(_mockFile, _mockCustomScalarMappingValidator);
        _mockLogger = Substitute.For<ILogger<ConvertCommand>>();
        _mockConvertCommandSettingsLoader = Substitute.For<IConvertCommandSettingsLoader>();
        _mockGraphQLToKarateConverterBuilder = Substitute.For<IGraphQLToKarateConverterBuilder>();

        _mockConvertCommand = new ConvertCommand(
            _mockFileSystem,
            _mockConvertCommandSettingsLoader,
            _mockGraphQLToKarateConverterBuilder,
            _mockLogger
        );

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

        _mockConvertCommandSettingsLoader!
            .LoadAsync(Arg.Any<ConvertCommandSettings>())
            .Returns(new LoadedConvertCommandSettings
            {
                GraphQLSchema = "some GraphQL schema",
                OutputFile = "graphql.feature",
                CustomScalarMapping = new CustomScalarMapping(),
                ExcludeQueries = false,
                BaseUrl = "baseUrl",
                QueryName = GraphQLToken.Query,
                TypeFilter = new HashSet<string>(StringComparer.OrdinalIgnoreCase),
                OperationFilter = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            });

        // act
        var result = await CommandAppConfigurator
            .ConfigureCommandApp(_mockTypeRegistrar!)
            .RunAsync(new[] { "convert", "schema.gql" });

        // assert
        result.Should().Be(0);
    }
}