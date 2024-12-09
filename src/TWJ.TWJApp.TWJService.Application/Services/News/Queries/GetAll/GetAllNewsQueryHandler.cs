using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Application.Helpers.Interfaces;

namespace TWJ.TWJApp.TWJService.Application.Services.News.Queries.GetAll
{
    public class GetAllNewsQueryHandler : IRequestHandler<GetAllNewsQuery, IList<GetAllNewsModel>>
    {
        private readonly ITWJAppDbContext _context;
        private readonly IGlobalHelperService _globalHelper;
        private readonly string currentClassName = "";

        public GetAllNewsQueryHandler(ITWJAppDbContext context, IGlobalHelperService globalHelper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _globalHelper = globalHelper ?? throw new ArgumentNullException(nameof(globalHelper));
            currentClassName = GetType().Name;
        }

        public async Task<IList<GetAllNewsModel>> Handle(GetAllNewsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var data = await _context.News
                                    .AsNoTracking()
                                    .OrderByDescending(x => x.CreatedAt)
                                    .ToListAsync(cancellationToken);

                var mappedData = data.Select(t => new GetAllNewsModel
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description
                }).ToList();
                return mappedData;
            }
            catch (Exception ex)
            {
                await _globalHelper.Log(ex, currentClassName);
                throw new Exception("An error occurred while processing the request.", ex);
            }
        }
    }
}