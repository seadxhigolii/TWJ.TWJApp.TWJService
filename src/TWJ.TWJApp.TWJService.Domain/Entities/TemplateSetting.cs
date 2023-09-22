using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Domain.Entities.Base;

namespace TWJ.TWJApp.TWJService.Domain.Entities
{
    public class TemplateSetting : BaseEntity<Guid>
    {
        #region Properties
        public string Name { get; set; }
        public string? Description { get; set; }
        public int? DependOn { get; set; }
        #endregion

        #region Entity Models
        public List<Template> TemplateList { get; set; }
        #endregion
    }
}
