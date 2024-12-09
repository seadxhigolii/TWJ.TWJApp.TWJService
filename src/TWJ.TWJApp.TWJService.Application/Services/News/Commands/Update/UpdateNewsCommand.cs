using MediatR;
using System;

namespace TWJ.TWJApp.TWJService.Application.Services.News.Commands.Update
{
    public class UpdateNewsCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int NoOfPosts { get; set; }
        public bool IsUsed { get; set; }

        public Domain.Entities.News Update(Domain.Entities.News news)
        {
            news.Title = Title;
            news.Description = Description;
            news.NoOfPosts = NoOfPosts;
            news.IsUsed = IsUsed;
            return news;
        }
    }
}