namespace Ecommerce_Project.Models
{
    public class Payment
    {


        public int Id { get; set; }

        public string PaymentMethod { get; set; }

        public string PaymentStatus { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.Now;

        public int OrderId { get; set; }
        public Order? Order { get; set; }
    



}
}
