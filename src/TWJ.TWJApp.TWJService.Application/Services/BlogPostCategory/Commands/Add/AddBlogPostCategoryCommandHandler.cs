using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Application.Helpers.Interfaces;

namespace TWJ.TWJApp.TWJService.Application.Services.BlogPostCategory.Commands.Add
{
    public class AddBlogPostCategoryCommandHandler : IRequestHandler<AddBlogPostCategoryCommand, Unit>
    {
        private readonly ITWJAppDbContext _context;
        private readonly IGlobalHelperService _globalHelper;
        private readonly string currentClassName = "";

        public AddBlogPostCategoryCommandHandler(ITWJAppDbContext context, IGlobalHelperService globalHelper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _globalHelper = globalHelper ?? throw new ArgumentNullException(nameof(globalHelper));
            currentClassName = GetType().Name;
        }

        public async Task<Unit> Handle(AddBlogPostCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var urlFriendlyName = request.Name
                    .Replace("&", "and")
                    .ToLower()
                    .Replace(" ", "-");

                var category = request.AddBlogPostCategory();
                category.URL = urlFriendlyName; 

                await _context.BlogPostCategories.AddAsync(category, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
            catch (Exception ex)
            {
                await _globalHelper.Log(ex, currentClassName);
                throw new Exception("An error occurred while processing the request.", ex);
            }
        }

    }
}