using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;

namespace TWJ.TWJApp.TWJService.Application.Services.FeaturedAdClick.Queries.GetAll
{
    public class GetAllFeaturedAdClickQueryHandler : IRequestHandler<GetAllFeaturedAdClickQuery, IList<GetAllFeaturedAdClickModel>>
    {
        private readonly ITWJAppDbContext _context;

        public GetAllFeaturedAdClickQueryHandler(ITWJAppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IList<GetAllFeaturedAdClickModel>> Handle(GetAllFeaturedAdClickQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var data = await _context.FeaturedAdClicks
                                    .AsNoTracking()
                                    .OrderByDescending(x => x.CreatedAt)
                                    .ToListAsync(cancellationToken);

                var mappedData = data.Select(t => new GetAllFeaturedAdClickModel
                {
                    Id = t.Id,
                    ProductId = t.ProductId,
                    BlogPostId = t.BlogPostId,
                    ClickTime = t.ClickTime,
                    UserSessionId = t.UserSessionId
                }).ToList();
                return mappedData;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
