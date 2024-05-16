using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Dto.Commands.Base;
using TWJ.TWJApp.TWJService.Application.Dto.Models.Base;
using TWJ.TWJApp.TWJService.Application.Helpers.Interfaces;
using TWJ.TWJApp.TWJService.Application.Interfaces;

namespace TWJ.TWJApp.TWJService.Application.Services.Product.Queries.GetFiltered
{
    public class GetFilteredProductQueryHandler : IRequestHandler<GetFilteredProductQuery, FilterResponse<GetFilteredProductModel>>
    {
        private readonly ITWJAppDbContext _context;
        private readonly IGlobalHelperService _globalHelper;
        private readonly string currentClassName = "";

        public GetFilteredProductQueryHandler(ITWJAppDbContext context, IGlobalHelperService globalHelper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _globalHelper = globalHelper ?? throw new ArgumentNullException(nameof(_globalHelper));
            currentClassName = GetType().Name;
        }

        public async Task<FilterResponse<GetFilteredProductModel>> Handle(GetFilteredProductQuery request, CancellationToken cancellationToken)
        {
            try
            {
                IQueryable<TWJ.TWJApp.TWJService.Domain.Entities.Product> query = _context.Products.AsQueryable();

                query = query.ApplySorting(request.SortBy, request.SortDirection, topRecords: request.TopRecords);

                if (!request.TopRecords.HasValue)
                {
                    query = query.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize);
                }

                var totalItems = await _context.Products.CountAsync(cancellationToken);

                var mappedData = await query.Select(src => new GetFilteredProductModel
                {
                    Id = src.Id,
                    ProductName = src.ProductName,
                    VendorName = src.VendorName,
                    AvgRating = src.AvgRating,
                    Price = src.Price
                }).ToListAsync(cancellationToken);

                return new FilterResponse<GetFilteredProductModel>
                {
                    Data = mappedData,
                    TotalPages = (int)Math.Ceiling((double)totalItems / request.PageSize),
                    TotalItems = totalItems
                };
            }
            catch (Exception ex)
            {
                await _globalHelper.Log(ex, currentClassName);
                throw new Exception("An error occurred while processing the request.", ex);
            }
        }

    }
}