using MapperSegregator.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Common.Constants;
using TWJ.TWJApp.TWJService.Common.Exceptions;

namespace TWJ.TWJApp.TWJService.Application.Services.Product.Queries.GetById
{
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, GetProductByIdModel>
    {
        private readonly ITWJAppDbContext _context;
        private readonly IMapperSegregator _mapper;
        
        public GetProductByIdQueryHandler(ITWJAppDbContext context, IMapperSegregator mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<GetProductByIdModel> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var data = await _context.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (data == null) throw new BadRequestException(ValidatorMessages.NotFound("Record"));

                return await _mapper.MapAsync<TWJ.TWJApp.TWJService.Domain.Entities.Product, GetProductByIdModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}