using MediatR;
using System;

namespace TWJ.TWJApp.TWJService.Application.Services.BlogPostCategory.Commands.Update
{
    public class UpdateBlogPostCategoryCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string URL { get; set; }

        public Domain.Entities.BlogPostCategory Update(Domain.Entities.BlogPostCategory blogpostcategory)
        {
            blogpostcategory.Name = Name;
            blogpostcategory.Description = Description;
            blogpostcategory.URL = URL;
            return blogpostcategory;
        }
    }
}