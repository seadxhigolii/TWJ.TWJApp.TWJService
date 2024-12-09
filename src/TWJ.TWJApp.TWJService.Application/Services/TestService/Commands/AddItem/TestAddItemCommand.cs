using MediatR;

namespace TWJ.TWJApp.TWJService.Application.Services.TestService.Commands.AddItem
{
    public class TestAddItemCommand : IRequest<Unit>
    {
        public string Name { get; set; }
    }
}
