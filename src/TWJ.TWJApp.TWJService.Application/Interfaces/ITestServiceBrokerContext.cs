using System.Collections.Generic;
using System.Threading.Tasks;

namespace TWJ.TWJApp.TWJService.Application.Interfaces
{
    public interface ITestServiceBrokerContext
    {
        Task<IList<object>> GetData();
    }
}
