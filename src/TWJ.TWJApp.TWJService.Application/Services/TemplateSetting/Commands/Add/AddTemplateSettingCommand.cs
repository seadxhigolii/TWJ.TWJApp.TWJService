using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Domain.Entities;
using TWJ.TWJApp.TWJService.Domain.Entities.Base;

namespace TWJ.TWJApp.TWJService.Application.Services.Base.Commands.Add
{
    public class AddTemplateSettingCommand : IRequest<Unit>
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public Guid? DependOn { get; set; }

        public TemplateSetting AddTemplateSetting()
        {
            return new TemplateSetting
            {
                Name = Name,
                Description = Description,
                DependOn = DependOn
            };
        }
    }
}
