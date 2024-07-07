using MapperSegregator.Interfaces;
using System;
using System.Threading.Tasks;

namespace TWJ.TWJApp.TWJService.Application.Services.FeaturedAdClick.Queries.GetAll
{
    public class GetAllFeaturedAdClickModel : IProfile
    {
        public Guid Id { get; set; }
        public Guid BlogPostId { get; set; }
        public Guid ProductId { get; set; }
        public DateTime ClickTime { get; set; }
        public string UserSessionId { get; set; }

        public async Task MapData(IProfileMapper profileMapper)
        {
            profileMapper.Build<TWJ.TWJApp.TWJService.Domain.Entities.FeaturedAdClick, GetAllFeaturedAdClickModel>(
                (src, options) =>
                {
                    return new GetAllFeaturedAdClickModel
                    {
                        Id = src.Id,
                        BlogPostId = src.BlogPostId,
                        ProductId = src.ProductId,
                        ClickTime = src.ClickTime,
                        UserSessionId = src.UserSessionId

                    };
                });
        }
    }
}
