using MediatR;
using System;
using TWJ.TWJApp.TWJService.Domain.Entities;

namespace TWJ.TWJApp.TWJService.Application.Services.Product.Commands.Update
{
    public class UpdateProductCommand : IRequest<Unit>
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

        public TWJ.TWJApp.TWJService.Domain.Entities.Product Update(TWJ.TWJApp.TWJService.Domain.Entities.Product product)
        {
            product.Id = Id;
            product.ProductName = ProductName;
            product.Description = Description;
            product.VendorName = VendorName;
            product.CategoryId = CategoryId;
            product.AvgRating = AvgRating;
            product.Price = Price;
            product.Currency = Currency;
            product.AffiliateLink = AffiliateLink;
            product.Image = Image;
            product.PromotionStart = PromotionStart;
            product.PromotionEnd = PromotionEnd;
            return product;
        }
    }
}