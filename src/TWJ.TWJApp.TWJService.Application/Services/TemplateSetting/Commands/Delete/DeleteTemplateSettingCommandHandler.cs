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
    public class DeleteTemplateSettingCommandHandler : IRequestHandler<DeleteTemplateSettingCommand, Unit>
    {
        private readonly ITWJAppDbContext _context;
        public DeleteTemplateSettingCommandHandler(ITWJAppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Unit> Handle(DeleteTemplateSettingCommand request, CancellationToken cancellationToken)
        {
            var templateSetting = await _context.TemplateSetting.AsNoTrackingWithIdentityResolution().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (templateSetting == null) throw new BadRequestException(ValidatorMessages.NotFound("TemplateSetting"));

            _context.TemplateSetting.Remove(templateSetting);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
