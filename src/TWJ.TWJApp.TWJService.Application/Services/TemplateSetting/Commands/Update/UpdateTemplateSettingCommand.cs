using MediatR;
using System;
using TWJ.TWJApp.TWJService.Domain.Entities;
using TWJ.TWJApp.TWJService.Domain.Entities.Base;

namespace TWJ.TWJApp.TWJService.Application.Services.Base.Commands.Update
{
    public class UpdateTemplateSettingCommand : IRequest<Unit>
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public Guid? DependOn { get; set; }

        public TemplateSetting UpdateTemplateSetting(TemplateSetting templateSetting)
        {
            templateSetting.Name = Name;
            templateSetting.Description = Description;
            templateSetting.DependOn = DependOn;
            return templateSetting;
        }
    }
}
