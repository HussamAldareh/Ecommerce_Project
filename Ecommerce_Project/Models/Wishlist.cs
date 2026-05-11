namespace Ecommerce_Project.Models
{
    public class Wishlist
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public ICollection<WishlistItem>? WishlistItems { get; set; }
    }


}
