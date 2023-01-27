using GraphQLToKarate.CommandLine.Infrastructure;
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
}