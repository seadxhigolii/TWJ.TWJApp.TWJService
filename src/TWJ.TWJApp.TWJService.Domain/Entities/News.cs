using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Domain.Entities.Base;

namespace TWJ.TWJApp.TWJService.Domain.Entities
{
    public class News : BaseEntity<Guid>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string URL { get; set; }
        public string Tags { get; set; }
        public bool IsUsed { get; set; } = false;
        public bool IsUsedInstagram { get; set; } = false;
        public bool Active { get; set; } = true;
        public int NoOfPosts { get; set; } = 0;
        public DateTime ReleaseDate { get; set; }
    }
}
