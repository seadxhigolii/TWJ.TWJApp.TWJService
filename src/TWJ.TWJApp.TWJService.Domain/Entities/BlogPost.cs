using System;
using TWJ.TWJApp.TWJService.Domain.Entities.Base;

namespace TWJ.TWJApp.TWJService.Domain.Entities
{
    public class BlogPost : BaseEntity<Guid>
    {
        #region Properties
        public string Title { get; set; }
        public string Content { get; set; }
        public Guid UserId { get; set; }
        public Guid CategoryId { get; set; }
        public string Tags { get; set; }
        public Byte[] Image { get; set; }
        public int Views { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public int Comments { get; set; }
        public Guid? ProductID { get; set; }

        #endregion

        #region Entity Models
        public Product Product { get; set; }
        public Category Category { get; set; }
        public User User { get; set; }
        #endregion

    }
}
