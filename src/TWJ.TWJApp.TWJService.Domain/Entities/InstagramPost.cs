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
        #region Properties
        public string Image { get; set; }
        public int Type { get; set; }
        public bool IsVideo { get; set; }
        #endregion
    }

}
