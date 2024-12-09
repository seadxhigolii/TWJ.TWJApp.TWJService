using MapperSegregator.Interfaces;
using System;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Services.NewsLetterSubscriber.Queries.GetAll;

namespace TWJ.TWJApp.TWJService.Application.Services.NewsLetterSubscriber.Queries.GetById
{
    public class GetNewsLetterSubscriberByIdModel : IProfile
    {
        public Guid Id { get; set; }
        public string Email { get; set; }

        public async Task MapData(IProfileMapper profileMapper)
        {
            profileMapper.Build<Domain.Entities.NewsLetterSubscriber, GetNewsLetterSubscriberByIdModel>(
                (src, options) =>
                {
                    return new GetNewsLetterSubscriberByIdModel
                    {
                        Id = src.Id,
                        Email = src.Email,
                    };
                });
        }
    }
}