using MediatR;
using TWJ.TWJApp.TWJService.Application.Dto.Commands.Base;
using TWJ.TWJApp.TWJService.Application.Dto.Models.Base;

namespace TWJ.TWJApp.TWJService.Application.Services.BlogPostCategory.Queries.GetFiltered
{
    public class GetFilteredBlogPostCategoryQuery : FilterRequest, IRequest<FilterResponse<GetFilteredBlogPostCategoryModel>>
    {
    }
}