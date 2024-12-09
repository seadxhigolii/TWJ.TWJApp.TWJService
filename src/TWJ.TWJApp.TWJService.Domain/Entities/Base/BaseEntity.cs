using System;

namespace TWJ.TWJApp.TWJService.Domain.Entities.Base
{
    public class BaseEntity<TKey> : IEntityTimeStamp where TKey : IEquatable<TKey>
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DeletedAt { get; set; } = null;
        public string CreatedBy { get; set; }         
    }
}
