using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Application.Services.FeaturedAdClick.Queries.GetAll;

namespace TWJ.TWJApp.TWJService.Application.Services.AdClick.Queries.GetAll
{
    public class GetAllAdClickQueryHandler : IRequestHandler<GetAllAdClickQuery, IList<GetAllAdClickModel>>
    {
        private readonly ITWJAppDbContext _context;

        public GetAllAdClickQueryHandler(ITWJAppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IList<GetAllAdClickModel>> Handle(GetAllAdClickQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var data = await _context.AdClicks
                                    .AsNoTracking()
                                    .OrderByDescending(x => x.CreatedAt)
                                    .ToListAsync(cancellationToken);

                var mappedData = data.Select(t => new GetAllAdClickModel
                {
                    Id = t.Id,
                    ProductId = t.ProductId,
                    BannerId = t.BannerId,
                    BlogPostId = t.BlogPostId,
                    ClickTime = t.ClickTime,
                    UserSessionId = t.UserSessionId,
                    URL = t.URL
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
