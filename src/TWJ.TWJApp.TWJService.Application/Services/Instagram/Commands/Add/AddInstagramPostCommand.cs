using MediatR;

namespace TWJ.TWJApp.TWJService.Application.Services.Instagram.Commands.Add
{
    public class AddInstagramPostCommand : IRequest<Unit>
    {
        public int Type { get; set; }
        public bool IsVideo { get; set; }
    }
}
