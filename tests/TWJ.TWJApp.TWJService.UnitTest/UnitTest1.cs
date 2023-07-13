using TWJ.TWJApp.TWJService.Application.Services.TestService.Queries.Get;
using TWJ.TWJApp.TWJService.UnitTest.Base;
using System.Threading.Tasks;
using Xunit;

namespace TWJ.TWJApp.TWJService.UnitTest
{
    public class UnitTest1 : UnitContext
    {
        [Fact]
        public async Task Test1()
        {
            var query = new GetItemsQuery();

            var result = await Mediator.Send(query);

            Assert.NotNull(result);
        }
    }
}