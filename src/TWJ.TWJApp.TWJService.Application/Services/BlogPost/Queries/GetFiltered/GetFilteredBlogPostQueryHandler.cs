
using Asp.Nappox.School.Common.Extensions;
using Google.Type;
using MapperSegregator.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Dto.Commands.Base;
using TWJ.TWJApp.TWJService.Application.Dto.Models;
using TWJ.TWJApp.TWJService.Application.Dto.Models.Base;
using TWJ.TWJApp.TWJService.Application.Helpers.Interfaces;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Application.Services.Product.Queries.GetFiltered;

namespace TWJ.TWJApp.TWJService.Application.Services.BlogPost.Queries.GetFiltered
{
    public class GetFilteredBlogPostQueryHandler : IRequestHandler<GetFilteredBlogPostQuery, FilterResponse<GetFilteredBlogPostModel>>
    {
        private readonly ITWJAppDbContext _context;
        private readonly IGlobalHelperService _globalHelper;
        private readonly string currentClassName = "";

        public GetFilteredBlogPostQueryHandler(ITWJAppDbContext context, IGlobalHelperService globalHelper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _globalHelper = globalHelper ?? throw new ArgumentNullException(nameof(_globalHelper));
            currentClassName = GetType().Name;
        }

        public async Task<FilterResponse<GetFilteredBlogPostModel>> Handle(GetFilteredBlogPostQuery request, CancellationToken cancellationToken)
        {
            try
            {
                IQueryable<TWJ.TWJApp.TWJService.Domain.Entities.BlogPost> query = _context.BlogPosts.AsQueryable()
                                                                                    .Where(x => !string.IsNullOrEmpty(x.Image));

                var totalItems = await query.CountAsync(cancellationToken);

                query = query.ApplySorting(request.SortBy, request.SortDirection, topRecords: request.TopRecords);

                if (!request.TopRecords.HasValue)
                {
                    query = query.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize);
                }

                var mappedData = await query.Select(src => new GetFilteredBlogPostModel
                {
                    Id = src.Id,
                    Title = src.Title,
                    Image = src.Image,
                    URL = src.URL,
                    CreatedAt = src.CreatedAt,
                    AuthorName = _context.User.Where(u => u.Id == src.UserId).Select(u => u.FirstName + " " + u.LastName).FirstOrDefault(),
                    AuthorImage = _context.User.Where(u => u.Id == src.UserId).Select(u => u.Image).FirstOrDefault()
                }).ToListAsync(cancellationToken);

                return new FilterResponse<GetFilteredBlogPostModel>
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