using System;
using TWJ.TWJApp.TWJService.Domain.Entities.Base;

namespace TWJ.TWJApp.TWJService.Domain.Entities
{
    public class UserRole : BaseEntity<Guid>
    {
        #region Properties
        public Guid UserID { get; set; }
        public Guid RoleID { get; set; }
        #endregion

        #region Entity Models
        public User User { get; set; }
        public Role Role { get; set; }
        #endregion
    }
}
