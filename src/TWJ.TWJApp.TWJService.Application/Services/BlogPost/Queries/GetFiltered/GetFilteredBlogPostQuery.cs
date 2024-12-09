
using MediatR;
using TWJ.TWJApp.TWJService.Application.Dto.Commands.Base;
using TWJ.TWJApp.TWJService.Application.Dto.Models.Base;

namespace TWJ.TWJApp.TWJService.Application.Services.BlogPost.Queries.GetFiltered
{
    public class GetFilteredBlogPostQuery : FilterRequest, IRequest<FilterResponse<GetFilteredBlogPostModel>>
    {
    }
}