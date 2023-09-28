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
using TWJ.TWJApp.TWJService.Domain.Entities.Base;

namespace TWJ.TWJApp.TWJService.Application.Services.Base.Queries.GetFiltered
{
    public class GetFilteredBaseQueryHandler : IRequestHandler<GetFilteredBaseQuery, FilterResponse<GetFilteredBaseModel>>
    {
        private readonly ITWJAppDbContext _context;
        public GetFilteredBaseQueryHandler(ITWJAppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<FilterResponse<GetFilteredBaseModel>> Handle(GetFilteredBaseQuery request, CancellationToken cancellationToken)
        {
            try
            {
                return new FilterResponse<GetFilteredBaseModel>
                {
                    Data = await _context.Base
                            .AsNoTracking()
                            .OrderByDescending(x => x.Property)
                            .SkipAndTake(request.Page, request.PageSize, out int pages, out int items)
                            .OrderByDescending(x => x.Property)
                            .MapToListAsync<BaseModel, GetFilteredBaseModel>(),
                    TotalPages = pages,
                    TotalItems = items
                };
            }
            catch (Exception e)
            {
                throw e;
            }

        }
    }
}
