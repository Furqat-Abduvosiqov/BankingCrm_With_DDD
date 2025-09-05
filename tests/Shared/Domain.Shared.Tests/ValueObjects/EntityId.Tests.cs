using System.Reflection;

namespace Domain.Shared.Tests.ValueObjects;

public class EntityIdTests : TestDataGenerator
{
    [Fact]
    public void New_Should_Create_NonEmpty_Id()
    {
        var id = OrderId.New();

        id.Should().NotBeNull();
        id.Value.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void FromGuid_Should_Create_Id_With_Specified_Guid()
    {
        var guid = Guid.NewGuid();

        var id = OrderId.FromGuid(guid);

        id.Value.Should().Be(guid);
    }

    [Fact]
    public void Constructor_Should_Throw_When_Guid_Is_Empty()
    {
        Action act = () => OrderId.FromGuid(Guid.Empty);

        act.Should()
            .Throw<TargetInvocationException>()
            .WithInnerException<ArgumentException>()
            .WithMessage("ID cannot be empty. (Parameter 'value')");
    }

    [Fact]
    public void Two_Ids_With_Same_Guid_Should_Be_Equal()
    {
        var guid = Guid.NewGuid();

        var id1 = OrderId.FromGuid(guid);
        var id2 = OrderId.FromGuid(guid);

        id1.Should().Be(id2);
        id1.Equals(id2).Should().BeTrue();
    }

    [Fact]
    public void Two_Ids_With_Different_Guids_Should_Not_Be_Equal()
    {
        var id1 = OrderId.New();
        var id2 = OrderId.New();

        id1.Should().NotBe(id2);
        id1.Equals(id2).Should().BeFalse();
    }

    [Fact]
    public void ToString_Should_Return_Guid_String()
    {
        var guid = Guid.NewGuid();
        var id = OrderId.FromGuid(guid);

        id.ToString().Should().Be(guid.ToString());
    }
}