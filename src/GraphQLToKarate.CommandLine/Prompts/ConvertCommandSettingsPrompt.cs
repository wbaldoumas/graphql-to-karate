using GraphQLToKarate.CommandLine.Settings;
using GraphQLToKarate.Library.Adapters;
using GraphQLToKarate.Library.Mappings;
using GraphQLToKarate.Library.Parsers;
using GraphQLToKarate.Library.Settings;
using Spectre.Console;
using System.Diagnostics.CodeAnalysis;

namespace GraphQLToKarate.CommandLine.Prompts;

/// <inheritdoc cref="IConvertCommandSettingsPrompt"/>
[ExcludeFromCodeCoverage]
internal class ConvertCommandSettingsPrompt : IConvertCommandSettingsPrompt
{
    private const string MoreChoicesText = "[grey](Move up and down to reveal more operations)[/]";
    private const string InstructionsText = $"[grey](Press [{InstructionsColorRgb}]<space>[/] to toggle an operation, [{InfoColorRgb}]<enter>[/] to accept)[/]";

    private const string InfoColorRgb = "rgb(97,214,214)";
    private const string WarnColorRgb = "rgb(249,241,165)";
    private const string ErrorColorRgb = "rgb(231,72,86)";
    private const string InstructionsColorRgb = "rgb(58,150,221)";
    private const string OptionalColor = "grey";
    private const string OptionalPrompt = $"[{OptionalColor}][[optional]][/]";

    private static readonly Color InfoColor = new(97, 214, 214);
    private static readonly Style InfoStyle = new(InfoColor);
    private static readonly Style InstructionsStyle = new(new Color(58, 150, 221));

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
        _ansiConsole.Write(
            new Panel(
                new FigletText("graphql-to-karate").Color(InfoColor)
            ).RoundedBorder().DoubleBorder().BorderColor(InfoColor)
        );

        _ansiConsole.WriteLine();

        if (!_ansiConsole.Profile.Capabilities.Interactive)
        {
            _ansiConsole.MarkupLine($"[{ErrorColorRgb}]Current environment does not support interaction.[/] Continuing with default settings. Use a console which supports interaction or use the [{InstructionsColorRgb}]--interactive=false[/] option to specify settings.");
            _ansiConsole.Write(new Padder(new Rule { Style = InfoStyle }).PadLeft(1).PadRight(1));

            return initialLoadedConvertCommandSettings;
        }

        void Newline() => _ansiConsole.WriteLine();

        var outputFile = PromptForOutputFile(initialLoadedConvertCommandSettings);
        Newline();

        var queryName = PromptForQueryName(initialLoadedConvertCommandSettings);
        Newline();

        var mutationName = PromptForMutationName(initialLoadedConvertCommandSettings);
        Newline();

        var excludeQueries = PromptForExcludeQueriesChoice(initialLoadedConvertCommandSettings);
        Newline();

        var includeMutations = PromptForIncludeMutationsChoice(initialLoadedConvertCommandSettings);
        Newline();

        var baseUrl = PromptForBaseUrl(initialLoadedConvertCommandSettings);
        Newline();

        var customScalarMapping = await PromptForCustomScalarMappingAsync().ConfigureAwait(false);
        Newline();

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

        var loadedConvertCommandSettings = new LoadedConvertCommandSettings
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

        WriteCapturedOptions(loadedConvertCommandSettings);

