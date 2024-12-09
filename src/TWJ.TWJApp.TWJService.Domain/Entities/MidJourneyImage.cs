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
        #region Properties
        public Byte[] Image { get; set; }
        #endregion

    }

}
