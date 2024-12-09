using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Domain.Entities.Base;

namespace TWJ.TWJApp.TWJService.Domain.Entities
{
    public class Banner : BaseEntity<Guid>
    {
        #region Properties
        public string AdvertiserName { get; set; }
        public string ImageUrl { get; set; }
        public string DestinationUrl { get; set; }
        public Guid? ProductId { get; set; }
        public string AltText { get; set; }
        public string Position { get; set; }
        public int Clicks { get; set; }
        public int TimesShown { get; set; }
        public double CTR { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool Active { get; set; } = true;
        #endregion

        #region Entity Models
        public ICollection<BlogPostBanner> BlogPostBanners { get; set; }
        public ICollection<AdClick> AdClicks { get; set; }
        public Product Product { get; set; }
        #endregion

    }
}
