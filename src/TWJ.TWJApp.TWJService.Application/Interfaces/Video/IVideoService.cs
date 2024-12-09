using System.Threading.Tasks;
using System.Threading;
using MediatR;

namespace TWJ.TWJApp.TWJService.Application.Interfaces.Video
{
    public interface IVideoService
    {
        Task<Unit> GenerateVideo(int requestType, CancellationToken cancellationToken);
    }
}
