using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Domain.Entities.Base;

namespace TWJ.TWJApp.TWJService.Domain.Entities
{
    public class PinterestPinKeywords : BaseEntity<Guid>
    {
        #region Properties

        public Guid PinterestPinId { get; set; }
        public string Keyword { get; set; }
        #endregion

        #region Entity Models
        public PinterestPin PinterestPin { get; set; }
        #endregion

    }

}
