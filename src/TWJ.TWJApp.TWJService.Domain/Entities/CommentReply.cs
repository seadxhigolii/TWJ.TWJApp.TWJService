using System;
using TWJ.TWJApp.TWJService.Domain.Entities.Base;

namespace TWJ.TWJApp.TWJService.Domain.Entities
{
    public class CommentReply : BaseEntity<Guid>
    {
        #region Properties
        public string Content { get; set; }
        public Guid CommentID { get; set; }
        public Guid UserID { get; set; }

        #endregion

        #region Entity Models
        public User User { get; set; }
        public Comment Comment { get; set; }
        #endregion
    }
}
