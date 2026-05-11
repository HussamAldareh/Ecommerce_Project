namespace Ecommerce_Project.Models
{
    public class Testimonial
    {
        public int Id { get; set; }

        public string Message { get; set; }

        public bool IsApproved { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string UserId { get; set; }
        public ApplicationUser? User
        {
            get; set;
        }
        }
}
