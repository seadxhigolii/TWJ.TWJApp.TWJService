using MapperSegregator.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Application.Services.BlogPost.Queries.GetById;
using TWJ.TWJApp.TWJService.Common.Constants;
using TWJ.TWJApp.TWJService.Common.Exceptions;

namespace TWJ.TWJApp.TWJService.Application.Services.User.Queries.GetById
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, GetUserByIdModel>
    {
        private readonly ITWJAppDbContext _context;

        public GetUserByIdQueryHandler(ITWJAppDbContext context, IMapperSegregator mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<GetUserByIdModel> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var data = await _context.User.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (data == null) throw new BadRequestException(ValidatorMessages.NotFound("Record"));

                var result = new GetUserByIdModel()
                {
                    Id = data.Id,
                    Description = data.Description
                };

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
