using MediatR;

namespace TWJ.TWJApp.TWJService.Application.Services.News.Commands.Add
{
    public class AddNewsCommand : IRequest<Unit>
    {
        public string Title { get; set; }
    }
}