using FluentAssertions;
using GraphQLParser.AST;
using GraphQLToKarate.Library.Adapters;
using GraphQLToKarate.Library.Converters;
using GraphQLToKarate.Library.Extensions;
using GraphQLToKarate.Library.Tokens;
using GraphQLToKarate.Library.Types;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace GraphQLToKarate.Tests.Converters;

[TestFixture]
internal sealed class GraphQLCustomScalarTypeConverterTests
{
    private IGraphQLTypeConverter? _mockGraphQLTypeConverter;
    private IDictionary<string, string>? _customScalarMapping;
    private IGraphQLTypeConverter? _subjectUnderTest;

    private const string CustomScalarNameLong = "Long";
    private const string CustomScalarNameTime = "Time";

    [SetUp]
    public void SetUp()
    {
        _mockGraphQLTypeConverter = Substitute.For<IGraphQLTypeConverter>();

        _customScalarMapping = new Dictionary<string, string>
        {
            { CustomScalarNameLong, KarateToken.Number },
            { CustomScalarNameTime, KarateToken.String },
        };

        _subjectUnderTest = new GraphQLCustomScalarTypeConverter(
            _customScalarMapping!,
            _mockGraphQLTypeConverter!
        );
    }

    [Test]
    [TestCaseSource(nameof(TestCases))]
    public void Convert(
        string graphQLFieldName,
        GraphQLType graphQLType,
        IGraphQLDocumentAdapter graphQLDocumentAdapter,
        KarateTypeBase expectedKarateType)
    {
        // arrange
        var shouldCallUnderlyingGraphQLTypeConverter = !_customScalarMapping!.ContainsKey(graphQLType.GetTypeName());

        if (shouldCallUnderlyingGraphQLTypeConverter)
        {
            _mockGraphQLTypeConverter!
                .Convert(graphQLFieldName, graphQLType, graphQLDocumentAdapter)
                .Returns(expectedKarateType);
        }
        else
        {
            _mockGraphQLTypeConverter!
                .Convert(graphQLFieldName, graphQLType, graphQLDocumentAdapter)
                .Throws(new InvalidOperationException());
        }

        var karateType = _subjectUnderTest!.Convert(
            graphQLFieldName,
            graphQLType,
            graphQLDocumentAdapter
        );

        // assert
        karateType.Should().BeEquivalentTo(expectedKarateType);

        if (shouldCallUnderlyingGraphQLTypeConverter)
        {
            _mockGraphQLTypeConverter!
                .Received()
                .Convert(graphQLFieldName, graphQLType, graphQLDocumentAdapter);
        }
        else
        {
            _mockGraphQLTypeConverter!
                .DidNotReceiveWithAnyArgs()
                .Convert(Arg.Any<string>(), Arg.Any<GraphQLType>(), Arg.Any<IGraphQLDocumentAdapter>());
        }
    }

    private static IEnumerable<TestCaseData> TestCases
    {
        get
        {
            const string testFieldName = "Test";

            var emptyGraphQLDocumentAdapter = new GraphQLDocumentAdapter(new GraphQLDocument());

            yield return new TestCaseData(
                testFieldName,
                new GraphQLNamedType
                {
                    Name = new GraphQLName(CustomScalarNameLong)
                },
                emptyGraphQLDocumentAdapter,
                new KarateType(KarateToken.Number, testFieldName)
            ).SetName("Custom scalar present in the mapping returns expected mapped Karate type.");

            yield return new TestCaseData(
                testFieldName,
                new GraphQLNamedType
                {
                    Name = new GraphQLName(CustomScalarNameTime)
                },
                emptyGraphQLDocumentAdapter,
                new KarateType(KarateToken.String, testFieldName)
            ).SetName("Other custom scalar present in the mapping returns expected mapped Karate type.");

            yield return new TestCaseData(
                testFieldName,
                new GraphQLNamedType
                {
                    Name = new GraphQLName("SomeFunnyType")
                },
                emptyGraphQLDocumentAdapter,
                new KarateType(KarateToken.Present, testFieldName)
            ).SetName("Custom type not present in the mapping returns expected Karate type from underlying GraphQLTypeConverter.");
        }
    }
}