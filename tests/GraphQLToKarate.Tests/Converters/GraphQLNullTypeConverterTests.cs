﻿using FluentAssertions;
using GraphQLParser.AST;
using GraphQLToKarate.Library.Converters;
using GraphQLToKarate.Library.Extensions;
using GraphQLToKarate.Library.Tokens;
using GraphQLToKarate.Library.Types;
using NUnit.Framework;

namespace GraphQLToKarate.Tests.Converters;

[TestFixture]
internal sealed class GraphQLNullTypeConverterTests
{
    private IGraphQLTypeConverter? _subjectUnderTest;

    [SetUp]
    public void SetUp()
    {
        var graphQLTypeConverterFactory = new GraphQLTypeConverterFactory();

        _subjectUnderTest = new GraphQLNullTypeConverter(graphQLTypeConverterFactory);
    }

    [TestCaseSource(nameof(TestCases))]
    public void Convert(
        string graphQLFieldName,
        GraphQLType graphQLType,
        GraphQLUserDefinedTypes graphQLUserDefinedTypes,
        KarateTypeBase expectedKarateType)
    {
        // act
        var karateType = _subjectUnderTest!.Convert(
            graphQLFieldName,
            graphQLType,
            graphQLUserDefinedTypes
        );

        // assert
        karateType.Should().BeEquivalentTo(expectedKarateType);
    }

    private static IEnumerable<TestCaseData> TestCases
    {
        get
        {
            const string testFieldName = "Test";

            var emptyGraphQLUserDefinedTypes = new GraphQLUserDefinedTypes
            {
                GraphQLEnumTypeDefinitionsByName = new Dictionary<string, GraphQLEnumTypeDefinition>(),
                GraphQLObjectTypeDefinitionsByName = new Dictionary<string, GraphQLObjectTypeDefinition>()
            };

            yield return new TestCaseData(
                testFieldName,
                new GraphQLNamedType
                {
                    Name = new GraphQLName(GraphQLToken.Boolean)
                },
                emptyGraphQLUserDefinedTypes,
                new KarateNullType(new KarateType(KarateToken.Boolean, testFieldName))
            ).SetName("Nullable Boolean GraphQL type is converted to nullable boolean Karate type.");

            yield return new TestCaseData(
                testFieldName,
                new GraphQLNamedType
                {
                    Name = new GraphQLName(GraphQLToken.Float)
                },
                emptyGraphQLUserDefinedTypes,
                new KarateNullType(new KarateType(KarateToken.Number, testFieldName))
            ).SetName("Nullable Float GraphQL type is converted to nullable number Karate type.");


            yield return new TestCaseData(
                testFieldName,
                new GraphQLNamedType
                {
                    Name = new GraphQLName(GraphQLToken.Int)
                },
                emptyGraphQLUserDefinedTypes,
                new KarateNullType(new KarateType(KarateToken.Number, testFieldName))
            ).SetName("Nullable Int GraphQL type is converted to nullable number Karate type.");

            yield return new TestCaseData(
                testFieldName,
                new GraphQLNamedType
                {
                    Name = new GraphQLName(GraphQLToken.String)
                },
                emptyGraphQLUserDefinedTypes,
                new KarateNullType(new KarateType(KarateToken.String, testFieldName))
            ).SetName("Nullable String GraphQL type is converted to nullable string Karate type.");

            yield return new TestCaseData(
                testFieldName,
                new GraphQLNamedType
                {
                    Name = new GraphQLName(GraphQLToken.Id)
                },
                emptyGraphQLUserDefinedTypes,
                new KarateNullType(new KarateType(KarateToken.String, testFieldName))
            ).SetName("Nullable ID GraphQL type is converted to nullable string Karate type.");

            const string enumTypeName = "Color";

            yield return new TestCaseData(
                testFieldName,
                new GraphQLNamedType
                {
                    Name = new GraphQLName(enumTypeName)
                },
                new GraphQLUserDefinedTypes
                {
                    GraphQLEnumTypeDefinitionsByName = new Dictionary<string, GraphQLEnumTypeDefinition>
                    {
                        { enumTypeName, new GraphQLEnumTypeDefinition() }
                    },
                    GraphQLObjectTypeDefinitionsByName = new Dictionary<string, GraphQLObjectTypeDefinition>()
                },
                new KarateNullType(new KarateType(KarateToken.String, testFieldName))
            ).SetName("Nullable enum GraphQL type is converted to nullable string Karate type.");

            const string customTypeName = "ToDo";

            yield return new TestCaseData(
                testFieldName,
                new GraphQLNamedType
                {
                    Name = new GraphQLName(customTypeName)
                },
                new GraphQLUserDefinedTypes
                {
                    GraphQLEnumTypeDefinitionsByName = new Dictionary<string, GraphQLEnumTypeDefinition>
                    {
                        { enumTypeName, new GraphQLEnumTypeDefinition() }
                    },
                    GraphQLObjectTypeDefinitionsByName = new Dictionary<string, GraphQLObjectTypeDefinition>
                    {
                        { customTypeName, new GraphQLObjectTypeDefinition() }
                    }
                },
                new KarateNullType(new KarateType($"{customTypeName.FirstCharToLower()}Schema", testFieldName))
            ).SetName("Nullable custom GraphQL type is converted to nullable custom Karate type.");

            yield return new TestCaseData(
                testFieldName,
                new GraphQLListType
                {
                    Type = new GraphQLNamedType
                    {
                        Name = new GraphQLName(GraphQLToken.Boolean)
                    }
                },
                emptyGraphQLUserDefinedTypes,
                new KarateNullType(
                    new KarateListType(
                        new KarateNullType(
                            new KarateType(KarateToken.Boolean, testFieldName)
                        )
                    )
                )
            ).SetName("Nullable list of nullable booleans is converted to nullable list karate type.");
        }
    }
}