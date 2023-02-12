using FluentAssertions;
using GraphQLToKarate.CommandLine.Infrastructure;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using Spectre.Console.Testing;

namespace GraphQLToKarate.CommandLine.Tests.Infrastructure;

[TestFixture]
internal sealed class TypeRegistrarTests
{
    private TypeRegistrarBaseTests? _tests;

    [SetUp]
    public void SetUp() => _tests = new TypeRegistrarBaseTests(() => TypeRegistrarConfigurator.ConfigureTypeRegistrar(Array.Empty<string>()));

    [Test]
    public void Test() => _tests!.RunAllTests();


    [Test]
    public void RegisterLazy_registers_type_as_expected()
    {
        // arrange
        var subjectUnderTest = new TypeRegistrar(HostConfigurator.ConfigureHostBuilder(Array.Empty<string>()));

        // act
        subjectUnderTest.RegisterLazy(typeof(IHostBuilder), () => HostConfigurator.ConfigureHostBuilder(Array.Empty<string>()));

        // assert
        subjectUnderTest.Build().Resolve(typeof(IHostBuilder)).Should().BeEquivalentTo(HostConfigurator.ConfigureHostBuilder(Array.Empty<string>()));
    }

    [Test]
    public void RegisterLazy_throws_exception_when_registration_func_is_null()
    {
        // arrange
        var hostBuilder = HostConfigurator.ConfigureHostBuilder(Array.Empty<string>());
        var subjectUnderTest = new TypeRegistrar(hostBuilder);

        // act
        var act = () => subjectUnderTest.RegisterLazy(typeof(IHostBuilder), null);

        // assert
        act.Should().Throw<ArgumentNullException>();
    }
}