using MediatR;
using System;

namespace TWJ.TWJApp.TWJService.Application.Services.Product.Commands.Add
{
    public class AddProductCommand : IRequest<Unit>
    {
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

        public TWJ.TWJApp.TWJService.Domain.Entities.Product AddProduct()
        {
            return new TWJ.TWJApp.TWJService.Domain.Entities.Product
            {
                ProductName = ProductName,
                Description = Description,
                VendorName = "Amazon",
                CategoryId = CategoryId,
                AvgRating = AvgRating,
                Price = Price,
                Currency = "$",
                AffiliateLink = AffiliateLink,
                Image = Image,
                PromotionStart = PromotionStart,
                PromotionEnd = PromotionEnd
            };
        }
    }
}