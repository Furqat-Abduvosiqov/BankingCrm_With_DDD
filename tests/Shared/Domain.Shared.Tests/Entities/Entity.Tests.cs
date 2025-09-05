namespace Domain.Shared.Tests.Entities;

public class EntityTestsDataGenerator : TestDataGenerator
{
     #region Domain Events

        [Fact]
        public void AddDomainEvent_Should_Add_Event()
        {
            var entity = new TestEntity();
            var @event = new TestDomainEvent(DateTimeOffset.UtcNow);

            entity.AddDomainEvent(@event);

            entity.DomainEvents.Should()
                .ContainSingle()
                .Which.Should().Be(@event);
        }

        [Fact]
        public void AddDomainEvent_Should_Throw_When_Event_Is_Null()
        {
            var entity = new TestEntity();

            Action act = () => entity.AddDomainEvent(null!);

            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("eventItem");
        }

        [Fact]
        public void RemoveDomainEvent_Should_Remove_Event()
        {
            var entity = new TestEntity();
            var @event = new TestDomainEvent(DateTimeOffset.UtcNow);
            entity.AddDomainEvent(@event);

            entity.RemoveDomainEvent(@event);

            entity.DomainEvents.Should().BeEmpty();
        }

        [Fact]
        public void RemoveDomainEvent_Should_Throw_When_Event_Is_Null()
        {
            var entity = new TestEntity();

            Action act = () => entity.RemoveDomainEvent(null!);

            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("eventItem");
        }

        [Fact]
        public void ClearDomainEvents_Should_Remove_All_Events()
        {
            var entity = new TestEntity();
            entity.AddDomainEvent(new TestDomainEvent(DateTimeOffset.UtcNow));
            entity.AddDomainEvent(new TestDomainEvent(DateTimeOffset.UtcNow));

            entity.ClearDomainEvents();

            entity.DomainEvents.Should().BeEmpty();
        }

        #endregion

        #region Equality

        [Fact]
        public void Entities_With_Same_Id_Should_Be_Equal()
        {
            var guid = Guid.NewGuid();
            var e1 = new TestEntity(guid);
            var e2 = new TestEntity(guid);

            e1.Should().Be(e2);
            (e1 == e2).Should().BeTrue();
            (e1 != e2).Should().BeFalse();
        }

        [Fact]
        public void Entities_With_Different_Ids_Should_Not_Be_Equal()
        {
            var e1 = new TestEntity(Guid.NewGuid());
            var e2 = new TestEntity(Guid.NewGuid());

            e1.Should().NotBe(e2);
            (e1 == e2).Should().BeFalse();
            (e1 != e2).Should().BeTrue();
        }

        [Fact]
        public void Entities_With_Default_Id_Should_Not_Be_Equal()
        {
            var e1 = new TestEntity(Guid.Empty);
            var e2 = new TestEntity(Guid.Empty);

            e1.Should().NotBe(e2);
        }

        [Fact]
        public void Equals_Should_Return_False_When_Types_Are_Different()
        {
            var e1 = new TestEntity(Guid.NewGuid());
            var e2 = new { e1.Id };

            e1.Equals(e2).Should().BeFalse();
        }

        [Fact]
        public void Equals_Should_Return_True_For_Same_Reference()
        {
            var e1 = new TestEntity(Guid.NewGuid());

            // ReSharper disable once EqualExpressionComparison
            e1.Equals(e1).Should().BeTrue();
        }

        [Fact]
        public void Equals_Should_Return_False_When_Comparing_To_Null()
        {
            var e1 = new TestEntity(Guid.NewGuid());

            e1.Equals(null).Should().BeFalse();
        }

        [Fact]
        public void GetHashCode_Should_Be_Same_For_Equal_Entities()
        {
            var guid = Guid.NewGuid();
            var e1 = new TestEntity(guid);
            var e2 = new TestEntity(guid);

            e1.GetHashCode().Should().Be(e2.GetHashCode());
        }

        [Fact]
        public void GetHashCode_Should_Be_Different_For_Different_Entities()
        {
            var e1 = new TestEntity(Guid.NewGuid());
            var e2 = new TestEntity(Guid.NewGuid());

            e1.GetHashCode().Should().NotBe(e2.GetHashCode());
        }

        #endregion
}