using System;
using TWJ.TWJApp.TWJService.Domain.Entities.Base;

namespace TWJ.TWJApp.TWJService.Domain.Entities
{
    public class BlogPostTags : BaseEntity<Guid>
    {
        #region Properties
        public Guid BlogPostID { get; set; }
        public Guid TagID { get; set; }
        #endregion

        #region Entity Models
        public Tag Tag { get; set; }
        public BlogPost BlogPost { get; set; }
        #endregion
    }
}
