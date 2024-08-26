using MapperSegregator.Interfaces;
using System;
using System.Threading.Tasks;

namespace TWJ.TWJApp.TWJService.Application.Services.User.Queries.GetById
{
    public class GetUserByIdModel : IProfile
    {
        public Guid Id { get; set; }
        public string Description { get; set; }

        public async Task MapData(IProfileMapper profileMapper)
        {
            profileMapper.Build<TWJ.TWJApp.TWJService.Domain.Entities.User, GetUserByIdModel>(
                (src, options) =>
                {
                    return new GetUserByIdModel
                    {
                        Id = src.Id,
                        Description =  src.Description
                    };
                });
        }
    }
}
