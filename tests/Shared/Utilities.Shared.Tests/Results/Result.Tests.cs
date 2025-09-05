using FluentAssertions;
using Utilities.Shared.Results;

namespace Utilities.Shared.Tests.Results;

public class ResultTests
{
    [Fact]
    public void Success_Should_Set_Value_And_IsSuccess()
    {
        var result = Result<int>.Success(42);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(42);
        result.Error.Should().BeNull();
    }

    [Fact]
    public void Failure_Should_Set_Error_And_Not_IsSuccess()
    {
        var error = new Error("E001", "Something went wrong");
        var result = Result<int>.Failure(error);

        result.IsSuccess.Should().BeFalse();
        result.Value.Should().Be(default);
        result.Error.Should().Be(error);
    }
}