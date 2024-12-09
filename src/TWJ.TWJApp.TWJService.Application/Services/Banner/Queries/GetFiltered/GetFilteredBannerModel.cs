using MapperSegregator.Interfaces;
using System;
using System.Threading.Tasks;

namespace TWJ.TWJApp.TWJService.Application.Services.Banner.Queries.GetFiltered
{
    public class GetFilteredBannerModel : IProfile
    {
        public Guid Id { get; set; }
        public string AdvertiserName { get; set; }
        public string ImageUrl { get; set; }

        public async Task MapData(IProfileMapper profileMapper)
        {
            profileMapper.Build<Domain.Entities.Banner, GetFilteredBannerModel>(
                (src, options) =>
                {
                    return new GetFilteredBannerModel
                    {
                        Id = src.Id,
                        AdvertiserName = src.AdvertiserName,
                        ImageUrl = src.ImageUrl
                    };
                });
        }
    }
}