using MapperSegregator.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Helpers.Interfaces;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Application.Services.SEOKeyword.Queries.GetAll;

namespace TWJ.TWJApp.TWJService.Application.Services.Tag.Queries.GetAll
{
    public class GetAllTagQueryHandler : IRequestHandler<GetAllTagQuery, IList<GetAllTagModel>>
    {
        private readonly ITWJAppDbContext _context;
        private readonly IGlobalHelperService _globalHelper;
        private readonly string currentClassName = "";

        public GetAllTagQueryHandler(ITWJAppDbContext context, IGlobalHelperService globalHelper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _globalHelper = globalHelper ?? throw new ArgumentNullException(nameof(globalHelper));
            currentClassName = GetType().Name;
        }

        public async Task<IList<GetAllTagModel>> Handle(GetAllTagQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var data = await _context.Tag
                                    .AsNoTracking()
                                    .Where(x=>x.PostCount > 0)
                                    .OrderByDescending(x => x.PostCount)
                                    .ToListAsync(cancellationToken);

                var mappedData = data.Select(t => new GetAllTagModel
                {
                    Id = t.Id,
                    PostCount = t.PostCount,
                    Name = t.Name
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
