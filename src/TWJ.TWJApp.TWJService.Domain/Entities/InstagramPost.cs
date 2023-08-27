using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Domain.Entities.Base;

namespace TWJ.TWJApp.TWJService.Domain.Entities
{
    public class InstagramPost : BaseEntity<Guid>
    {
        public Guid BlogPostId { get; set; }
        public Byte[] ImageUrl { get; set; }

        public BlogPost BlogPost { get; set; }
        public ICollection<MidJourneyImage> MidJourneyImages { get; set; }
    }

}
