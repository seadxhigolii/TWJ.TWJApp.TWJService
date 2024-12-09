using MapperSegregator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Services.FeaturedAdClick.Queries.GetAll;

namespace TWJ.TWJApp.TWJService.Application.Services.AdClick.Queries.GetAll
{
    public class GetAllAdClickModel : IProfile
    {
        public Guid Id { get; set; }
        public Guid BlogPostId { get; set; }
        public Guid BannerId { get; set; }
        public Guid ProductId { get; set; }
        public DateTime ClickTime { get; set; }
        public string UserSessionId { get; set; }
        public string URL { get; set; }

        public async Task MapData(IProfileMapper profileMapper)
        {
            profileMapper.Build<TWJ.TWJApp.TWJService.Domain.Entities.AdClick, GetAllAdClickModel>(
                (src, options) =>
                {
                    return new GetAllAdClickModel
                    {
                        Id = src.Id,
                        BlogPostId = src.BlogPostId,
                        BannerId = src.BannerId,
                        ProductId = src.ProductId,
                        ClickTime = src.ClickTime,
                        UserSessionId = src.UserSessionId,
                        URL = URL
                    };
                });
        }
    }
}
