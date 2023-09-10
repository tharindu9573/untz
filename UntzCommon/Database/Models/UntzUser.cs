using Microsoft.AspNetCore.Identity;

namespace UntzCommon.Database.Models
{
    public class UntzUser : IdentityUser
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
    }
}
