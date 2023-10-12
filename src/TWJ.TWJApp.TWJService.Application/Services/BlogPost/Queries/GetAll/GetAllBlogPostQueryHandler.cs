using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;

namespace TWJ.TWJApp.TWJService.Application.Services.BlogPost.Queries.GetAll
{
    public class GetAllBlogPostQueryHandler : IRequestHandler<GetAllBlogPostQuery, IList<GetAllBlogPostModel>>
    {
        private readonly ITWJAppDbContext _context;

        public GetAllBlogPostQueryHandler(ITWJAppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IList<GetAllBlogPostModel>> Handle(GetAllBlogPostQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var data = await _context.BlogPost
                                    .AsNoTracking()
                                    .OrderByDescending(x => x.CreatedAt)
                                    .ToListAsync(cancellationToken);

                var mappedData = data.Select(t => new GetAllBlogPostModel
                {
                    Id = t.Id,
                    Title = t.Title,
                    Content = t.Content,
                    UserId = t.UserId,
                    CategoryId = t.CategoryId,
                    Tags = t.Tags,
                    Image = t.Image,
                    Views = t.Views,
                    Likes = t.Likes,
                    Dislikes = t.Dislikes,
                    NumberOfComments = t.NumberOfComments,
                    ProductID = t.ProductID,
                }).ToList();
                return mappedData;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}