namespace Ecommerce_Project.Models
{
    public class Product
    {

        public int Id { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }
        public bool IsAvailable { get; set; } = true;

        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public ICollection<ProductImage>? ProductImages { get; set; }

        public ICollection<ProductComment>? ProductComments { get; set; }

        public ICollection<ProductRating>? ProductRatings { get; set; }

        public ICollection<OrderItem>? OrderItems { get; set; }

        public ICollection<CartItem>? CartItems { get; set; }

        public ICollection<WishlistItem>? WishlistItems { get; set; }

        public ICollection<Discount>? Discounts { get; set; }
    


}
}
