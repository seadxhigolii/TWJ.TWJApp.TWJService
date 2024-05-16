using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Domain.Entities.Base;

namespace TWJ.TWJApp.TWJService.Domain.Entities
{
    public class BlogPostCategory : BaseEntity<Guid>
    {
        #region Properties
        public string Name { get; set; }
        public string Description { get; set; }
        public string URL { get; set; }
        #endregion

        #region Entity Models
        public ICollection<BlogPost> BlogPosts { get; set; }

        #endregion

    }
}
