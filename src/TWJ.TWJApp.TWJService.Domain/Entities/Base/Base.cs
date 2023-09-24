using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWJ.TWJApp.TWJService.Domain.Entities.Base
{
    public class BaseModel : BaseEntity<Guid>
    {
        public Guid Id { get; set; }
        public string Property { get; set; }
    }
}
