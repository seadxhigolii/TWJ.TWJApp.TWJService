using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;

namespace TWJ.TWJApp.TWJService.Application.Services.Base.Queries.GetAll
{
    public class GetAllBaseQueryHandler : IRequestHandler<GetAllBaseQuery, IList<GetAllBaseModel>>
    {
        private readonly ITWJAppDbContext _context;
        public GetAllBaseQueryHandler(ITWJAppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IList<GetAllBaseModel>> Handle(GetAllBaseQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var data = await _context.Base
                                    .AsNoTracking()
                                    .OrderByDescending(x => x.Property)
                                    .ToListAsync(cancellationToken);

                var mappedData = data.Select(t => new GetAllBaseModel
                {
                    Id = t.Id,
                    Property = t.Property
                }).ToList();
                return mappedData;
            }
            catch (Exception e)
            {
                throw e;
            }

        }
    }
}
