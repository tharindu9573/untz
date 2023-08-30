using Microsoft.AspNetCore.Identity;

namespace Untz.Database.Models
{
    public class UntzUser : IdentityUser
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
    }
}
