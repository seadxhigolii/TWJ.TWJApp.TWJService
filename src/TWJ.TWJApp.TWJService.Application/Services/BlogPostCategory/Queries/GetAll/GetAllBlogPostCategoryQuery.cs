using MediatR;
using System.Collections.Generic;

namespace TWJ.TWJApp.TWJService.Application.Services.BlogPostCategory.Queries.GetAll
{
    public class GetAllBlogPostCategoryQuery : IRequest<IList<GetAllBlogPostCategoryModel>>
    {
    }
}