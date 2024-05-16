using MapperSegregator.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Common.Constants;
using TWJ.TWJApp.TWJService.Common.Exceptions;
using TWJ.TWJApp.TWJService.Application.Helpers.Interfaces;

namespace TWJ.TWJApp.TWJService.Application.Services.News.Queries.GetById
{
    public class GetNewsByIdQueryHandler : IRequestHandler<GetNewsByIdQuery, GetNewsByIdModel>
    {
        private readonly ITWJAppDbContext _context;
        private readonly IMapperSegregator _mapper;
        private readonly IGlobalHelperService _globalHelper;
        private readonly string currentClassName = "";
        
        public GetNewsByIdQueryHandler(ITWJAppDbContext context, IMapperSegregator mapper, IGlobalHelperService globalHelper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _globalHelper = globalHelper ?? throw new ArgumentNullException(nameof(globalHelper));
            currentClassName = GetType().Name;
        }

        public async Task<GetNewsByIdModel> Handle(GetNewsByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var data = await _context.News.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (data == null) throw new BadRequestException(ValidatorMessages.NotFound("News"));

                return await _mapper.MapAsync<Domain.Entities.News, GetNewsByIdModel>(data);
            }
            catch (Exception ex)
            {
                await _globalHelper.Log(ex, currentClassName);
                throw new Exception("An error occurred while processing the request.", ex);
            }
        }
    }
}