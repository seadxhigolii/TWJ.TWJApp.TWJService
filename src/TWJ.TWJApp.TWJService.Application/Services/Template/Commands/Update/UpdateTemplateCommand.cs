using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Domain.Entities;
using TWJ.TWJApp.TWJService.Domain.Entities.Base;

namespace TWJ.TWJApp.TWJService.Application.Services.Template.Commands.Update
{
    public class UpdateTemplateCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public Guid TemplateSettingId { get; set; }
        public string DisplayText { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
        public Guid? ParentId { get; set; }

        public TWJ.TWJApp.TWJService.Domain.Entities.Template UpdateTemplate(TWJ.TWJApp.TWJService.Domain.Entities.Template template)
        {
            template.TemplateSettingId = TemplateSettingId;
            template.DisplayText = DisplayText;
            template.Description = Description;
            template.IsActive = IsActive;
            template.IsDefault = IsDefault;
            template.ParentId = ParentId;
            return template;
        }
    }
}
