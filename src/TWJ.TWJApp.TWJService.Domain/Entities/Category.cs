using System;
using TWJ.TWJApp.TWJService.Domain.Entities.Base;

namespace TWJ.TWJApp.TWJService.Domain.Entities
{
    public class Category : BaseEntity<Guid>
    {
        #region Properites
        public string Name { get; set; }
        public string Description { get; set; }
        #endregion
    }
}
