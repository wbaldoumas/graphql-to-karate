using GraphQLToKarate.Library.Adapters;
using GraphQLToKarate.Library.Mappings;
using GraphQLToKarate.Library.Parsers;
using GraphQLToKarate.Library.Settings;
using Spectre.Console;
using System.Diagnostics.CodeAnalysis;

namespace GraphQLToKarate.CommandLine.Settings;

/// <inheritdoc cref="IConvertCommandSettingsPrompt"/>
[ExcludeFromCodeCoverage]
internal class ConvertCommandSettingsPrompt : IConvertCommandSettingsPrompt
{
    private readonly IGraphQLSchemaParser _graphQLSchemaParser;
    private readonly ICustomScalarMappingLoader _customScalarMappingLoader;
    private readonly IAnsiConsole _ansiConsole;

    public ConvertCommandSettingsPrompt(
        IGraphQLSchemaParser graphQLSchemaParser,
        ICustomScalarMappingLoader customScalarMappingLoader,
        IAnsiConsole ansiConsole)
    {
        _graphQLSchemaParser = graphQLSchemaParser;
        _customScalarMappingLoader = customScalarMappingLoader;
        _ansiConsole = ansiConsole;
    }

    public async Task<LoadedConvertCommandSettings> PromptAsync(
        LoadedConvertCommandSettings initialLoadedConvertCommandSettings)
    {
        var outputFile = PromptForOutputFile(initialLoadedConvertCommandSettings);
        var queryName = PromptForQueryName(initialLoadedConvertCommandSettings);
        var mutationName = PromptForMutationName(initialLoadedConvertCommandSettings);
        var excludeQueries = PromptForExcludeQueriesChoice(initialLoadedConvertCommandSettings);
        var includeMutations = PromptForIncludeMutationsChoice(initialLoadedConvertCommandSettings);
        var baseUrl = PromptForBaseUrl(initialLoadedConvertCommandSettings);

        var graphQLDocumentAdapter = new GraphQLDocumentAdapter(
            _graphQLSchemaParser.Parse(initialLoadedConvertCommandSettings.GraphQLSchema),
            new GraphQLToKarateSettings
            {
                QueryName = queryName,
                MutationName = mutationName
            }
        );

        var queryOperationFilter = PromptForQueryOperationFilter(graphQLDocumentAdapter, excludeQueries);
        var mutationOperationFilter = PromptForMutationOperationFilter(graphQLDocumentAdapter, includeMutations);
        var graphQLTypeFilter = PromptForTypeFilter(graphQLDocumentAdapter);
        var customScalarMapping = await PromptForCustomScalarMappingAsync().ConfigureAwait(false);

        _ansiConsole.Write(new Padder(new Rule()).PadLeft(1).PadRight(1));

        return new LoadedConvertCommandSettings
        {
            GraphQLSchema = initialLoadedConvertCommandSettings.GraphQLSchema,
            QueryName = queryName,
            MutationName = mutationName,
            BaseUrl = baseUrl,
            ExcludeQueries = excludeQueries,
            IncludeMutations = includeMutations,
            QueryOperationFilter = queryOperationFilter,
            MutationOperationFilter = mutationOperationFilter,
            TypeFilter = graphQLTypeFilter,
            CustomScalarMapping = customScalarMapping,
            OutputFile = outputFile
        };
    }

    private string PromptForOutputFile(LoadedConvertCommandSettings initialLoadedConvertCommandSettings) =>
        _ansiConsole.Prompt(
            new TextPrompt<string>("Enter the path and file name of where to generate the Karate feature file:")
            {
                AllowEmpty = false,
            }.DefaultValue(initialLoadedConvertCommandSettings.OutputFile)
        );

    private string PromptForQueryName(LoadedConvertCommandSettings initialLoadedConvertCommandSettings) =>
        _ansiConsole.Prompt(
            new TextPrompt<string>("Enter the name of the GraphQL query type:")
            {
                AllowEmpty = false,
            }.DefaultValue(initialLoadedConvertCommandSettings.QueryName)
        );

    private string PromptForMutationName(LoadedConvertCommandSettings initialLoadedConvertCommandSettings) =>
        _ansiConsole.Prompt(
            new TextPrompt<string>("Enter the name of the GraphQL mutation type:")
            {
                AllowEmpty = false,
            }.DefaultValue(initialLoadedConvertCommandSettings.MutationName)
        );

    private bool PromptForExcludeQueriesChoice(LoadedConvertCommandSettings initialLoadedConvertCommandSettings) =>
        _ansiConsole.Prompt(
            new ConfirmationPrompt("Exclude queries from the generated Karate feature?")
            {
                DefaultValue = initialLoadedConvertCommandSettings.ExcludeQueries
            }
        );

