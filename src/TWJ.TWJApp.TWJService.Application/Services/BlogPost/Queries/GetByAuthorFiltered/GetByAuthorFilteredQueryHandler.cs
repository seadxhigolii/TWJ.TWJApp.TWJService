using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Dto.Commands.Base;
using TWJ.TWJApp.TWJService.Application.Dto.Models.Base;
using TWJ.TWJApp.TWJService.Application.Helpers.Interfaces;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Application.Services.BlogPost.Queries.GetFiltered;

namespace TWJ.TWJApp.TWJService.Application.Services.BlogPost.Queries.GetByAuthorFiltered
{
    public class GetByAuthorFilteredQueryHandler : IRequestHandler<GetByAuthorFilteredQuery, FilterResponse<GetByAuthorFilteredModel>>
    {
        private readonly ITWJAppDbContext _context;
        private readonly IGlobalHelperService _globalHelper;
        private readonly string currentClassName = "";

        public GetByAuthorFilteredQueryHandler(ITWJAppDbContext context, IGlobalHelperService globalHelper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _globalHelper = globalHelper ?? throw new ArgumentNullException(nameof(_globalHelper));
            currentClassName = GetType().Name;
        }

        public async Task<FilterResponse<GetByAuthorFilteredModel>> Handle(GetByAuthorFilteredQuery request, CancellationToken cancellationToken)
        {
            try
            {
                IQueryable<TWJ.TWJApp.TWJService.Domain.Entities.BlogPost> query = null;

                var author = await _context.User.Where(x=>x.FirstName + " " + x.LastName == request.AuthorName).FirstOrDefaultAsync();
               
                query = _context.BlogPosts.AsQueryable()
                                            .Where(x => !string.IsNullOrEmpty(x.Image) && x.UserId == author.Id && x.Published == true)
                                            .OrderByDescending(x => x.CreatedAt);
                

                var totalItems = await query.CountAsync(cancellationToken);

                query = query.ApplySorting(request.SortBy, request.SortDirection, topRecords: request.TopRecords);

                if (!request.TopRecords.HasValue)
                {
                    query = query.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize);
                }

                var mappedData = await query.Select(src => new GetByAuthorFilteredModel
                {
                    Id = src.Id,
                    Title = src.Title,
                    Image = src.Image,
                    URL = src.URL,
                    CreatedAt = src.CreatedAt,
                    AuthorName = _context.User.Where(u => u.Id == src.UserId).Select(u => u.FirstName + " " + u.LastName).FirstOrDefault(),
                    AuthorImage = _context.User.Where(u => u.Id == src.UserId).Select(u => u.Image).FirstOrDefault()
                }).ToListAsync(cancellationToken);

                return new FilterResponse<GetByAuthorFilteredModel>
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
