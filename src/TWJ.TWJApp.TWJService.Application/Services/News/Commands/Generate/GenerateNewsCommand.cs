using MediatR;

namespace TWJ.TWJApp.TWJService.Application.Services.News.Commands.Generate
{
    public class GenerateNewsCommand : IRequest<Unit>
    {
        public int NewsType { get; set; }
    }
}
