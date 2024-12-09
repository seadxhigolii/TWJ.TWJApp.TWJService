using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Application.Helpers.Interfaces;

namespace TWJ.TWJApp.TWJService.Application.Services.Banner.Queries.GetAll
{
    public class GetAllBannerQueryHandler : IRequestHandler<GetAllBannerQuery, IList<GetAllBannerModel>>
    {
        private readonly ITWJAppDbContext _context;
        private readonly IGlobalHelperService _globalHelper;
        private readonly string currentClassName = "";

        public GetAllBannerQueryHandler(ITWJAppDbContext context, IGlobalHelperService globalHelper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _globalHelper = globalHelper ?? throw new ArgumentNullException(nameof(globalHelper));
            currentClassName = GetType().Name;
        }

        public async Task<IList<GetAllBannerModel>> Handle(GetAllBannerQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var data = await _context.Banners
                                    .AsNoTracking()
                                    .OrderByDescending(x => x.AdvertiserName)
                                    .ToListAsync(cancellationToken);

                var mappedData = data.Select(t => new GetAllBannerModel
                {
                    Id = t.Id,
                    AdvertiserName = t.AdvertiserName,
                    ImageUrl = t.ImageUrl
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