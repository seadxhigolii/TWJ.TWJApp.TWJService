using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;

namespace TWJ.TWJApp.TWJService.Application.Services.Base.Commands.Update
{
    public class UpdateBaseCommandHandler : IRequestHandler<UpdateBaseCommand, Unit>
    {
        private readonly ITWJAppDbContext _context;

        public UpdateBaseCommandHandler(ITWJAppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Unit> Handle(UpdateBaseCommand request, CancellationToken cancellationToken)
        {
            var data = await _context.Base.AsNoTrackingWithIdentityResolution().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            _context.Base.Update(request.Update(data));

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
