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

namespace TWJ.TWJApp.TWJService.Application.Services.Category.Queries.GetFiltered
{
public class GetFilteredCategoryQueryHandler : IRequestHandler<GetFilteredCategoryQuery, FilterResponse<GetFilteredCategoryModel>>
{
    private readonly ITWJAppDbContext _context;
        
    public GetFilteredCategoryQueryHandler(ITWJAppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<FilterResponse<GetFilteredCategoryModel>> Handle(GetFilteredCategoryQuery request, CancellationToken cancellationToken)
    {
        try
        {
            return new FilterResponse<GetFilteredCategoryModel>
            {
                Data = await _context.ProductCategories
                        .AsNoTracking()
                        .OrderByDescending(x => x.Name)
                        .SkipAndTake(request.Page, request.PageSize, out int pages, out int items)
                        .MapToListAsync<TWJ.TWJApp.TWJService.Domain.Entities.Category, GetFilteredCategoryModel>(),
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