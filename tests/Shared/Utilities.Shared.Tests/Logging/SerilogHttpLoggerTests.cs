using FluentAssertions;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.InMemory;
using Utilities.Shared.Logging.Config;
using Utilities.Shared.Logging.Utils;

namespace Utilities.Shared.Tests.Logging;

public class SerilogHttpLoggerTests
{
    private readonly SerilogHttpLogger _sut;

    public SerilogHttpLoggerTests()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.InMemory()
            .CreateLogger();

        _sut = new SerilogHttpLogger();
    }

    [Fact]
    public async Task LogRequestAsync_Should_Log_Request_Info_And_Body()
    {
        // Arrange
        var request = new RequestLogModel
        {
            Method = "POST",
            Path = "/api/customers",
            Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }!,
            Body = "{ \"id\": 1 }"
        };

        // Act
        await _sut.LogRequestAsync(request);

        // Assert
        var events = InMemorySink.Instance.LogEvents.ToList();

        events.Should().ContainSingle(e =>
            e.Level == LogEventLevel.Information &&
            e.MessageTemplate.Text.Contains("HTTP Request {Method} {Url}"));

        events.Should().ContainSingle(e =>
            e.Level == LogEventLevel.Debug &&
            e.MessageTemplate.Text.Contains("HTTP Request Body: {Body}"));
    }

    [Fact]
    public async Task LogResponseAsync_Should_Log_Response_Info_And_Body()
    {
        // Arrange
        var response = new ResponseLogModel
        {
            StatusCode = 200,
            Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }!,
            Body = "{ \"id\": 1 }"
        };

        // Act
        await _sut.LogResponseAsync(response);

        // Assert
        var events = InMemorySink.Instance.LogEvents.ToList();

        events.Should().ContainSingle(e =>
            e.Level == LogEventLevel.Information &&
            e.MessageTemplate.Text.Contains("HTTP Response {StatusCode}"));

        events.Should().ContainSingle(e =>
            e.Level == LogEventLevel.Debug &&
            e.MessageTemplate.Text.Contains("HTTP Response Body: {Body}"));
    }
}