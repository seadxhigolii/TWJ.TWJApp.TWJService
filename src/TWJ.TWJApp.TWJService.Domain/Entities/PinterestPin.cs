using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Domain.Entities.Base;

namespace TWJ.TWJApp.TWJService.Domain.Entities
{
    public class PinterestPin : BaseEntity<Guid>
    {
        #region Properties
        public Byte[] Image { get; set; }
        #endregion

        #region Entity Models
        public ICollection<PinterestPinKeywords> PinterestPinKeywords { get; set; }

        #endregion
    }

}
