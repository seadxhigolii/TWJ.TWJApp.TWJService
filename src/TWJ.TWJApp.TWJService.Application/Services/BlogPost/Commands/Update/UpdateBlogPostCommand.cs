
using MediatR;
using System;

namespace TWJ.TWJApp.TWJService.Application.Services.BlogPost.Commands.Update
{
    public class UpdateBlogPostCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public Guid ProductCategoryId { get; set; }
        public Guid BlogPostCategoryId { get; set; }
        public string Tags { get; set; }
        public string Image { get; set; }
        public int Views { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public int NumberOfComments { get; set; }
        public Guid? ProductID { get; set; }

        public TWJ.TWJApp.TWJService.Domain.Entities.BlogPost Update(TWJ.TWJApp.TWJService.Domain.Entities.BlogPost blogpost)
        {
            blogpost.Title = Title;
            blogpost.Content = Content;
            blogpost.ProductCategoryId = ProductCategoryId;
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