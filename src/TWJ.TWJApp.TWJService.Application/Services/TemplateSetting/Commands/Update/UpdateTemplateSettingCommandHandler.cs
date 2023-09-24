using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;

namespace TWJ.TWJApp.TWJService.Application.Services.Base.Commands.Update
{
    public class UpdateTemplateSettingCommandHandler : IRequestHandler<UpdateTemplateSettingCommand, Unit>
    {
        private readonly ITWJAppDbContext _context;

        public UpdateTemplateSettingCommandHandler(ITWJAppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Unit> Handle(UpdateTemplateSettingCommand request, CancellationToken cancellationToken)
        {
            var data = await _context.TemplateSetting.AsNoTrackingWithIdentityResolution().FirstOrDefaultAsync(x => x.Name == request.Name, cancellationToken);

            _context.TemplateSetting.Update(request.UpdateTemplateSetting(data));

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