    private bool PromptForIncludeMutationsChoice(LoadedConvertCommandSettings initialLoadedConvertCommandSettings) =>
        _ansiConsole.Prompt(
            new ConfirmationPrompt("Include mutations in the generated Karate feature?")
            {
                DefaultValue = initialLoadedConvertCommandSettings.IncludeMutations
            }
        );

    private string PromptForBaseUrl(LoadedConvertCommandSettings initialLoadedConvertCommandSettings) =>
        _ansiConsole.Prompt(
            new TextPrompt<string>("Enter the base URL to be used in the Karate feature:")
            {
                AllowEmpty = false,
            }.DefaultValue(initialLoadedConvertCommandSettings.BaseUrl)
        );

    private ISet<string> PromptForQueryOperationFilter(IGraphQLDocumentAdapter graphQLDocumentAdapter, bool excludeQueries)
    {
        if (excludeQueries)
        {
            return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        var graphQLQueryType = graphQLDocumentAdapter.GraphQLQueryTypeDefinition;

        if (graphQLQueryType is null)
        {
            _ansiConsole.MarkupLine("[red]No query type found in the GraphQL schema. Skipping query operation filter selection.[/]");

            return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        var graphQLQueryOperations = graphQLQueryType.Fields!.Select(
            graphQLFieldDefinition => graphQLFieldDefinition.Name.StringValue
        );

        return _ansiConsole.Prompt(
            new MultiSelectionPrompt<string>()
                .PageSize(10)
                .Required(false)
                .Title("Which [green]query operations[/] would you like to test?")
                .MoreChoicesText("[grey](Move up and down to reveal more operations)[/]")
                .InstructionsText("[grey](Press [blue]<space>[/] to toggle an operation, [green]<enter>[/] to accept)[/]")
                .AddChoiceGroup("all", graphQLQueryOperations)
        ).ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    private ISet<string> PromptForMutationOperationFilter(IGraphQLDocumentAdapter graphQLDocumentAdapter, bool includeMutations)
    {
        if (!includeMutations)
        {
            return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        var graphQLMutationType = graphQLDocumentAdapter.GraphQLMutationTypeDefinition;

        if (graphQLMutationType is null)
        {
            _ansiConsole.MarkupLine("[red]No mutation type found in the GraphQL schema. Skipping mutation operation filter selection.[/]");

            return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        var graphQLMutationOperations = graphQLMutationType.Fields!.Select(
            graphQLFieldDefinition => graphQLFieldDefinition.Name.StringValue
        );

        return _ansiConsole.Prompt(
            new MultiSelectionPrompt<string>()
                .PageSize(10)
                .Required(false)
                .Title("Which [green]mutation operations[/] would you like to test?")
                .MoreChoicesText("[grey](Move up and down to reveal more operations)[/]")
                .InstructionsText("[grey](Press [blue]<space>[/] to toggle an operation, [green]<enter>[/] to accept)[/]")
                .AddChoiceGroup("all", graphQLMutationOperations)
        ).ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    private ISet<string> PromptForTypeFilter(IGraphQLDocumentAdapter graphQLDocumentAdapter)
    {
        var graphQLTypes = graphQLDocumentAdapter
            .GraphQLObjectTypeDefinitions
            .Select(graphQLTypeDefinition => graphQLTypeDefinition.Name.StringValue)
            .Concat(
                graphQLDocumentAdapter
                    .GraphQLInterfaceTypeDefinitions
                    .Select(graphQLInterfaceTypeDefinition => graphQLInterfaceTypeDefinition.Name.StringValue)
            )
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        // ReSharper disable once InvertIf - this is easier to read
        if (!graphQLTypes.Any())
        {
            _ansiConsole.MarkupLine("[red]No types found in the GraphQL schema. Skipping type filter selection.[/]");

            return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        return _ansiConsole.Prompt(
            new MultiSelectionPrompt<string>()
                .PageSize(10)
                .Required(false)
                .Title("Which [green]GraphQL types[/] would you like to generate Karate schemas for?")
                .MoreChoicesText("[grey](Move up and down to reveal more operations)[/]")
                .InstructionsText("[grey](Press [blue]<space>[/] to toggle an operation, [green]<enter>[/] to accept)[/]")
                .AddChoiceGroup("all", graphQLTypes)
        ).ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    private async Task<ICustomScalarMapping> PromptForCustomScalarMappingAsync()
    {
        var customScalarMappingSource = _ansiConsole.Prompt(
            new TextPrompt<string>("Enter the custom scalar mapping (raw mapping value(s) or path to file): ")
            {
                AllowEmpty = true,
            }.Validate(
                _customScalarMappingLoader.IsValid,
                "[red]Invalid custom scalar mapping.[/] Please provide either a raw mapping value(s) (e.g. \"DateTime:string,Long:number\" etc.) or a path to a custom scalar mapping file (containing JSON or comma-separated mappings)."
            )
        );

        return await _customScalarMappingLoader.LoadAsync(customScalarMappingSource).ConfigureAwait(false);
    }
}