namespace Ecommerce_Project.Models
{
    public class ProductRating
    {
        public int Id { get; set; }

        public int RatingValue { get; set; }

        public bool IsApproved { get; set; } = false;

        public string UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public int ProductId { get; set; }
        public Product? Product
        {
            get; set;
        }
        }
}   
