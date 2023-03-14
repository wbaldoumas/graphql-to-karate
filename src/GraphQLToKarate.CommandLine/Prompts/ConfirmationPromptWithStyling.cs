using Spectre.Console;
using System.Diagnostics.CodeAnalysis;

namespace GraphQLToKarate.CommandLine.Prompts;

/// <summary>
/// A prompt that is answered with a yes or no. With styling!
/// </summary>
[ExcludeFromCodeCoverage(Justification = "This is just temporary until the Spectre.Console library supports it.")]
public sealed class ConfirmationPromptWithStyling : IPrompt<bool>
{
    private readonly string _prompt;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfirmationPromptWithStyling"/> class.
    /// </summary>
    /// <param name="prompt">The prompt markup text.</param>
    public ConfirmationPromptWithStyling(string prompt)
    {
        _prompt = prompt ?? throw new ArgumentNullException(nameof(prompt));
    }

    /// <summary>
    /// Gets or sets the character that represents "yes".
    /// </summary>
    public char Yes { get; set; } = 'y';

    /// <summary>
    /// Gets or sets the character that represents "no".
    /// </summary>
    public char No { get; set; } = 'n';

    /// <summary>
    /// Gets or sets a value indicating whether "yes" is the default answer.
    /// </summary>
    public bool DefaultValue { get; set; } = true;

    /// <summary>
    /// Gets or sets the message for invalid choices.
    /// </summary>
    public string InvalidChoiceMessage { get; set; } = "[red]Please select one of the available options[/]";

    /// <summary>
    /// Gets or sets a value indicating whether or not
    /// choices should be shown.
    /// </summary>
    public bool ShowChoices { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether or not
    /// default values should be shown.
    /// </summary>
    public bool ShowDefaultValue { get; set; } = true;

    /// <summary>
    /// Gets or sets the style in which the default value is displayed.
    /// </summary>
    public Style? DefaultValueStyle { get; set; }

    /// <summary>
    /// Gets or sets the style in which the list of choices is displayed.
    /// </summary>
    public Style? ChoicesStyle { get; set; }

    /// <inheritdoc/>
    public bool Show(IAnsiConsole console)
    {
        return ShowAsync(console, CancellationToken.None).GetAwaiter().GetResult();
    }

    /// <inheritdoc/>
    public async Task<bool> ShowAsync(IAnsiConsole console, CancellationToken cancellationToken)
    {
        var prompt = new TextPrompt<char>(_prompt)
            .InvalidChoiceMessage(InvalidChoiceMessage)
            .ValidationErrorMessage(InvalidChoiceMessage)
            .ShowChoices(ShowChoices)
            .ShowDefaultValue(ShowDefaultValue)
            .DefaultValueStyle(DefaultValueStyle ?? "green")
            .DefaultValue(DefaultValue ? Yes : No)
            .ChoicesStyle(ChoicesStyle ?? "blue")
            .AddChoice(Yes)
            .AddChoice(No);

        var result = await prompt.ShowAsync(console, cancellationToken).ConfigureAwait(false);

        return result == Yes;
    }
}