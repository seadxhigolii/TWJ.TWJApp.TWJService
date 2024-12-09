using MapperSegregator.Interfaces;
using System;
using System.Threading.Tasks;

namespace TWJ.TWJApp.TWJService.Application.Services.Banner.Queries.GetAll
{
    public class GetAllBannerModel : IProfile
    {
        public Guid Id { get; set; }
        public string AdvertiserName { get; set; }
        public string ImageUrl { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public async Task MapData(IProfileMapper profileMapper)
        {
            profileMapper.Build<Domain.Entities.Banner, GetAllBannerModel>(
                (src, options) =>
                {
                    return new GetAllBannerModel
                    {
                        Id = src.Id,
                        AdvertiserName = src.AdvertiserName,
                        ImageUrl = src.ImageUrl,
                        Width = src.Width,
                        Height = src.Height
                    };
                });
        }
    }
}