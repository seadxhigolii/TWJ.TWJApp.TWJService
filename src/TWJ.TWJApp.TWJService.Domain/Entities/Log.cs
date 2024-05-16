using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Domain.Entities.Base;

namespace TWJ.TWJApp.TWJService.Domain.Entities
{
    public class Log : BaseEntity<Guid>
    {
        public string Class { get; set; }
        public string Method { get; set; }
        public string Message { get; set; }
    }
}
