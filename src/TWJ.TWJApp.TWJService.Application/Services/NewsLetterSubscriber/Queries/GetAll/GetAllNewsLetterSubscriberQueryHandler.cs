using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Application.Helpers.Interfaces;

namespace TWJ.TWJApp.TWJService.Application.Services.NewsLetterSubscriber.Queries.GetAll
{
    public class GetAllNewsLetterSubscriberQueryHandler : IRequestHandler<GetAllNewsLetterSubscriberQuery, IList<GetAllNewsLetterSubscriberModel>>
    {
        private readonly ITWJAppDbContext _context;
        private readonly IGlobalHelperService _globalHelper;
        private readonly string currentClassName = "";

        public GetAllNewsLetterSubscriberQueryHandler(ITWJAppDbContext context, IGlobalHelperService globalHelper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _globalHelper = globalHelper ?? throw new ArgumentNullException(nameof(globalHelper));
            currentClassName = GetType().Name;
        }

        public async Task<IList<GetAllNewsLetterSubscriberModel>> Handle(GetAllNewsLetterSubscriberQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var data = await _context.NewsLetterSubscribers
                                    .AsNoTracking()
                                    .OrderByDescending(x => x.Email)
                                    .ToListAsync(cancellationToken);

                var mappedData = data.Select(t => new GetAllNewsLetterSubscriberModel
                {
                    Id = t.Id,
                    Email = t.Email
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