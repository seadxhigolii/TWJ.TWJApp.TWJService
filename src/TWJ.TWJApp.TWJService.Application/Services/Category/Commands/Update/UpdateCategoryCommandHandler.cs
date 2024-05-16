using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;

namespace TWJ.TWJApp.TWJService.Application.Services.Category.Commands.Update
{
    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Unit>
    {
        private readonly ITWJAppDbContext _context;

        public UpdateCategoryCommandHandler(ITWJAppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Unit> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var data = await _context.ProductCategories.AsNoTrackingWithIdentityResolution().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            _context.ProductCategories.Update(request.Update(data));

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}