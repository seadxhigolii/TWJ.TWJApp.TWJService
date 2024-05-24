using MapperSegregator.Interfaces;
using System;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Services.Banner.Queries.GetAll;

namespace TWJ.TWJApp.TWJService.Application.Services.Banner.Queries.GetById
{
    public class GetBannerByIdModel : IProfile
    {
        public Guid Id { get; set; }
        public string AdvertiserName { get; set; }
        public string ImageUrl { get; set; }

        public async Task MapData(IProfileMapper profileMapper)
        {
            profileMapper.Build<Domain.Entities.Banner, GetBannerByIdModel>(
                (src, options) =>
                {
                    return new GetBannerByIdModel
                    {
                        Id = src.Id,
                        AdvertiserName = src.AdvertiserName,
                        ImageUrl = src.ImageUrl
                    };
                });
        }
    }
}