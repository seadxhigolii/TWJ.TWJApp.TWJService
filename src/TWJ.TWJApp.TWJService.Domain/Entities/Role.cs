using System;
using System.Collections.Generic;
using TWJ.TWJApp.TWJService.Domain.Entities.Base;

namespace TWJ.TWJApp.TWJService.Domain.Entities
{
    public class Role : BaseEntity<Guid>
    {
        #region Properties
        public string Name { get; set; }
        #endregion

        #region Entity Models
        public ICollection<UserRole> UserRoles { get; set; }

        #endregion
    }
}
