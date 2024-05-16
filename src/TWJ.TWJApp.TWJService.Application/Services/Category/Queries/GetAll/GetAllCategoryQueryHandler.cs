using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;

namespace TWJ.TWJApp.TWJService.Application.Services.Category.Queries.GetAll
{
    public class GetAllCategoryQueryHandler : IRequestHandler<GetAllCategoryQuery, IList<GetAllCategoryModel>>
    {
        private readonly ITWJAppDbContext _context;

        public GetAllCategoryQueryHandler(ITWJAppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IList<GetAllCategoryModel>> Handle(GetAllCategoryQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var data = await _context.ProductCategories
                                    .Where(x=>x.Active == true)
                                    .AsNoTracking()
                                    .OrderByDescending(x => x.CreatedAt)
                                    .ToListAsync(cancellationToken);

                var mappedData = data.Select(t => new GetAllCategoryModel
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description
                }).ToList();
                return mappedData;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}