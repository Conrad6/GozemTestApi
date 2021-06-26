using System;

using Raven.Identity;

namespace GozemApi.Models
{
    public class ApplicationUser : IdentityUser, IBaseEntity
    {
        public string ProfilePhoto { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? LastUpdated { get; set; }
    }
}