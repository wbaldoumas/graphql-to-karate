namespace GraphQLToKarate.Integration.Api.Types;

public sealed class PageInfoInput
{
    public int? Limit { get; set; }

    public int? Offset { get; set; }
}