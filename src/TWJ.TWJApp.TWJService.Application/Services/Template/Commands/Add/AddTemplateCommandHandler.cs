using MediatR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Application.Services.Base.Commands.Add;
using TWJ.TWJApp.TWJService.Domain.Entities;

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
            try
            {
                await _context.Templates.AddAsync(request.AddTemplate(), cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
            catch (Exception ex)
            {
                var stackTrace = new StackTrace();
                var stackFrame = stackTrace.GetFrame(0);
                Log log = new Log()
                {
                    Class = this.GetType().Name,
                    Method = stackFrame.GetMethod().ToString(),
                    Message = ex.Message,
                    CreatedAt = DateTime.Now
                };
                await _context.Logs.AddAsync(log);
                await _context.SaveChangesAsync();
                throw new Exception("An error occurred while processing the request.", ex);
            }
        }
    }
}
