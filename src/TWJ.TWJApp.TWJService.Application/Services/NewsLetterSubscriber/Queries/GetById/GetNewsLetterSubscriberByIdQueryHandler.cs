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

namespace TWJ.TWJApp.TWJService.Application.Services.NewsLetterSubscriber.Queries.GetById
{
    public class GetNewsLetterSubscriberByIdQueryHandler : IRequestHandler<GetNewsLetterSubscriberByIdQuery, GetNewsLetterSubscriberByIdModel>
    {
        private readonly ITWJAppDbContext _context;
        private readonly IMapperSegregator _mapper;
        private readonly IGlobalHelperService _globalHelper;
        private readonly string currentClassName = "";
        
        public GetNewsLetterSubscriberByIdQueryHandler(ITWJAppDbContext context, IMapperSegregator mapper, IGlobalHelperService globalHelper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _globalHelper = globalHelper ?? throw new ArgumentNullException(nameof(globalHelper));
            currentClassName = GetType().Name;
        }

        public async Task<GetNewsLetterSubscriberByIdModel> Handle(GetNewsLetterSubscriberByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var data = await _context.NewsLetterSubscribers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (data == null) throw new BadRequestException(ValidatorMessages.NotFound("NewsLetterSubscriber"));

                return await _mapper.MapAsync<Domain.Entities.NewsLetterSubscriber, GetNewsLetterSubscriberByIdModel>(data);
            }
            catch (Exception ex)
            {
                await _globalHelper.Log(ex, currentClassName);
                throw new Exception("An error occurred while processing the request.", ex);
            }
        }
    }
}