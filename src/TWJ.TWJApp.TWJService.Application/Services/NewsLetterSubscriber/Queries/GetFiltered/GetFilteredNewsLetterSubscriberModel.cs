using MapperSegregator.Interfaces;
using System;
using System.Threading.Tasks;

namespace TWJ.TWJApp.TWJService.Application.Services.NewsLetterSubscriber.Queries.GetFiltered
{
    public class GetFilteredNewsLetterSubscriberModel : IProfile
    {
        public Guid Id { get; set; }
        public string Email { get; set; }

        public async Task MapData(IProfileMapper profileMapper)
        {
            profileMapper.Build<Domain.Entities.NewsLetterSubscriber, GetFilteredNewsLetterSubscriberModel>(
                (src, options) =>
                {
                    return new GetFilteredNewsLetterSubscriberModel
                    {
                        Id = src.Id,
                        Email = src.Email,
                    };
                });
        }
    }
}