using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Application.Services.BlogPost.Commands.Add;

namespace TWJ.TWJApp.TWJService.Application.Services.BlogPost.Commands.Generate
{
    public class GenerateBlogPostCommandHandler : IRequestHandler<GenerateBlogPostCommand, string>
    {
        private readonly ITWJAppDbContext _context;
        private readonly IOpenAiService _openAiService;

        public GenerateBlogPostCommandHandler(ITWJAppDbContext context, IOpenAiService openAiService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _openAiService = openAiService ?? throw new ArgumentNullException(nameof(openAiService));
        }

        public async Task<string> Handle(GenerateBlogPostCommand request, CancellationToken cancellationToken)
        {
            try
            {
                //return await _openAiService.GenerateSEOFocusedBlogPostAsync(request, cancellationToken); ;
                return "";
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
