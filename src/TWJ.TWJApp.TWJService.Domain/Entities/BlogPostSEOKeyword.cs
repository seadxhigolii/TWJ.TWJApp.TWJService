using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWJ.TWJApp.TWJService.Domain.Entities
{
    public class BlogPostSEOKeyword
    {
        #region Properties
        public Guid BlogPostID { get; set; }
        public Guid SEOKeywordID { get; set; }
        #endregion

        #region Entity Models
        public BlogPost BlogPost { get; set; }
        public SEOKeyword SEOKeyword { get; set; }
        #endregion
    }
}
