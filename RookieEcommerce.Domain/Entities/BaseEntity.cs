namespace RookieEcommerce.Domain.Entities
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }

        public void UpdateModifiedDate()
        {
            ModifiedDate = DateTime.UtcNow;
        }
    }
}