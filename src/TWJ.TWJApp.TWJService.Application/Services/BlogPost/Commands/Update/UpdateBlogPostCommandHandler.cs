
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;

namespace TWJ.TWJApp.TWJService.Application.Services.BlogPost.Commands.Update
{
    public class UpdateBlogPostCommandHandler : IRequestHandler<UpdateBlogPostCommand, Unit>
    {
        private readonly ITWJAppDbContext _context;

        public UpdateBlogPostCommandHandler(ITWJAppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Unit> Handle(UpdateBlogPostCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var data = await _context.BlogPosts.AsNoTrackingWithIdentityResolution().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                _context.BlogPosts.Update(request.Update(data));

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