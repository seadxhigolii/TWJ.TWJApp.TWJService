using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWJ.TWJApp.TWJService.Application.Services.Template.Commands.Delete
{
    public class DeleteTemplateCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
