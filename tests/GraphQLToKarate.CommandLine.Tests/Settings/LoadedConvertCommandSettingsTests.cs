using FluentAssertions;
using GraphQLToKarate.CommandLine.Settings;
using GraphQLToKarate.Library.Mappings;
using NUnit.Framework;

namespace GraphQLToKarate.CommandLine.Tests.Settings;

[TestFixture]
internal sealed class LoadedConvertCommandSettingsTests
{
    [Test]
    [TestCaseSource(nameof(TestCases))]
    public void ToCommandLineCommand_generates_expected_command_line_command(
        LoadedConvertCommandSettings loadedConvertCommandSettings,
        string expectedCommandLineOptions
    ) => loadedConvertCommandSettings.ToCommandLineCommand().Should().Be(expectedCommandLineOptions);

    private static IEnumerable<TestCaseData> TestCases
    {
        get
        {
            yield return new TestCaseData(
                new LoadedConvertCommandSettings
                {
                    GraphQLSchema = "some schema",
                    InputFile = "schema.graphql",
                    OutputFile = Options.OutputFileOptionDefaultValue,
                    QueryName = Options.QueryNameOptionDefaultValue,
                    MutationName = Options.MutationNameOptionDefaultValue,
                    ExcludeQueries = Options.ExcludeQueriesOptionDefaultValue,
                    IncludeMutations = Options.IncludeMutationsOptionDefaultValue,
                    BaseUrl = Options.BaseUrlOptionDefaultValue,
                    CustomScalarMapping = Options.DefaultCustomScalarMapping,
                    QueryOperationFilter = Options.QueryOperationFilterOptionDefaultValue,
                    MutationOperationFilter = Options.MutationOperationFilterOptionDefaultValue,
                    TypeFilter = Options.TypeFilterOptionDefaultValue
                },
                "graphql-to-karate convert schema.graphql --non-interactive"
            ).SetName("When all options are default, they are not included in the command-line option output.");

            yield return new TestCaseData(
                new LoadedConvertCommandSettings
                {
                    GraphQLSchema = "some schema",
                    InputFile = "schema.graphql",
                    OutputFile = "output.karate",
                    QueryName = "SomeQuery",
                    MutationName = "SomeMutation",
                    ExcludeQueries = true,
                    IncludeMutations = true,
                    BaseUrl = "\"https://localhost:5001\"",
                    CustomScalarMapping = new CustomScalarMapping(
                        new Dictionary<string, string>
                        {
                            { "Date", "string" },
                            { "Long", "number" }
                        }
                    ),
                    QueryOperationFilter = new HashSet<string> { "Query1", "Query2" },
                    MutationOperationFilter = new HashSet<string> { "Mutation1", "Mutation2" },
                    TypeFilter = new HashSet<string> { "Type1", "Type2" }
                },
                "graphql-to-karate convert schema.graphql --non-interactive --output-file output.karate --query-name SomeQuery --mutation-name SomeMutation --exclude-queries --include-mutations --base-url \\\"https://localhost:5001\\\" --custom-scalar-mapping Date:string,Long:number --query-operation-filter Query1,Query2 --mutation-operation-filter Mutation1,Mutation2 --type-filter Type1,Type2"
            ).SetName("When all options are non-default, they are included in the command-line option output.");

            // when some options are non-default and some are default, only the non-default value options are included in the output
            yield return new TestCaseData(
                new LoadedConvertCommandSettings
                {
                    GraphQLSchema = "some schema",
                    InputFile = "schema.graphql",
                    OutputFile = "output.karate",
                    QueryName = Options.QueryNameOptionDefaultValue,
                    MutationName = Options.MutationNameOptionDefaultValue,
                    ExcludeQueries = Options.ExcludeQueriesOptionDefaultValue,
                    IncludeMutations = true,
                    BaseUrl = Options.BaseUrlOptionDefaultValue,
                    CustomScalarMapping = Options.DefaultCustomScalarMapping,
                    QueryOperationFilter = Options.QueryOperationFilterOptionDefaultValue,
                    MutationOperationFilter = Options.MutationOperationFilterOptionDefaultValue,
                    TypeFilter = Options.TypeFilterOptionDefaultValue
                },
                "graphql-to-karate convert schema.graphql --non-interactive --output-file output.karate --include-mutations"
            ).SetName("When some options are non-default and some are default, only the non-default value options are included in the output.");
        }
    }
}