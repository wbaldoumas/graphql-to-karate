using FluentAssertions;
using GraphQLToKarate.CommandLine.Commands;
using GraphQLToKarate.CommandLine.Settings;
using NSubstitute;
using NUnit.Framework;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Testing;
using System.IO.Abstractions;

namespace GraphQLToKarate.CommandLine.Tests.Commands;

[TestFixture]
internal sealed class ConvertCommandTests
{
    private IAnsiConsole? _mockConsole;
    private IFile? _mockFile;
    private AsyncCommand<ConvertCommandSettings>? _subjectUnderTest;

    [SetUp]
    public void SetUp()
    {
        _mockConsole = new TestConsole()
            .Colors(ColorSystem.Standard)
            .EmitAnsiSequences();

        _mockFile = Substitute.For<IFile>();
        _subjectUnderTest = new ConvertCommand(_mockConsole, _mockFile);
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
            GraphQLSchemaFile = "schema.graphql"
        };

        const string schemaFileContent = "some GraphQL schema";

        _mockFile!
            .ReadAllTextAsync(convertCommandSettings.GraphQLSchemaFile)
            .Returns(schemaFileContent);

        // act
        var result = await _subjectUnderTest!.ExecuteAsync(commandContext, convertCommandSettings);

        // assert
        result.Should().Be(0);

        await _mockFile
            .ReceivedWithAnyArgs()
            .ReadAllTextAsync(default!);

        // initial command implementation just writes the schema to console...
        (_mockConsole as TestConsole)!.Output.Should().Contain(schemaFileContent);
    }
}