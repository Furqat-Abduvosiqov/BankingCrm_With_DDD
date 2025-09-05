namespace Domain.Shared.Tests.ValueObjects;

public class ValueObjectTests : BaseTest
{
    [Fact]
    public void ValueObjects_With_Same_Values_Should_Be_Equal()
    {
        var m1 = new Money(100, "USD");
        var m2 = new Money(100, "USD");

        m1.Should().Be(m2);
        (m1 == m2).Should().BeTrue();
        (m1 != m2).Should().BeFalse();
    }

    [Fact]
    public void ValueObjects_With_Different_Values_Should_Not_Be_Equal()
    {
        var m1 = new Money(100, "USD");
        var m2 = new Money(200, "USD");

        m1.Should().NotBe(m2);
        (m1 == m2).Should().BeFalse();
        (m1 != m2).Should().BeTrue();
    }

    [Fact]
    public void Equal_Objects_Should_Have_Same_HashCode()
    {
        var m1 = new Money(100, "USD");
        var m2 = new Money(100, "USD");

        m1.GetHashCode().Should().Be(m2.GetHashCode());
    }

    [Fact]
    public void Different_Objects_Should_Have_Different_HashCodes()
    {
        var m1 = new Money(100, "USD");
        var m2 = new Money(200, "USD");

        m1.GetHashCode().Should().NotBe(m2.GetHashCode());
    }

    [Fact]
    public void Reference_Equality_Should_Be_True()
    {
        var m1 = new Money(100, "USD");
        var m2 = m1;

        (m1 == m2).Should().BeTrue();
        m1.Equals(m2).Should().BeTrue();
    }

    [Fact]
    public void ShallowCopy_Should_Create_Clone()
    {
        var original = new Money(100, "USD");
        var clone = original.ShallowCopy();

        clone.Should().BeEquivalentTo(original);
        clone.Should().NotBeSameAs(original);
    }

    [Fact]
    public void Objects_Of_Different_Types_Should_Not_Be_Equal()
    {
        var money = new Money(100, "USD");

        var anonymous = new { Amount = 100, Currency = "USD" };

        money.Equals(anonymous).Should().BeFalse();
    }
}