using FluentAssertions;
using GraphQLToKarate.Library.Extensions;
using NUnit.Framework;

namespace GraphQLToKarate.Tests.Extensions;

[TestFixture]
public class CollectionExtensionsTests
{
    [Test, TestCaseSource(nameof(TestCases))]
    public void NoneOrContains_should_return_expected_result(
        ICollection<int> collection, 
        int item, 
        bool expected)
    {
        // Act
        var actual = collection.NoneOrContains(item);

        // Assert
        actual.Should().Be(expected);
    }

    private static IEnumerable<TestCaseData> TestCases
    {
        get
        {
            yield return new TestCaseData(Array.Empty<int>(), 1, true).SetName("Empty Collection");
            yield return new TestCaseData(new[] { 1, 2, 3 }, 1, true).SetName("Contains Item");
            yield return new TestCaseData(new[] { 1, 2, 3 }, 4, false).SetName("Does Not ContainItem");
        }
    }
}