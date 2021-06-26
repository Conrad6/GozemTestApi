using System;

using Microsoft.AspNetCore.Identity;

namespace GozemApi.Models {
    public class ApplicationRole : IdentityRole, IBaseEntity
    {
        public DateTime? DateCreated { get; set; }
        public DateTime? LastUpdated { get; set; }
    }
}