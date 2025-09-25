namespace Domain.Shared.Tests.ValueObjects;

public class EntityIdTests : TestDataGenerator
{
    const int Value = 123;
    const int Value2 = 456;
    

    [Fact]
    public void Should_Create_Instance_With_Specified_Value()
    {
        var id = OrderId.New(Value);
        id.Value.Should().Be(Value);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData((short)0)]
    [InlineData((long)0)]
    [InlineData("")]
    [InlineData(null)]
    public void IsDefault_Should_Handle_All_Cases<T>(T value)
    {
        Action act = () =>
        {
            _ = new TestId<T>(value);
        };

        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("ID cannot be default/empty. (Parameter 'value')");
       
    }

    [Fact]
    public void Two_Ids_With_Same_Value_Should_Be_Equal()
    {
        var id1 = OrderId.New(Value);
        var id2 = OrderId.New(Value);

        id1.Should().Be(id2);
        id1.Equals(id2).Should().BeTrue();
    }

    [Fact]
    public void Two_Ids_With_Different_Values_Should_Not_Be_Equal()
    {
        var id1 = OrderId.New(Value);
        var id2 = OrderId.New(Value2);

        id1.Should().NotBe(id2);
        id1.Equals(id2).Should().BeFalse();
    }

    [Fact]
    public void ToString_Should_Return_Value_String()
    {
        var id = OrderId.New(Value);

        id.ToString().Should().Be(Value.ToString());
    }
}