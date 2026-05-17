using Microsoft.AspNetCore.Identity;

namespace Ecommerce_Project.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }

        public string? Phone { get; set; }
    }
}
