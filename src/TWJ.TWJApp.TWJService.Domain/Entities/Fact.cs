using System;
using TWJ.TWJApp.TWJService.Domain.Entities.Base;

namespace TWJ.TWJApp.TWJService.Domain.Entities
{
    public class Fact : BaseEntity<Guid>
    {
        public string Content { get; set; }
        public string Category { get; set; }
    }
}
