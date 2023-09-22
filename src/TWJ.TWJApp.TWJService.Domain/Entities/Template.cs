using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Domain.Entities.Base;

namespace TWJ.TWJApp.TWJService.Domain.Entities
{
    public class Template : BaseEntity<Guid>
    {
        #region Properties
        public Guid TemplateSettingId { get; set; }
        public string DisplayText { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
        public int? ParentId { get; set; }
        #endregion

        #region Entity Models

        public TemplateSetting TemplateSetting { get; set; }

        #endregion
    }
}
