using MediatR;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Helpers.Interfaces;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Application.Services.Banner.Queries.GetAll;

namespace TWJ.TWJApp.TWJService.Application.Services.Banner.Queries.GetRandom
{
    public class GetRandomBannerQueryHandler : IRequestHandler<GetRandomBannerQuery, IList<GetRandomBannerModel>>
    {
        private readonly ITWJAppDbContext _context;
        private readonly IGlobalHelperService _globalHelper;
        private readonly string currentClassName = "";

        public GetRandomBannerQueryHandler(ITWJAppDbContext context, IGlobalHelperService globalHelper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _globalHelper = globalHelper ?? throw new ArgumentNullException(nameof(globalHelper));
            currentClassName = GetType().Name;
        }

        public async Task<IList<GetRandomBannerModel>> Handle(GetRandomBannerQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var banners = await _context.Banners
                                    .AsNoTracking()
                                    .OrderByDescending(x => x.AdvertiserName)
                                    .ToListAsync(cancellationToken);

                var productIds = banners.Select(b => b.ProductId).Where(id => id.HasValue).Distinct().ToList();
                var products = await _context.Products
                             .Where(p => productIds.Contains(p.Id) && p.Id != Guid.Empty)
                             .ToListAsync(cancellationToken);

                var productDictionary = products.ToDictionary(p => p.Id, p => p);

                foreach (var product in products)
                {
                    if (string.IsNullOrEmpty(product.AffiliateLink))
                    {
                        await _globalHelper.Log(new Exception(), currentClassName);
                    }
                }
                var filteredBanners = banners
                                    .Where(t => (t.Position == "TopRight" || t.Position == "Right") ||
                                                (t.Position == "Top" && t.Height <= 90))
                                    .GroupBy(t => t.Position)
                                    .ToDictionary(g => g.Key, g => g.ToList());

                var random = new Random();

                List<GetRandomBannerModel> selectedBanners = new List<GetRandomBannerModel>();

                void AddRandomBanners(string position, int count)
                {
                    if (filteredBanners.ContainsKey(position))
                    {
                        var banners = filteredBanners[position];
                        selectedBanners.AddRange(banners.OrderBy(x => random.Next()).Take(count).Select(t =>
                        {
                            var finalUrl = t.DestinationUrl;
                            if (t.ProductId.HasValue)
                            {
                                if (t.ProductId.HasValue && productDictionary.ContainsKey(t.ProductId.Value))
                                {
                                    finalUrl = productDictionary[t.ProductId.Value].AffiliateLink;
                                }
                                finalUrl = finalUrl.Replace("zzzzz", "medamri");
                            }

                            return new GetRandomBannerModel
                            {
                                Id = t.Id,
                                AdvertiserName = t.AdvertiserName,
                                ImageUrl = t.ImageUrl,
                                Position = t.Position,
                                Width = t.Width,
                                Height = t.Height,
                                FinalUrl = finalUrl
                            };
                        }));
                    }
                }

                AddRandomBanners("Top", 2);
                AddRandomBanners("TopRight", 2);
                AddRandomBanners("Right", 1);

                return selectedBanners;
            }
            catch (Exception ex)
            {
                await _globalHelper.Log(ex, currentClassName);
                throw new Exception("An error occurred while processing the request.", ex);
            }
        }

    }
}
