using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Utilities.Shared.Logging.Policies;
using Utilities.Shared.Logging.Utils;

namespace Utilities.Shared.Tests.Logging;

public class ContentBasedEnricherTests
{
    [Fact]
    public async Task EnrichRequestAsync_Should_Add_Service_TraceId_And_UserId()
    {
        var context = new DefaultHttpContext
        {
            TraceIdentifier = "trace-123",
            User = new ClaimsPrincipal(new ClaimsIdentity([
                new Claim("sub", "user-42")
            ], "TestAuth"))
        };

        var model = new RequestLogModel();
        var sut = new ContentBasedEnricher("CustomersService");

        await sut.EnrichRequestAsync(context, model);

        model.TraceId.Should().Be("trace-123");
        model.Additional["service"].Should().Be("CustomersService");
        model.Additional["user.id"].Should().Be("user-42");
        model.Additional["env"].Should().Be("CustomersService");
    }

    [Fact]
    public async Task EnrichResponseAsync_Should_Add_Service_And_TraceId()
    {
        var context = new DefaultHttpContext
        {
            TraceIdentifier = "trace-456"
        };
        var model = new ResponseLogModel();
        var sut = new ContentBasedEnricher("OrdersService");

        await sut.EnrichResponseAsync(context, model);

        model.TraceId.Should().Be("trace-456");
        model.Additional["service"].Should().Be("OrdersService");
    }

    [Fact]
    public async Task EnrichRequestAsync_With_HttpRequestMessage_Should_Copy_XRequestId()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.test.com");
        request.Headers.Add("X-Request-Id", "req-999");

        var model = new RequestLogModel();
        var sut = new ContentBasedEnricher("PaymentsService");

        await sut.EnrichRequestAsync(request, model);

        model.TraceId.Should().Be("req-999");
        model.Additional["service"].Should().Be("PaymentsService");
    }
}