using System;
using System.Collections.Generic;
using TWJ.TWJApp.TWJService.Domain.Entities.Base;

namespace TWJ.TWJApp.TWJService.Domain.Entities
{
    public class Product : BaseEntity<Guid>
    {
        #region Properties
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
        public bool Active { get; set; } = true;

        #endregion

        #region Entity Models
        public ICollection<BlogPost> BlogPosts { get; set; }
        public ICollection<AdClick> AdClicks { get; set; }
        public ICollection<FeaturedAdClick> FeaturedAdClicks { get; set; }
        public ICollection<Banner> Banners { get; set; }
        public Category Category { get; set; }     

        #endregion
    }
}
