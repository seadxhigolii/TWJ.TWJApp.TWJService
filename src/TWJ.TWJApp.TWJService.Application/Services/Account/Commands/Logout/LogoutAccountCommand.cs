using MediatR;

namespace TWJ.TWJApp.TWJService.Application.Services.Account.Commands.Logout
{
    public class LogoutAccountCommand : IRequest<bool>
    {
        public string UserId { get; set; }
    }
}
