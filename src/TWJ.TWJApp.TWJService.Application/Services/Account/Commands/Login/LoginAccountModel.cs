using System;

namespace TWJ.TWJApp.TWJService.Application.Services.Account.Commands.Login
{
    public class LoginAccountModel
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}
