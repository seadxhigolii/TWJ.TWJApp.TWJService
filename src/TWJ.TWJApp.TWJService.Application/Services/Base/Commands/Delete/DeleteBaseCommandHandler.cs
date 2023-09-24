using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Common.Constants;
using TWJ.TWJApp.TWJService.Common.Exceptions;

namespace TWJ.TWJApp.TWJService.Application.Services.Base.Commands.Delete
{
    public class DeleteBaseCommandHandler : IRequestHandler<DeleteBaseCommand, Unit>
    {
        private readonly ITWJAppDbContext _context;
        public DeleteBaseCommandHandler(ITWJAppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Unit> Handle(DeleteBaseCommand request, CancellationToken cancellationToken)
        {
            var data = await _context.Base.AsNoTrackingWithIdentityResolution().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (data == null) throw new BadRequestException(ValidatorMessages.NotFound("Record"));

            _context.Base.Remove(data);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
