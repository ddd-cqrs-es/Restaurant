using System;
using System.Linq;
using Moq;
using Restaurant.Infrastructure.Abstract;
using Restaurant.Messages;
using Restaurant.Models;
using Restaurant.Workers;
using Should;
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

        [Fact]
        public void ShouldNotPassDoubleMessage()
        {
            var publisherMock = new Mock<IPublisher>();
            var midget = new RegularMidget(publisherMock.Object);

            midget.Handle(new OrderPriced(new Order(), string.Empty));
            midget.Handle(new OrderPriced(new Order(), string.Empty));

            publisherMock.Verify(p => p.Publish(It.IsAny<TakePayment>()), Times.Once);
        }

        [Fact]
        public void ShouldPublish_DuplicateOrder_When_OrderPriced_ReceivedTwice()
        {
            var publisherMock = new Mock<IPublisher>();
            var midget = new RegularMidget(publisherMock.Object);

            midget.Handle(new OrderPriced(new Order(), string.Empty));
            midget.Handle(new OrderPriced(new Order(), string.Empty));

            publisherMock.Verify(p => p.Publish(It.IsAny<DuplicateOrder>()));
        }

        [Fact]
        public void ShouldPublish_DuplicateOrder_When_PriceOrdered_ReceivedTwice()
        {
            var publisherMock = new Mock<IPublisher>();
            var handler = new AssistantManager(publisherMock.Object);

            var msg = new PriceOrdered(new Order(), string.Empty);
            handler.Handle(msg);
            handler.Handle(msg);

            publisherMock.Verify(p => p.Publish(It.IsAny<DuplicateOrder>()));
        }

        [Fact]
        public void ShouldSendRemainderAboutFutureMessage()
        {
            var publisherMock = new Mock<IPublisher>();
            var regularMidget = new RegularMidget(publisherMock.Object);

            regularMidget.Handle(new OrderPlaced(new Order(), "cu"));

            publisherMock.Verify(p => p.Publish(It.IsAny<FutureMessage>()));
        }

        [Fact]
        public void ShouldReTryCookFoodMessage()
        {
            var publisherMock = new Mock<IPublisher>();
            FutureMessage msg = null;
            publisherMock.Setup(x => x.Publish(It.IsAny<FutureMessage>()))
                         .Callback<FutureMessage>(r => msg = r);


            var regularMidget = new RegularMidget(publisherMock.Object);

            var msgToDeliver = new CookFood(new Order(), "cu");
            regularMidget.Handle(new FutureMessage(msgToDeliver, DateTime.MaxValue));

            publisherMock.Verify(p => p.Publish(It.IsAny<FutureMessage>()));
            publisherMock.Verify(p => p.Publish(It.IsAny<CookFood>()));

            msg.MessageToDeliver.MessageId
                .ShouldEqual(msgToDeliver.MessageId);

        }

        [Fact]
        public void ShouldNotReTryCookFoodMessage()
        {
            var publisherMock = new Mock<IPublisher>();
            FutureMessage msg = null;
            publisherMock.Setup(x => x.Publish(It.IsAny<FutureMessage>()))
                         .Callback<FutureMessage>(r => msg = r);

            var regularMidget = new RegularMidget(publisherMock.Object);

            var msgToDeliver = new CookFood(new Order(), "cu");
            regularMidget.Handle(new OrderCooked(new Order(), string.Empty));
            regularMidget.Handle(new FutureMessage(msgToDeliver, DateTime.MaxValue));

            publisherMock.Verify(p => p.Publish(It.IsAny<FutureMessage>()), Times.Never);
            publisherMock.Verify(p => p.Publish(It.IsAny<CookFood>()), Times.Never);
            publisherMock.Verify(p => p.Publish(It.IsAny<PriceOrdered>()));

        }
    }
}
