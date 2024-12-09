using System;
using TWJ.TWJApp.TWJService.Domain.Entities.Base;

namespace TWJ.TWJApp.TWJService.Domain.Entities
{
    public class UserSettings : BaseEntity<Guid>
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
