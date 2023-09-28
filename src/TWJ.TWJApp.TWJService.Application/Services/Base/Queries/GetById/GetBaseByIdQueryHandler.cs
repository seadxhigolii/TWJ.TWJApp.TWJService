using MapperSegregator.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Common.Constants;
using TWJ.TWJApp.TWJService.Common.Exceptions;
using TWJ.TWJApp.TWJService.Domain.Entities.Base;

namespace TWJ.TWJApp.TWJService.Application.Services.Base.Queries.GetById
{
    public class GetBaseByIdQueryHandler : IRequestHandler<GetBaseByIdQuery, GetBaseByIdModel>
    {
        private readonly ITWJAppDbContext _context;
        private readonly IMapperSegregator _mapper;
        public GetBaseByIdQueryHandler(ITWJAppDbContext context, IMapperSegregator mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<GetBaseByIdModel> Handle(GetBaseByIdQuery request, CancellationToken cancellationToken)
        {
            var data = await _context.Base.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (data == null) throw new BadRequestException(ValidatorMessages.NotFound("Record"));

            return await _mapper.MapAsync<BaseModel, GetBaseByIdModel>(data);
        }
    }
}
