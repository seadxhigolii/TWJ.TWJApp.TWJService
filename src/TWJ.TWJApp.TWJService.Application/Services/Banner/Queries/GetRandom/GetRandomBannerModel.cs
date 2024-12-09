using MapperSegregator.Interfaces;
using System;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Services.Banner.Queries.GetAll;

namespace TWJ.TWJApp.TWJService.Application.Services.Banner.Queries.GetRandom
{
    public class GetRandomBannerModel : IProfile
    {
        public Guid Id { get; set; }
        public string AdvertiserName { get; set; }
        public string FinalUrl { get; set; }
        public string ImageUrl { get; set; }
        public string Position { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public async Task MapData(IProfileMapper profileMapper)
        {
            profileMapper.Build<Domain.Entities.Banner, GetRandomBannerModel>(
                (src, options) =>
                {
                    return new GetRandomBannerModel
                    {
                        Id = src.Id,
                        AdvertiserName = src.AdvertiserName,
                        ImageUrl = src.ImageUrl,
                        Width = src.Width,
                        Height = src.Height,
                        FinalUrl = src.ProductId.HasValue && src.Product != null ? src.Product.AffiliateLink : src.DestinationUrl
                    };
                });
        }
    }
}
