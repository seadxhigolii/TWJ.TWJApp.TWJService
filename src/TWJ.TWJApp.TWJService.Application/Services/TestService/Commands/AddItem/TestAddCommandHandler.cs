using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace TWJ.TWJApp.TWJService.Application.Services.TestService.Commands.AddItem
{
    public class TestAddCommandHandler : IRequestHandler<TestAddItemCommand, Unit>
    {
        public async Task<Unit> Handle(TestAddItemCommand request, CancellationToken cancellationToken)
        {
            await Task.Delay(500);

            return Unit.Value;
        }
    }
}
