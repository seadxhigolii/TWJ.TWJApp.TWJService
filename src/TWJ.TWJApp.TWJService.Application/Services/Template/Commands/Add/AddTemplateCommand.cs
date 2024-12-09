using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Domain.Entities;
using TWJ.TWJApp.TWJService.Domain.Entities.Base;

namespace TWJ.TWJApp.TWJService.Application.Services.Template.Commands.Add
{
    public class AddTemplateCommand : IRequest<Unit>
    {
        public Guid TemplateSettingId { get; set; }
        public string DisplayText { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
        public Guid? ParentId { get; set; }

        public TWJ.TWJApp.TWJService.Domain.Entities.Template AddTemplate()
        {
            return new TWJ.TWJApp.TWJService.Domain.Entities.Template
            {
                TemplateSettingId = TemplateSettingId,
                DisplayText = DisplayText,
                Description = Description,
                IsActive = IsActive,
                IsDefault = IsDefault,
                ParentId = ParentId

            };
        }
    }
}
