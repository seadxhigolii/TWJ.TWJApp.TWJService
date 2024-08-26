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

namespace TWJ.TWJApp.TWJService.Application.Services.Tag.Queries.GetFiltered
{
    public class GetFilteredTagQueryHandler : IRequestHandler<GetFilteredTagQuery, FilterResponse<GetFilteredTagModel>>
    {
        private readonly ITWJAppDbContext _context;
        private readonly IGlobalHelperService _globalHelper;
        private readonly string currentClassName = "";

        public GetFilteredTagQueryHandler(ITWJAppDbContext context, IGlobalHelperService globalHelper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _globalHelper = globalHelper ?? throw new ArgumentNullException(nameof(globalHelper));
            currentClassName = GetType().Name;
        }

        public async Task<FilterResponse<GetFilteredTagModel>> Handle(GetFilteredTagQuery request, CancellationToken cancellationToken)
        {
            try
            {
                IQueryable<TWJ.TWJApp.TWJService.Domain.Entities.Tag> query = _context.Tag.AsQueryable().Where(x=>x.PostCount > 0).OrderByDescending(x=>x.PostCount);

                var totalItems = await query.CountAsync(cancellationToken);

                query = query.ApplySorting(request.SortBy, request.SortDirection, topRecords: request.TopRecords);

                if (!request.TopRecords.HasValue)
                {
                    query = query.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize);
                }

                var mappedData = await query.Select(src => new GetFilteredTagModel
                {
                    Id = src.Id,
                    PostCount = src.PostCount,
                    Name = src.Name

                }).ToListAsync(cancellationToken);

                return new FilterResponse<GetFilteredTagModel>
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
