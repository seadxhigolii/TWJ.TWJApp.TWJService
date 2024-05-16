using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Helpers.Interfaces;
using TWJ.TWJApp.TWJService.Application.Interfaces;

namespace TWJ.TWJApp.TWJService.Application.Services.SEOKeyword.Commands.Add
{
    public class AddSEOKeywordCommandHandler : IRequestHandler<AddSEOKeywordCommand, Unit>
    {
        private readonly ITWJAppDbContext _context;
        private readonly IGlobalHelperService _globalHelper;
        private readonly string currentClassName = "";

        public AddSEOKeywordCommandHandler(ITWJAppDbContext context, IGlobalHelperService globalHelper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _globalHelper = globalHelper ?? throw new ArgumentNullException(nameof(globalHelper));
            currentClassName = GetType().Name;
        }

        public async Task<Unit> Handle(AddSEOKeywordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _context.SEOKeywords.AddAsync(request.AddSEOKeyword(), cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
            catch (Exception ex)
            {
                await _globalHelper.Log(ex, currentClassName);
                throw new Exception("An error occurred while processing the request.", ex);
            }
        }
    }
}