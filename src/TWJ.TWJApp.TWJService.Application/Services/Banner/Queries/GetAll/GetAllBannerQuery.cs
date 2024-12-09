using MediatR;
using System.Collections.Generic;

namespace TWJ.TWJApp.TWJService.Application.Services.Banner.Queries.GetAll
{
    public class GetAllBannerQuery : IRequest<IList<GetAllBannerModel>>
    {
    }
}