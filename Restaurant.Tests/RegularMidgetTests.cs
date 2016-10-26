using System.Linq;
using Moq;
using Restaurant.Infrastructure.Abstract;
using Restaurant.Messages;
using Restaurant.Models;
using Restaurant.Workers;
using Xunit;

namespace Restaurant.Tests
{
    public class RegularMidgetTests
    {
        [Fact]
        public void ShouldPublish_CookFood_When_OrderPlaced_Received()
        {
            var publisherMock = new Mock<IPublisher>();
            var midget = new RegularMidget(publisherMock.Object);

            midget.Handle(new OrderPlaced(new Order(), string.Empty));

            publisherMock.Verify(p => p.Publish(It.IsAny<CookFood>()));
        }

        [Fact]
        public void ShouldPublish_PriceOrdered_When_OrderCooked_Received()
        {
            var publisherMock = new Mock<IPublisher>();
            var midget = new RegularMidget(publisherMock.Object);

            midget.Handle(new OrderCooked(new Order(), string.Empty));

            publisherMock.Verify(p => p.Publish(It.IsAny<PriceOrdered>()));
        }

        [Fact]
        public void ShouldPublish_TakePayment_When_OrderPriced_Received()
        {
            var publisherMock = new Mock<IPublisher>();
            var midget = new RegularMidget(publisherMock.Object);

            midget.Handle(new OrderPriced(new Order(), string.Empty));

            publisherMock.Verify(p => p.Publish(It.IsAny<TakePayment>()));
        }

        [Fact]
        public void ShouldCleanUp_When_OrderPaid_Received()
        {
            var publisherMock = new Mock<IPublisher>();
            var midget = new RegularMidget(publisherMock.Object)
            {
                CleanUp = corrId => corrId.SequenceEqual("corrId")
            };

            midget.Handle(new OrderPaid(new Order(), "corrId"));
        }
    }
}
