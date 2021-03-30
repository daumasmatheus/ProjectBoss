using Microsoft.AspNetCore.Identity;
using System;

namespace ProjectBoss.Domain.Extensions
{
    public class ApplicationUser : IdentityUser
    {
        public string Provider { get; set; } = "LOCAL";
        public string ExternalUserId { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public bool IsAdmin { get; set; } = false;
    }
}
