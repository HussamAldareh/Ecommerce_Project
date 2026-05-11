namespace Ecommerce_Project.Models
{
    public class Order
    {
        public int Id { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = "Processing";

        public string ShippingAddress { get; set; }

        public string UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public ICollection<OrderItem>? OrderItems { get; set; }

        public Payment? Payment { get; set; }
    }

}
