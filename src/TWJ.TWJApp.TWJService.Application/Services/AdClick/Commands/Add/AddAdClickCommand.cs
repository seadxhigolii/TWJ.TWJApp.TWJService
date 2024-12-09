using MediatR;
using System;

namespace TWJ.TWJApp.TWJService.Application.Services.AdClick.Commands.Add
{
    public class AddAdClickCommand : IRequest<Unit>
    {
        public Guid ProductId { get; set; }
        public Guid BlogPostId { get; set; }
        public Guid BannerId { get; set; }
        public DateTime ClickTime { get; set; }
        public string UserSessionId { get; set; }
        public string URL { get; set; }

        public Domain.Entities.AdClick AddAdClick()
        {
            return new Domain.Entities.AdClick
            {
                ProductId = ProductId,
                BlogPostId = BlogPostId,
                BannerId = BannerId,
                ClickTime = ClickTime,
                UserSessionId = UserSessionId,
                URL = URL
            };
        }
    }
}
