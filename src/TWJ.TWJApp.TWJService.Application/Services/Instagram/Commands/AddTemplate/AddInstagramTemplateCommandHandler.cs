using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Application.Services.BlogPost.Commands.Add;

namespace TWJ.TWJApp.TWJService.Application.Services.Instagram.Commands.AddTemplate
{
    public class AddInstagramTemplateCommandHandler : IRequestHandler<AddInstagramTemplateCommand, Unit>
    {
        private readonly ITWJAppDbContext _context;

        public AddInstagramTemplateCommandHandler(ITWJAppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Unit> Handle(AddInstagramTemplateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _context.InstagramPosts.AddAsync(request.AddInstagramTemplate(), cancellationToken);
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
