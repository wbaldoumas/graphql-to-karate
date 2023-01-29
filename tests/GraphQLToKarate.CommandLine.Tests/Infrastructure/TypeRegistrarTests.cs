using FluentAssertions;
using GraphQLToKarate.CommandLine.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Spectre.Console.Testing;

namespace GraphQLToKarate.CommandLine.Tests.Infrastructure;

[TestFixture]
internal sealed class TypeRegistrarTests
{
    private TypeRegistrarBaseTests? _tests;

    [SetUp]
    public void SetUp() => _tests = new TypeRegistrarBaseTests(TypeRegistrarConfigurator.ConfigureTypeRegistrar);

    [Test]
    public void Test() => _tests!.RunAllTests();


    [Test]
    public void RegisterLazy_registers_type_as_expected()
    {
        // arrange
        var serviceCollection = new ServiceCollection();
        var subjectUnderTest = new TypeRegistrar(serviceCollection);

        // act
        subjectUnderTest.RegisterLazy(typeof(IServiceCollection), () => serviceCollection);

        // assert
        subjectUnderTest.Build().Resolve(typeof(IServiceCollection)).Should().BeEquivalentTo(serviceCollection);
    }

    [Test]
    public void RegisterLazy_throws_exception_when_registration_func_is_null()
    {
        // arrange
        var serviceCollection = new ServiceCollection();
        var subjectUnderTest = new TypeRegistrar(serviceCollection);

        // act
        var act = () => subjectUnderTest.RegisterLazy(typeof(IServiceCollection), null);

        // assert
        act.Should().Throw<ArgumentNullException>();
    }
}