using System.Net;
using FluentAssertions;
using Utilities.Shared.Logging.Interfaces;
using Utilities.Shared.Logging.Policies;

namespace Utilities.Shared.Tests.Logging;

public class DefaultRedactionPolicyTests
{
    [Theory]
    [InlineData("Authorization", true)]
    [InlineData("X-Api-Key", true)]
    [InlineData("Cookie", true)]
    [InlineData("Random-Header", false)]
    public void ShouldRedactHeader_Should_Work_As_Expected(string headerName, bool expected)
    {
        var sut = new DefaultRedactionPolicy();

        var result = sut.ShouldRedactHeader(headerName);

        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("application/json", "GET", false)]
    [InlineData("application/octet-stream", "POST", true)]
    [InlineData("multipart/form-data", "POST", true)]
    [InlineData("application/json", "PATCH", true)]
    public void ShouldSuppressRequestBody_Should_Respect_Rules(string contentType, string method, bool expected)
    {
        var sut = new DefaultRedactionPolicy();

        var result = sut.ShouldSuppressRequestBody(contentType, method);

        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("application/json", 200, false)]
    [InlineData("application/octet-stream", 200, true)]
    [InlineData("multipart/form-data", 200, true)]
    public void ShouldSuppressResponseBody_Should_Respect_Rules(string contentType, int statusCode, bool expected)
    {
        var sut = new DefaultRedactionPolicy();

        var result = sut.ShouldSuppressResponseBody(contentType, statusCode);

        result.Should().Be(expected);
    }

    [Fact]
    public void ShouldSuppressRequestBody_With_HttpRequestMessage()
    {
        var sut = new DefaultRedactionPolicy();
        var request = new HttpRequestMessage(HttpMethod.Patch, "https://test.com");
        request.Content = new StringContent("test", null, "application/json");

        var result = ((ILogPolicy)sut).ShouldSuppressRequestBody(request);

        result.Should().BeTrue();
    }

    [Fact]
    public void ShouldSuppressResponseBody_With_HttpResponseMessage()
    {
        var sut = new DefaultRedactionPolicy();
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("test", null, "application/octet-stream")
        };

        var result = ((ILogPolicy)sut).ShouldSuppressResponseBody(response);

        result.Should().BeTrue();
    }
}