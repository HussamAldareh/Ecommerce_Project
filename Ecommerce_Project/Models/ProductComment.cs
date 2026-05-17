namespace Ecommerce_Project.Models
{
    public class ProductComment
    {
        public int Id { get; set; }

        public string Comment { get; set; }

        public bool IsApproved { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
