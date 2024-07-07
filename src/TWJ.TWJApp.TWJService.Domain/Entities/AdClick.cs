using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Domain.Entities.Base;

namespace TWJ.TWJApp.TWJService.Domain.Entities
{
    public class AdClick : BaseEntity<Guid>
    {
        #region Properties
        public Guid ProductId { get; set; }
        public Guid BlogPostId { get; set; }
        public Guid BannerId { get; set; }
        public DateTime ClickTime { get; set; }
        public string UserSessionId { get; set; } 
        public string URL { get; set; } 
        public bool Converted { get; set; }
        #endregion

        #region Entities
        public Product Product { get; set; }
        public BlogPost BlogPost { get; set; }
        public Banner Banner { get; set; }
        #endregion Entities
    }
}
