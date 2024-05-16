using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Helpers.Interfaces;
using TWJ.TWJApp.TWJService.Application.Interfaces;

namespace TWJ.TWJApp.TWJService.Application.Services.BlogPost.Queries.GetAll
{
    public class GetAllBlogPostQueryHandler : IRequestHandler<GetAllBlogPostQuery, IList<GetAllBlogPostModel>>
    {
        private readonly ITWJAppDbContext _context;
        private readonly IGlobalHelperService _globalHelper;
        private readonly string currentClassName = "";

        public GetAllBlogPostQueryHandler(ITWJAppDbContext context, IGlobalHelperService globalHelper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _globalHelper = globalHelper ?? throw new ArgumentNullException(nameof(globalHelper));
            currentClassName = GetType().Name;
        }

        public async Task<IList<GetAllBlogPostModel>> Handle(GetAllBlogPostQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var now = DateTime.UtcNow;
                var threshold = now.Subtract(TimeSpan.FromHours(24));

                var data = await _context.BlogPosts
                                    .Where(x=>x.Published == true)
                                    //.Where(x=>x.Published == true && x.CreatedAt >= threshold)
                                    .AsNoTracking()
                                    .OrderByDescending(x => x.CreatedAt)
                                    .ToListAsync(cancellationToken);

                var mappedData = data.Select(t => new GetAllBlogPostModel
                {
                    Id = t.Id,
                    Title = t.Title,
                    UserId = t.UserId,
                    ProductCategoryId = t.ProductCategoryId,
                    CreatedAt = t.CreatedAt,
                    Image = t.Image,
                    Views = t.Views,
                    URL = t.URL
                    
                }).ToList();
                return mappedData;
            }
            catch (Exception ex)
            {
                await _globalHelper.Log(ex, currentClassName);
                throw ex;
            }
        }
    }
}