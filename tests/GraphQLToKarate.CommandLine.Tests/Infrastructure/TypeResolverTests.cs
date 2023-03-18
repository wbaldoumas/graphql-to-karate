using FluentAssertions;
using GraphQLToKarate.CommandLine.Infrastructure;
using Microsoft.Extensions.Hosting;
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
        var mockHost = Substitute.For<IHost>();
        var mockServiceProvider = Substitute.For<IServiceProvider>();

        mockHost.Services.Returns(mockServiceProvider);
        mockServiceProvider.GetService(Arg.Any<Type>()).Returns(expectedResolvedObject);

        var subjectUnderTest = new TypeResolver(mockHost);

        // act
        var resolvedObject = subjectUnderTest.Resolve(requestedType);

        // assert
        resolvedObject.Should().BeEquivalentTo(expectedResolvedObject, "because the service provider returned it.");
    }

    [Test]
    public void TypeResolver_throws_exception_when_null_ServiceProvider_passed()
    {
        // arrange + act
        var act = () => new TypeResolver(null);

        // assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void TypeResolver_properly_disposes_service_provider_when_it_is_disposable()
    {
        // arrange
        var host = Substitute.For<IHost, IDisposable>();
        var subjectUnderTest = new TypeResolver(host);

        // act
        subjectUnderTest.Dispose();

        // assert
        (host as IDisposable)!.Received().Dispose();
    }
}