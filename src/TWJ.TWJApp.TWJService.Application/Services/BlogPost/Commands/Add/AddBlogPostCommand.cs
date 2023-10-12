
using MediatR;
using System;
using TWJ.TWJApp.TWJService.Domain.Entities;

namespace TWJ.TWJApp.TWJService.Application.Services.BlogPost.Commands.Add
{
    public class AddBlogPostCommand : IRequest<Unit>
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public Guid UserId { get; set; }
        public Guid CategoryId { get; set; }
        public string Tags { get; set; }
        public Byte[] Image { get; set; }
        public int Views { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public int NumberOfComments { get; set; }
        public Guid? ProductID { get; set; }

        public TWJ.TWJApp.TWJService.Domain.Entities.BlogPost AddBlogPost()
        {
            return new TWJ.TWJApp.TWJService.Domain.Entities.BlogPost
            {
                Title = Title,
                Content = Content,
                UserId = UserId,
                CategoryId = CategoryId,
                Tags = Tags,
                Image = Image,
                Views = Views,
                Likes = Likes,
                Dislikes = Dislikes,
                NumberOfComments = NumberOfComments,
                ProductID = ProductID
            };
        }
    }
}