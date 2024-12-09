using System;
using System.Collections.Generic;
using TWJ.TWJApp.TWJService.Domain.Entities.Base;

namespace TWJ.TWJApp.TWJService.Domain.Entities
{
    public class Tag : BaseEntity<Guid>
    {
        #region Properties
        public string Name { get; set; }
        public int PostCount { get; set; }
        #endregion

        #region Entity Models
        public ICollection<BlogPostTags> BlogPostTags { get; set; }
        #endregion

    }
}
