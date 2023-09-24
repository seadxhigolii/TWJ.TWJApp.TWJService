using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;

namespace TWJ.TWJApp.TWJService.Application.Services.Template.Commands.Update
{
    public class UpdateTemplateCommandHandler : IRequestHandler<UpdateTemplateCommand, Unit>
    {
        private readonly ITWJAppDbContext _context;

        public UpdateTemplateCommandHandler(ITWJAppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Unit> Handle(UpdateTemplateCommand request, CancellationToken cancellationToken)
        {
            var data = await _context.Template.AsNoTrackingWithIdentityResolution().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            _context.Template.Update(request.UpdateTemplate(data));

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
