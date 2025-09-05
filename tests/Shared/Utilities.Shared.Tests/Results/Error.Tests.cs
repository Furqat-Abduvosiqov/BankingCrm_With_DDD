using FluentAssertions;
using Utilities.Shared.Results;

namespace Utilities.Shared.Tests.Results;

public class ErrorTests
{
    [Fact]
    public void NotFound_Should_Create_Error_With_Code_And_Message()
    {
        var error = Error.NotFound("Order");

        error.Code.Should().Be("NotFound");
        error.Message.Should().Be("Order not found.");
    }

    [Fact]
    public void Validation_Should_Create_Error_With_Code_And_Message()
    {
        var error = Error.Validation("Invalid input");

        error.Code.Should().Be("Validation");
        error.Message.Should().Be("Invalid input");
    }
}