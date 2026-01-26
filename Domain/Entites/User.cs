using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entites
{
    public class User :IdentityUser
    {
        public string Name { get; set; } = null!;
        public string? PhotoUrl { get; set; }
        public DateTime JoinedAt { get; set; }=DateTime.UtcNow;

    }
}
