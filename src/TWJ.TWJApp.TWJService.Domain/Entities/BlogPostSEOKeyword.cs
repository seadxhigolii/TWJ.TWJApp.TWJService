using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Domain.Entities.Base;

namespace TWJ.TWJApp.TWJService.Domain.Entities
{
    public class BlogPostSEOKeyword : BaseEntity<Guid>
    {
        #region Properties
        public Guid BlogPostID { get; set; }
        public Guid SEOKeywordID { get; set; }
        public double Score { get; set; }

        #endregion

        #region Entity Models
        public BlogPost BlogPost { get; set; }
        public SEOKeyword SEOKeyword { get; set; }
        #endregion
    }
}
