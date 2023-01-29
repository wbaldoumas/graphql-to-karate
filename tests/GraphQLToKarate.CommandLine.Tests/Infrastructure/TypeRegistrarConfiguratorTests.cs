using FluentAssertions;
using GraphQLToKarate.CommandLine.Infrastructure;
using GraphQLToKarate.CommandLine.Settings;
using NUnit.Framework;

namespace GraphQLToKarate.CommandLine.Tests.Infrastructure;

[TestFixture]
internal sealed class TypeRegistrarConfiguratorTests
{
    [Test]
    public void TypeRegistrarConfigurator_configures_expected_TypeRegistrar()
    {
        // arrange + act
        var typeRegistrar = TypeRegistrarConfigurator.ConfigureTypeRegistrar();
        var typeResolver = typeRegistrar.Build();

        // assert
        typeResolver
            .Should()
            .NotBeNull();

        typeResolver
            .Resolve(typeof(ConvertCommandSettings))
            .Should()
            .BeOfType<ConvertCommandSettings>();
    }
}