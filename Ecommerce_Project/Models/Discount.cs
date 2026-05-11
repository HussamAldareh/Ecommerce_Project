namespace Ecommerce_Project.Models
{
    public class Discount
    {

        public int Id { get; set; }

        public string Title { get; set; }

        public int Percentage { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        public int ProductId { get; set; }
        public Product Product { get; set; }
        public decimal MinmumAmount { get; set; }

    }
}
