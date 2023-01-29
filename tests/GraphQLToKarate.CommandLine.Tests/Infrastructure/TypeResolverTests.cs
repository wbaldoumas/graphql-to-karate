using FluentAssertions;
using GraphQLToKarate.CommandLine.Infrastructure;
using NSubstitute;
using NUnit.Framework;

namespace GraphQLToKarate.CommandLine.Tests.Infrastructure;

[TestFixture]
internal sealed class TypeResolverTests
{
    [Test]
    [TestCase(typeof(int), null)]
    [TestCase(null, null)]
    public void TypeResolver_resolves_expected_type(Type? requestedType, object? expectedResolvedObject)
    {
        // arrange
        var mockServiceProvider = Substitute.For<IServiceProvider>();

        mockServiceProvider.GetService(Arg.Any<Type>()).Returns(expectedResolvedObject);

        var subjectUnderTest = new TypeResolver(mockServiceProvider);

        // act
        var resolvedObject = subjectUnderTest.Resolve(requestedType);

        // assert
        resolvedObject.Should().BeEquivalentTo(expectedResolvedObject, "because the service provider returned it.");
    }

    [Test]
    public void TypeResolver_properly_disposes_service_provider_when_it_is_disposable()
    {
        // arrange
        var serviceProvider = Substitute.For<IServiceProvider, IDisposable>();
        var subjectUnderTest = new TypeResolver(serviceProvider);

        // act
        subjectUnderTest.Dispose();

        // assert
        (serviceProvider as IDisposable)!.Received().Dispose();
    }

    [Test]
    public void TypeResolver_properly_does_not_dispose_service_provider_when_it_is_not_disposable()
    {
        // arrange
        var mockServiceProvider = Substitute.For<IServiceProvider>();
        var subjectUnderTest = new TypeResolver(mockServiceProvider);

        // act
        subjectUnderTest.Dispose();

        // assert
        (mockServiceProvider as IDisposable)?.DidNotReceiveWithAnyArgs().Dispose();
    }

    [Test]
    public void TypeResolver_throws_exception_when_null_ServiceProvider_passed()
    {
        // arrange + act
        var act = () => new TypeResolver(null);

        // assert
        act.Should().Throw<ArgumentNullException>();
    }
}