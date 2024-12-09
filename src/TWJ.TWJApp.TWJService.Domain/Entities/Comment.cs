using System;
using System.Collections.Generic;
using TWJ.TWJApp.TWJService.Domain.Entities.Base;

namespace TWJ.TWJApp.TWJService.Domain.Entities
{
    public class Comment : BaseEntity<Guid>
    {
        #region Properties
        public string Content { get; set; }
        public string Email { get; set; }
        public Guid BlogPostID { get; set; }
        public string UserID { get; set; }
        #endregion

        #region Entity Models
        public BlogPost BlogPost { get; set; }
        public User User { get; set; }
        public ICollection<CommentLike> Likes { get; set; }
        public ICollection<CommentDislike> Dislikes { get; set; }
        public ICollection<CommentReply> Replies { get; set; }
        #endregion
    }
}
