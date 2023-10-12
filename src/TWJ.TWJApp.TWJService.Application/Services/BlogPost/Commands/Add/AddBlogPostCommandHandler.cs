using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;

namespace TWJ.TWJApp.TWJService.Application.Services.BlogPost.Commands.Add
{
    public class AddBlogPostCommandHandler : IRequestHandler<AddBlogPostCommand, Unit>
    {
        private readonly ITWJAppDbContext _context;

        public AddBlogPostCommandHandler(ITWJAppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Unit> Handle(AddBlogPostCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _context.BlogPost.AddAsync(request.AddBlogPost(), cancellationToken);
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