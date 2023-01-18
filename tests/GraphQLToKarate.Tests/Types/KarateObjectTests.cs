using FluentAssertions;
using GraphQLToKarate.Library.Tokens;
using GraphQLToKarate.Library.Types;
using NUnit.Framework;

namespace GraphQLToKarate.Tests.Types;

[TestFixture]
internal sealed class KarateObjectTests
{
    [Test]
    [TestCaseSource(nameof(TestCases))]
    public void Schema(KarateObject karateObject, string expectedName, string expectedSchema)
    {
        karateObject.Name.Should().Be(expectedName);
        karateObject.ToString().Should().Be(expectedSchema);
    }

    private static IEnumerable<TestCaseData> TestCases
    {
        get
        {
            const string testKarateObjectName = "test";

            yield return new TestCaseData(
                new KarateObject(
                    testKarateObjectName,
                    new List<KarateTypeBase>
                    {
                        new KarateNonNullType(
                            new KarateListType(
                                new KarateNullType(
                                    new KarateType(KarateToken.String, "names")
                                )
                            )
                        )
                    }
                ),
                testKarateObjectName,
                """
                {
                  names: '#[] ##string'
                }
                """
            ).SetName("Single field is handled as expected.");

            yield return new TestCaseData(
                new KarateObject(
                    testKarateObjectName,
                    new List<KarateTypeBase>
                    {
                        new KarateNonNullType(new KarateType(KarateToken.Number, "id")),
                        new KarateNonNullType(new KarateType(KarateToken.String, "name")),
                        new KarateNonNullType(new KarateType(KarateToken.Boolean, "isFriendly")),
                        new KarateNullType(new KarateType(KarateToken.Object, "foo")),
                        new KarateNonNullType(
                            new KarateListType(
                                new KarateNonNullType(
                                    new KarateType("friendSchema", "friends")
                                )
                            )
                        )
                    }
                ),
                testKarateObjectName,
                """
                {
                  id: '#number',
                  name: '#string',
                  isFriendly: '#boolean',
                  foo: '##object',
                  friends: '#[] #friendSchema'
                }
                """
            ).SetName("Multiple fields are handled as expected.");
        }
    }
}