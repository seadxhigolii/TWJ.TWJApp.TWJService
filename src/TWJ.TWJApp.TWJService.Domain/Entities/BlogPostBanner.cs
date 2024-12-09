using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Domain.Entities.Base;

namespace TWJ.TWJApp.TWJService.Domain.Entities
{
    public class BlogPostBanner : BaseEntity<Guid>
    {
        public Guid BlogPostId { get; set; }
        public Guid BannerId { get; set; }

        public BlogPost BlogPost { get; set; }
        public Banner Banner { get; set; }
    }
}
