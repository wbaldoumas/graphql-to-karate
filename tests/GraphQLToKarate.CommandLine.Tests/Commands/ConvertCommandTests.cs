using FluentAssertions;
using GraphQLToKarate.CommandLine.Commands;
using GraphQLToKarate.CommandLine.Settings;
using GraphQLToKarate.Library.Builders;
using GraphQLToKarate.Library.Converters;
using GraphQLToKarate.Library.Mappings;
using GraphQLToKarate.Library.Tokens;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using Spectre.Console.Cli;
using System.IO.Abstractions;

namespace GraphQLToKarate.CommandLine.Tests.Commands;

[TestFixture]
internal sealed class ConvertCommandTests
{
    private IFile? _mockFile;
    private IFileSystem? _mockFileSystem;
    private IConvertCommandSettingsLoader? _mockConvertCommandSettingsLoader;
    private IGraphQLToKarateConverterBuilder? _mockGraphQLToKarateConverterBuilder;
    private ICustomScalarMappingValidator? _mockCustomScalarMappingValidator;
    private ILogger<ConvertCommand>? _mockLogger;
    private AsyncCommand<ConvertCommandSettings>? _subjectUnderTest;

    [SetUp]
    public void SetUp()
    {
        _mockFile = Substitute.For<IFile>();
        _mockFileSystem = Substitute.For<IFileSystem>();
        _mockConvertCommandSettingsLoader = Substitute.For<IConvertCommandSettingsLoader>();
        _mockGraphQLToKarateConverterBuilder = Substitute.For<IGraphQLToKarateConverterBuilder>();
        _mockCustomScalarMappingValidator = Substitute.For<ICustomScalarMappingValidator>();
        _mockLogger = Substitute.For<ILogger<ConvertCommand>>();
        _mockFileSystem.File.Returns(_mockFile);

        _subjectUnderTest = new ConvertCommand(
            _mockFileSystem,
            _mockConvertCommandSettingsLoader,
            _mockGraphQLToKarateConverterBuilder,
            _mockLogger
        );
    }

    [Test]
    public async Task ConvertCommand_executes_as_expected()
    {
        // arrange
        var commandContext = new CommandContext(
            remaining: Substitute.For<IRemainingArguments>(),
            name: "Test CommandContext",
            data: new { }
        );

        var convertCommandSettings = new ConvertCommandSettings(_mockFile!, _mockCustomScalarMappingValidator!)
        {
            InputFile = "schema.graphql",
            OutputFile = "graphql.feature"
        };

        const string schemaFileContent = "some GraphQL schema";
        const string karateFeature = "some Karate feature";

        var mockGraphQLToKarateConverter = Substitute.For<IGraphQLToKarateConverter>();

        mockGraphQLToKarateConverter.Convert(Arg.Any<string>()).Returns(karateFeature);

        _mockGraphQLToKarateConverterBuilder!
            .Configure()
            .Build()
            .Returns(mockGraphQLToKarateConverter);

        _mockConvertCommandSettingsLoader!
            .LoadAsync(convertCommandSettings)
            .Returns(new LoadedConvertCommandSettings
            {
                GraphQLSchema = schemaFileContent,
                OutputFile = convertCommandSettings.OutputFile!,
                CustomScalarMapping = new CustomScalarMapping(),
                ExcludeQueries = convertCommandSettings.ExcludeQueries,
                IncludeMutations = convertCommandSettings.IncludeMutations,
                BaseUrl = convertCommandSettings.BaseUrl ?? "baseUrl",
                QueryName = convertCommandSettings.QueryName ?? GraphQLToken.Query,
                MutationName = convertCommandSettings.MutationName ?? GraphQLToken.Mutation,
                TypeFilter = convertCommandSettings.TypeFilter,
                OperationFilter = convertCommandSettings.OperationFilter
            });

        // act
        var result = await _subjectUnderTest!.ExecuteAsync(commandContext, convertCommandSettings);

        // assert
        result.Should().Be(0);

        await _mockConvertCommandSettingsLoader!
            .Received()
            .LoadAsync(convertCommandSettings);
    }
}