using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Services.Category.Queries.GetAll;

namespace TWJ.TWJApp.TWJService.Application.Services.FeaturedAdClick.Queries.GetAll
{
    public class GetAllFeaturedAdClickQuery : IRequest<IList<GetAllFeaturedAdClickModel>>
    {
    }
}
