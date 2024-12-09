using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;

namespace TWJ.TWJApp.TWJService.Application.Services.Base.Commands.Add
{
    public class AddTemplateSettingCommandHandler : IRequestHandler<AddTemplateSettingCommand, Unit>
    {
        private readonly ITWJAppDbContext _context;

        public AddTemplateSettingCommandHandler(ITWJAppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Unit> Handle(AddTemplateSettingCommand request, CancellationToken cancellationToken)
        {
            await _context.TemplateSettings.AddAsync(request.AddTemplateSetting(), cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
