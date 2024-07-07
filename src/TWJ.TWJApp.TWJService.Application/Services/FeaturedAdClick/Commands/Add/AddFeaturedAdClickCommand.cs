using MediatR;
using System;

namespace TWJ.TWJApp.TWJService.Application.Services.FeaturedAdClick.Commands.Add
{
    public class AddFeaturedAdClickCommand : IRequest<Unit>
    {
        public Guid ProductId { get; set; }
        public Guid BlogPostId { get; set; }
        public DateTime ClickTime { get; set; }
        public string UserSessionId { get; set; }

        public TWJ.TWJApp.TWJService.Domain.Entities.FeaturedAdClick AddFeaturedAdClick()
        {
            return new TWJ.TWJApp.TWJService.Domain.Entities.FeaturedAdClick
            {
                ProductId = ProductId,
                BlogPostId = BlogPostId,
                ClickTime = ClickTime,
                UserSessionId = UserSessionId
            };
        }
    }
}
