using Microsoft.AspNetCore.Identity;

namespace Ecommerce_Project.Models
{
    public class ApplicationUser : IdentityUser
    {


        public string FullName { get; set; }

        public string  Email { get; set; }
        public int  Phone  { get; set; }

    }
}
