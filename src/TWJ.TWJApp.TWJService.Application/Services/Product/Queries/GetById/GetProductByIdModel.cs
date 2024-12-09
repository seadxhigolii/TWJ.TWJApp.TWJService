using MapperSegregator.Interfaces;
using System;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Services.Product.Queries.GetAll;

namespace TWJ.TWJApp.TWJService.Application.Services.Product.Queries.GetById
{
    public class GetProductByIdModel : IProfile
    {
        public Guid Id { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public string VendorName { get; set; }
        public Guid CategoryId { get; set; }
        public decimal AvgRating { get; set; }
        public int TotalRatings { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public string AffiliateLink { get; set; }
        public string Image { get; set; }
        public DateTime PromotionStart { get; set; }
        public DateTime PromotionEnd { get; set; }

        public async Task MapData(IProfileMapper profileMapper)
        {
            profileMapper.Build<TWJ.TWJApp.TWJService.Domain.Entities.Product, GetProductByIdModel>(
                (src, options) =>
                {
                    return new GetProductByIdModel
                    {
                        Id = src.Id,
                        ProductName = src.ProductName,
                        Description = src.Description,
                        VendorName = src.VendorName,
                        CategoryId = src.CategoryId,
                        AvgRating = src.AvgRating,
                        Price = src.Price,
                        Currency = src.Currency,
                        AffiliateLink = src.AffiliateLink,
                        Image = src.Image,
                        PromotionStart = src.PromotionStart,
                        PromotionEnd = src.PromotionEnd
                    };
                });
        }
    }
}