using MediatR;

namespace TWJ.TWJApp.TWJService.Application.Services.BlogPostCategory.Commands.Add
{
    public class AddBlogPostCategoryCommand : IRequest<Unit>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string URL { get; set; }

        public Domain.Entities.BlogPostCategory AddBlogPostCategory()
        {
            return new Domain.Entities.BlogPostCategory
            {
                Name = Name,
                Description = Description,
                URL = URL
            };
        }
    }
}