namespace GraphQLToKarate.Integration.Api.Types;

public sealed class PageInfo
{
    public bool HasNextPage { get; set; }

    public bool HasPreviousPage { get; set; }

    public int TotalCount { get; set; }
}