
using MediatR;
using System;

namespace TWJ.TWJApp.TWJService.Application.Services.BlogPost.Commands.Update
{
    public class UpdateBlogPostCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public TWJ.TWJApp.TWJService.Domain.Entities.BlogPost Update(TWJ.TWJApp.TWJService.Domain.Entities.BlogPost blogpost)
        {
            blogpost.Title = Title;
            blogpost.Content = Content;
            return blogpost;
        }
    }
}