using System;
using System.Collections.Generic;
using TWJ.TWJApp.TWJService.Domain.Entities.Base;

namespace TWJ.TWJApp.TWJService.Domain.Entities
{
    public class BlogPost : BaseEntity<Guid>
    {
        #region Properties
        public string Title { get; set; }
        public string Content { get; set; }
        public string SEOKeywords { get; set; }
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
        public string BackLinkKeywords { get; set; }
        public string URL{ get; set; }
        public bool Published{ get; set; }

        #endregion

        #region Entity Models
        public Product Product { get; set; }
        public Category Category { get; set; }
        public BlogPostCategory BlogPostCategory { get; set; }
        public User User { get; set; }
        public ICollection<BlogPostTags> BlogPostTags { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<BlogPostSEOKeyword> BlogPostSEOKeywords { get; set; }
        public ICollection<AdClick> AdClicks { get; set; }

        #endregion

    }
}
