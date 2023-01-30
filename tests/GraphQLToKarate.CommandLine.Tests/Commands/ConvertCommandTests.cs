using FluentAssertions;
using GraphQLToKarate.CommandLine.Commands;
using GraphQLToKarate.CommandLine.Settings;
using NSubstitute;
using NUnit.Framework;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Testing;
using System.IO.Abstractions;
using GraphQLToKarate.Library.Builders;
using GraphQLToKarate.Library.Converters;

namespace GraphQLToKarate.CommandLine.Tests.Commands;

[TestFixture]
internal sealed class ConvertCommandTests
{
    private IAnsiConsole? _mockConsole;
    private IFile? _mockFile;
    private IFileSystem? _mockFileSystem;
    private IGraphQLToKarateConverterBuilder? _mockGraphQLToKarateConverterBuilder;
    private AsyncCommand<ConvertCommandSettings>? _subjectUnderTest;

    [SetUp]
    public void SetUp()
    {
        _mockConsole = new TestConsole()
            .Colors(ColorSystem.Standard)
            .EmitAnsiSequences();

        _mockFile = Substitute.For<IFile>();
        _mockFileSystem = Substitute.For<IFileSystem>();
        _mockGraphQLToKarateConverterBuilder = Substitute.For<IGraphQLToKarateConverterBuilder>();

        _mockFileSystem.File.Returns(_mockFile);

        _subjectUnderTest = new ConvertCommand(_mockConsole, _mockFileSystem, _mockGraphQLToKarateConverterBuilder);
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

        var convertCommandSettings = new ConvertCommandSettings(_mockFile!)
        {
            InputFile = "schema.graphql"
        };

        const string schemaFileContent = "some GraphQL schema";
        const string karateFeature = "some Karate feature";

        var mockGraphQLToKarateConverter = Substitute.For<IGraphQLToKarateConverter>();

        mockGraphQLToKarateConverter.Convert(Arg.Any<string>()).Returns(karateFeature);

        _mockGraphQLToKarateConverterBuilder!
            .Build()
            .Returns(mockGraphQLToKarateConverter);

        _mockFile!
            .ReadAllTextAsync(convertCommandSettings.InputFile)
            .Returns(schemaFileContent);

        // act
        var result = await _subjectUnderTest!.ExecuteAsync(commandContext, convertCommandSettings);

        // assert
        result.Should().Be(0);

        await _mockFile
            .ReceivedWithAnyArgs()
            .ReadAllTextAsync(default!);


        // initial command implementation just writes the schema to console...
        (_mockConsole as TestConsole)!.Output.Should().Contain("Running GraphQL to Karate conversion...");
    }
}