﻿using GraphQLParser.AST;
using GraphQLToKarate.Library.Types;

namespace GraphQLToKarate.Library.Converters;

/// <summary>
///     Converts <see cref="GraphQLObjectTypeDefinition"/> instances to <see cref="KarateObject"/> instances.
/// </summary>
public interface IGraphQLObjectTypeDefinitionConverter
{
    /// <summary>
    ///     Convert the given <see cref="GraphQLObjectTypeDefinition"/> to a <see cref="KarateObject"/>.
    /// </summary>
    /// <param name="graphQLObjectTypeDefinition">The GraphQL object to convert.</param>
    /// <param name="graphQLUserDefinedTypes">
    ///     User-defined GraphQL types, needed for handling nested types, enums, etc. as fields on the
    ///     GraphQL object being converted.
    /// </param>
    /// <returns></returns>
    KarateObject Convert(
        GraphQLObjectTypeDefinition graphQLObjectTypeDefinition, 
        GraphQLUserDefinedTypes graphQLUserDefinedTypes
    );
}