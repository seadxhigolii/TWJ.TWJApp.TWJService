
using MediatR;
using System;

namespace TWJ.TWJApp.TWJService.Application.Services.BlogPost.Commands.Update
{
    public class UpdateBlogPostCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public Guid CategoryId { get; set; }
        public string Tags { get; set; }
        public Byte[] Image { get; set; }
        public int Views { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public int NumberOfComments { get; set; }
        public Guid? ProductID { get; set; }

        public TWJ.TWJApp.TWJService.Domain.Entities.BlogPost Update(TWJ.TWJApp.TWJService.Domain.Entities.BlogPost blogpost)
        {
            blogpost.Title = Title;
            blogpost.Content = Content;
            blogpost.CategoryId = CategoryId;
            blogpost.Tags = Tags;
            blogpost.Image = Image;
            blogpost.Likes = Likes;
            blogpost.NumberOfComments = NumberOfComments;
            blogpost.Views = Views;
            blogpost.Dislikes = Dislikes;
            blogpost.ProductID = ProductID;
            return blogpost;
        }
    }
}