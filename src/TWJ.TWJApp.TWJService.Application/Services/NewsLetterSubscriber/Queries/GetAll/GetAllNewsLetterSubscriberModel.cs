using MapperSegregator.Interfaces;
using System;
using System.Threading.Tasks;

namespace TWJ.TWJApp.TWJService.Application.Services.NewsLetterSubscriber.Queries.GetAll
{
    public class GetAllNewsLetterSubscriberModel : IProfile
    {
        public Guid Id { get; set; }
        public string Email { get; set; }

        public async Task MapData(IProfileMapper profileMapper)
        {
            profileMapper.Build<Domain.Entities.NewsLetterSubscriber, GetAllNewsLetterSubscriberModel>(
                (src, options) =>
                {
                    return new GetAllNewsLetterSubscriberModel
                    {
                        Id = src.Id,
                        Email = src.Email,
                    };
                });
        }
    }
}