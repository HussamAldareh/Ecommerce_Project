namespace Ecommerce_Project.Models
{
    public class ReviewItemViewModel
    {
        public int CommentId { get; set; }

        public string? CommentText { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsApproved { get; set; }

        public string? CustomerName { get; set; }

        public string? CustomerEmail { get; set; }

        public string? ProductName { get; set; }
    }
}