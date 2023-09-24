using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Application.Services.Base.Commands.Add;

namespace TWJ.TWJApp.TWJService.Application.Services.Template.Commands.Add
{
    public class AddTemplateCommandHandler : IRequestHandler<AddTemplateCommand, Unit>
    {
        private readonly ITWJAppDbContext _context;
        public readonly string Name;

        public AddTemplateCommandHandler(ITWJAppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Unit> Handle(AddTemplateCommand request, CancellationToken cancellationToken)
        {
            await _context.Template.AddAsync(request.AddTemplate(), cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
