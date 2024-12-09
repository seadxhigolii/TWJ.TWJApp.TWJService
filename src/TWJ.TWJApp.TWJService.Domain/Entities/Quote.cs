using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Domain.Entities.Base;

namespace TWJ.TWJApp.TWJService.Domain.Entities
{
    public class Quote : BaseEntity<Guid>
    {
        public string Content { get; set; }
        public string Author { get; set; }
        public string Category { get; set; }
        public int Length { get; set; }
    }
}
