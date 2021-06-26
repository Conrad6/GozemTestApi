using System;

using Microsoft.AspNetCore.Identity;

namespace GozemApi.Models {
    public class ApplicationUserRole : IdentityUserRole<string>, IBaseEntity
    {
        public string Id { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? LastUpdated { get; set; }
    }
}