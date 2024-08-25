using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Domain.Entities.Base;

namespace TWJ.TWJApp.TWJService.Domain.Entities
{
    public class Graph : BaseEntity<Guid>
    {
        public string Title { get; set; }
        public int Type { get; set; }
        public bool IsUsed { get; set; }
    }
}
