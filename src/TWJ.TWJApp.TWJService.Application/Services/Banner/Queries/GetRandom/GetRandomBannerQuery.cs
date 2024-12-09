using MediatR;
using System.Collections.Generic;

namespace TWJ.TWJApp.TWJService.Application.Services.Banner.Queries.GetRandom
{
    public class GetRandomBannerQuery : IRequest<IList<GetRandomBannerModel>>
    {
    }
}
