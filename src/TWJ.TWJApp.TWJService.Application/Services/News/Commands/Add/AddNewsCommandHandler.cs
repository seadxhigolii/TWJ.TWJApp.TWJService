using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Application.Helpers.Interfaces;
using TWJ.TWJApp.TWJService.Domain.Entities;

namespace TWJ.TWJApp.TWJService.Application.Services.News.Commands.Add
{
    public class AddNewsCommandHandler : IRequestHandler<AddNewsCommand, Unit>
    {
        private readonly ITWJAppDbContext _context;
        private readonly IGlobalHelperService _globalHelper;
        private readonly IPreplexityService _preplexityService;
        private readonly string currentClassName = "";

        public AddNewsCommandHandler(ITWJAppDbContext context, IGlobalHelperService globalHelper, IPreplexityService preplexityService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _globalHelper = globalHelper ?? throw new ArgumentNullException(nameof(globalHelper));
            currentClassName = GetType().Name;
            _preplexityService = preplexityService;
        }

        public async Task<Unit> Handle(AddNewsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var newsDescription = await _preplexityService.GenerateContentAsync($"Browse the recent news and give me all the information on: '{request.Title}'. " +
                       $"No introductions, no 'sure' in the beginning, no greetings, no reiterations of the title, no use of words " +
                       $"like 'Here are some...' or 'Here is...' or something like this in the beginning. Just the facts.");
                
                var newsRecord = new Domain.Entities.News
                {
                    Title = request.Title,
                    Description = newsDescription
                };

                await _context.News.AddAsync(newsRecord);
                await _context.SaveChangesAsync();

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