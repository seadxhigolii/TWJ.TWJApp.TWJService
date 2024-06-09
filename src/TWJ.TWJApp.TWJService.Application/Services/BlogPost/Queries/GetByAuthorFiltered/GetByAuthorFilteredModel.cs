using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWJ.TWJApp.TWJService.Application.Services.BlogPost.Queries.GetByAuthorFiltered
{
    public class GetByAuthorFilteredModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public Guid UserId { get; set; }
        public Guid ProductCategoryId { get; set; }
        public Guid BlogPostCategoryId { get; set; }
        public string Tags { get; set; }
        public string Image { get; set; }
        public int Views { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public int NumberOfComments { get; set; }
        public Guid? ProductID { get; set; }
        public string AuthorName { get; set; }
        public string AuthorImage { get; set; }
        public string URL { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
