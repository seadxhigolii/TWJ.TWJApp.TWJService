using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Domain.Entities.Base;

namespace TWJ.TWJApp.TWJService.Domain.Entities
{
    public class MidJourneyImage : BaseEntity<Guid>
    {
        public Guid PinterestPinId { get; set; }
        public Byte[] ImageUrl { get; set; }

        public PinterestPin PinterestPin { get; set; }
    }

}
