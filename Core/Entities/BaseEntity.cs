using System;

namespace Core.Entities
{
    public class BaseEntity : BaseEntity<int>
    {
    }

    public class BaseEntity<T> : IEntity
    {
        public virtual T Id { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public DateTime? DeletedDate { get; set; }

        public int? CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }

        public int? DeletedBy { get; set; }

        public bool? IsActive { get; set; } = true;

        public bool? IsDeleted { get; set; } = false;
    }
}