        return loadedConvertCommandSettings;
    }

    private void WriteCapturedOptions(LoadedConvertCommandSettings loadedConvertCommandSettings)
    {
        var table = new Table()
            .Title("[bold underline]Captured Options[/]")
            .Border(TableBorder.DoubleEdge)
            .BorderColor(InfoColor)
            .HideHeaders()
            .AddColumn(new TableColumn("[bold underline]Option[/]"))
            .AddColumn(new TableColumn("[bold underline]Value[/]"))
            .AddRow($"[{InstructionsColorRgb}]--output-file[/]", loadedConvertCommandSettings.OutputFile)
            .AddRow($"[{InstructionsColorRgb}]--query-name[/]", loadedConvertCommandSettings.QueryName)
            .AddRow($"[{InstructionsColorRgb}]--mutation-name[/]", loadedConvertCommandSettings.MutationName)
            .AddRow($"[{InstructionsColorRgb}]--exclude-queries[/]", loadedConvertCommandSettings.ExcludeQueries.ToString())
            .AddRow($"[{InstructionsColorRgb}]--include-mutations[/]", loadedConvertCommandSettings.IncludeMutations.ToString())
            .AddRow($"[{InstructionsColorRgb}]--base-url[/]", loadedConvertCommandSettings.BaseUrl);

        if (loadedConvertCommandSettings.CustomScalarMapping.Any())
        {
            table.AddRow($"[{InstructionsColorRgb}]--custom-scalar-mapping[/]", string.Join(',', loadedConvertCommandSettings.CustomScalarMapping));
        }

        if (loadedConvertCommandSettings.QueryOperationFilter.Any())
        {
            table.AddRow($"[{InstructionsColorRgb}]--query-operation-filter[/]", string.Join(',', loadedConvertCommandSettings.QueryOperationFilter));
        }

        if (loadedConvertCommandSettings.MutationOperationFilter.Any())
        {
            table.AddRow($"[{InstructionsColorRgb}]--mutation-operation-filter[/]", string.Join(',', loadedConvertCommandSettings.MutationOperationFilter));
        }

        if (loadedConvertCommandSettings.TypeFilter.Any())
        {
            table.AddRow($"[{InstructionsColorRgb}]--type-filter[/]", string.Join(',', loadedConvertCommandSettings.TypeFilter));
        }

        _ansiConsole.Write(new Padder(table).PadLeft(1).PadRight(1));
    }

    private string PromptForOutputFile(LoadedConvertCommandSettings initialLoadedConvertCommandSettings) =>
        _ansiConsole.Prompt(
            new TextPrompt<string>($"Enter the [{InstructionsColorRgb}]path and file name[/] for the generated Karate feature file:")
            {
                AllowEmpty = false,
                DefaultValueStyle = InfoStyle
            }.DefaultValue(initialLoadedConvertCommandSettings.OutputFile)
        );

    private string PromptForQueryName(LoadedConvertCommandSettings initialLoadedConvertCommandSettings) =>
        _ansiConsole.Prompt(
            new TextPrompt<string>($"Enter the name of the [{InstructionsColorRgb}]GraphQL query type[/]:")
            {
                AllowEmpty = false,
                DefaultValueStyle = InfoStyle
            }.DefaultValue(initialLoadedConvertCommandSettings.QueryName)
        );

    private string PromptForMutationName(LoadedConvertCommandSettings initialLoadedConvertCommandSettings) =>
        _ansiConsole.Prompt(
            new TextPrompt<string>($"Enter the name of the [{InstructionsColorRgb}]GraphQL mutation type[/]:")
            {
                AllowEmpty = false,
                DefaultValueStyle = InfoStyle
            }.DefaultValue(initialLoadedConvertCommandSettings.MutationName)
        );

    private bool PromptForExcludeQueriesChoice(LoadedConvertCommandSettings initialLoadedConvertCommandSettings) =>
        _ansiConsole.Prompt(
            new ConfirmationPromptWithStyling("Exclude queries from the generated Karate feature?")
            {
                DefaultValue = initialLoadedConvertCommandSettings.ExcludeQueries,
                DefaultValueStyle = InfoStyle,
                ChoicesStyle = InstructionsStyle
            }
        );

    private bool PromptForIncludeMutationsChoice(LoadedConvertCommandSettings initialLoadedConvertCommandSettings) =>
        _ansiConsole.Prompt(
            new ConfirmationPromptWithStyling("Include mutations in the generated Karate feature?")
            {
                DefaultValue = initialLoadedConvertCommandSettings.IncludeMutations,
                DefaultValueStyle = InfoStyle,
                ChoicesStyle = InstructionsStyle
            }
        );

    private string PromptForBaseUrl(LoadedConvertCommandSettings initialLoadedConvertCommandSettings) =>
        _ansiConsole.Prompt(
            new TextPrompt<string>($"Enter the [{InstructionsColorRgb}]base URL[/] to be used in the Karate feature:")
            {
                AllowEmpty = false,
                DefaultValueStyle = InfoStyle,
                ChoicesStyle = InstructionsStyle
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
            _ansiConsole.MarkupLine($"[{WarnColorRgb}]No query type found in the GraphQL schema. Skipping query operation filter selection.[/]{Environment.NewLine}");

            return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        var graphQLQueryOperations = graphQLQueryType.Fields!.Select(
            graphQLFieldDefinition => graphQLFieldDefinition.Name.StringValue
        );

        return _ansiConsole.Prompt(
            new MultiSelectionPrompt<string>()
                .PageSize(10)
                .Required(false)
                .HighlightStyle(InstructionsStyle)
                .Title($"Which [{InstructionsColorRgb}]query operations[/] would you like to test? {OptionalPrompt}")
                .MoreChoicesText(MoreChoicesText)
                .InstructionsText(InstructionsText)
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
            _ansiConsole.MarkupLine($"[{WarnColorRgb}]No mutation type found in the GraphQL schema. Skipping mutation operation filter selection.[/]{Environment.NewLine}");

            return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        var graphQLMutationOperations = graphQLMutationType.Fields!.Select(
            graphQLFieldDefinition => graphQLFieldDefinition.Name.StringValue
        );

        return _ansiConsole.Prompt(
            new MultiSelectionPrompt<string>()
                .PageSize(10)
                .Required(false)
                .HighlightStyle(InstructionsStyle)
                .Title($"Which [{InstructionsColorRgb}]mutation operations[/] would you like to test? {OptionalPrompt}")
                .MoreChoicesText(MoreChoicesText)
                .InstructionsText(InstructionsText)
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
            _ansiConsole.MarkupLine($"[{WarnColorRgb}]No types found in the GraphQL schema. Skipping type filter selection.[/]");

            return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        return _ansiConsole.Prompt(
            new MultiSelectionPrompt<string>()
                .PageSize(10)
                .Required(false)
                .HighlightStyle(InstructionsStyle)
                .Title($"Which [{InstructionsColorRgb}]GraphQL types[/] would you like to generate Karate schemas for? {OptionalPrompt}")
                .MoreChoicesText(MoreChoicesText)
                .InstructionsText(InstructionsText)
                .AddChoiceGroup("all", graphQLTypes)
        ).ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    private async Task<ICustomScalarMapping> PromptForCustomScalarMappingAsync()
    {
        var customScalarMappingSource = _ansiConsole.Prompt(
            new TextPrompt<string>($"Enter the [{InstructionsColorRgb}]custom scalar mapping[/] (raw mapping value or path to file) {OptionalPrompt}:")
            {
                AllowEmpty = true,
                DefaultValueStyle = InfoStyle
            }.Validate(
                _customScalarMappingLoader.IsValid,
                $"{Environment.NewLine}[{ErrorColorRgb}]Invalid custom scalar mapping.[/] Please provide either a raw mapping value (e.g. [{InstructionsColorRgb}]\"DateTime:string,Long:number\"[/] etc.) or a [{InstructionsColorRgb}]path to a custom scalar mapping file[/] (containing a JSON or comma-separated mapping).{Environment.NewLine}"
            )
        );

        return await _customScalarMappingLoader.LoadAsync(customScalarMappingSource).ConfigureAwait(false);
    }
}