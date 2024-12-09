using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;

namespace TWJ.TWJApp.TWJService.Application.Services.Base.Commands.Add
{
    public class AddBaseCommandHandler : IRequestHandler<AddBaseCommand, Unit>
    {
        private readonly ITWJAppDbContext _context;
        public AddBaseCommandHandler(ITWJAppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Unit> Handle(AddBaseCommand request, CancellationToken cancellationToken)
        {
            await _context.Base.AddAsync(request.AddBase(), cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
