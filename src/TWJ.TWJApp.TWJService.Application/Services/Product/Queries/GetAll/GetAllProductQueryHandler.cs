using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using static System.Net.Mime.MediaTypeNames;
using TWJ.TWJApp.TWJService.Domain.Entities;

namespace TWJ.TWJApp.TWJService.Application.Services.Product.Queries.GetAll
{
    public class GetAllProductQueryHandler : IRequestHandler<GetAllProductQuery, IList<GetAllProductModel>>
    {
        private readonly ITWJAppDbContext _context;

        public GetAllProductQueryHandler(ITWJAppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IList<GetAllProductModel>> Handle(GetAllProductQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var data = await _context.Products
                                    .AsNoTracking()
                                    .OrderByDescending(x => x.AvgRating)
                                    .ToListAsync(cancellationToken);

                var mappedData = data.Select(t => new GetAllProductModel
                {
                    Id = t.Id,
                    ProductName = t.ProductName,
                    Description = t.Description,
                    VendorName = t.VendorName,
                    CategoryId = t.CategoryId,
                    AvgRating = t.AvgRating,
                    Price = t.Price,
                    Currency = t.Currency,
                    AffiliateLink = t.AffiliateLink,
                    Image = t.Image,
                    PromotionStart = t.PromotionStart,
                    PromotionEnd = t.PromotionEnd
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