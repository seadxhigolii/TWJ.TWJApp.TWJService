using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;

namespace TWJ.TWJApp.TWJService.Application.Services.Product.Commands.Update
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Unit>
    {
        private readonly ITWJAppDbContext _context;

        public UpdateProductCommandHandler(ITWJAppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var data = await _context.Products.AsNoTrackingWithIdentityResolution().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                _context.Products.Update(request.Update(data));

                await _context.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}