
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

namespace TWJ.TWJApp.TWJService.Application.Services.BlogPost.Queries.GetFiltered
{
public class GetFilteredBlogPostQueryHandler : IRequestHandler<GetFilteredBlogPostQuery, FilterResponse<GetFilteredBlogPostModel>>
{
    private readonly ITWJAppDbContext _context;
        
    public GetFilteredBlogPostQueryHandler(ITWJAppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<FilterResponse<GetFilteredBlogPostModel>> Handle(GetFilteredBlogPostQuery request, CancellationToken cancellationToken)
    {
        try
        {
            return new FilterResponse<GetFilteredBlogPostModel>
            {
                Data = await _context.BlogPost
                        .AsNoTracking()
                        .OrderByDescending(x => x.CreatedAt)
                        .SkipAndTake(request.Page, request.PageSize, out int pages, out int items)
                        .MapToListAsync<TWJ.TWJApp.TWJService.Domain.Entities.BlogPost, GetFilteredBlogPostModel>(),
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