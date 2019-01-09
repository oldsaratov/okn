using OKN.WebApp.Controllers;
using System;
using System.Threading;
using EventFlow;
using EventFlow.Queries;
using Moq;
using OKN.Core.Models;
using Xunit;

namespace OKN.WebApp.Tests
{
    public class ObjectsControllerTests
    {
        [Fact]
        public void Get()
        {
            var oid = Guid.NewGuid().ToString();

            var comandBusMock = new Mock<ICommandBus>();
            var queryProcessorMock = new Mock<IQueryProcessor>();

            var _controller = new ObjectsController(comandBusMock.Object, queryProcessorMock.Object);
            //var result = _controller.Get(oid);

            queryProcessorMock.Verify(x => x.ProcessAsync(It.IsAny<IQuery<OknObject>>(), CancellationToken.None));
            queryProcessorMock.VerifyNoOtherCalls();
            comandBusMock.VerifyNoOtherCalls();
        }
    }
}
