using MapperSegregator.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Helpers.Interfaces;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Common.Constants;
using TWJ.TWJApp.TWJService.Common.Exceptions;

namespace TWJ.TWJApp.TWJService.Application.Services.SEOKeyword.Queries.GetById
{
    public class GetSEOKeywordByIdQueryHandler : IRequestHandler<GetSEOKeywordByIdQuery, GetSEOKeywordByIdModel>
    {
        private readonly ITWJAppDbContext _context;
        private readonly IMapperSegregator _mapper;
        private readonly IGlobalHelperService _globalHelper;
        private readonly string currentClassName = "";
        
        public GetSEOKeywordByIdQueryHandler(ITWJAppDbContext context, IMapperSegregator mapper, IGlobalHelperService globalHelper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _globalHelper = globalHelper ?? throw new ArgumentNullException(nameof(globalHelper));
            currentClassName = GetType().Name;
        }

        public async Task<GetSEOKeywordByIdModel> Handle(GetSEOKeywordByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var data = await _context.SEOKeywords.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (data == null) throw new BadRequestException(ValidatorMessages.NotFound("SEOKeyword"));

                return await _mapper.MapAsync<TWJ.TWJApp.TWJService.Domain.Entities.SEOKeyword, GetSEOKeywordByIdModel>(data);
            }
            catch (Exception ex)
            {
                await _globalHelper.Log(ex, currentClassName);
                throw new Exception("An error occurred while processing the request.", ex);
            }
        }
    }
}