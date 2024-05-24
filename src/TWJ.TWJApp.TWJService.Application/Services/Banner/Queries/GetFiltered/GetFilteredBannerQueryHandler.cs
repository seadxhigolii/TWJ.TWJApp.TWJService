using Asp.Nappox.School.Common.Extensions;
using MapperSegregator.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Dto.Models.Base;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Application.Helpers.Interfaces;
using TWJ.TWJApp.TWJService.Application.Dto.Commands.Base;

namespace TWJ.TWJApp.TWJService.Application.Services.Banner.Queries.GetFiltered
{
    public class GetFilteredBannerQueryHandler : IRequestHandler<GetFilteredBannerQuery, FilterResponse<GetFilteredBannerModel>>
    {
        private readonly ITWJAppDbContext _context;
        private readonly IGlobalHelperService _globalHelper;
        private readonly string currentClassName = "";
        
        public GetFilteredBannerQueryHandler(ITWJAppDbContext context, IGlobalHelperService globalHelper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _globalHelper = globalHelper ?? throw new ArgumentNullException(nameof(globalHelper));
            currentClassName = GetType().Name;
        }

        public async Task<FilterResponse<GetFilteredBannerModel>> Handle(GetFilteredBannerQuery request, CancellationToken cancellationToken)
        {
            try
            {
                IQueryable<Domain.Entities.Banner> query = _context.Banners.AsQueryable();
                
                query = query.ApplySorting(request.SortBy, request.SortDirection, topRecords: request.TopRecords);
                
                if (!request.TopRecords.HasValue)
                {
                    query = query.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize);
                }

                var totalItems = await _context.Banners.CountAsync(cancellationToken);

                var mappedData = await query.Select(src => new GetFilteredBannerModel
                {
                    Id = src.Id,
                    AdvertiserName = src.AdvertiserName,
                    ImageUrl = src.ImageUrl,
                }).ToListAsync(cancellationToken);

                return new FilterResponse<GetFilteredBannerModel>
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