using System;

namespace GozemApi.Models {
    public class ApplicationUserViewModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? LastUpdated { get; set; }
        public string LastName { get; set; }
        public string ProfilePhoto { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}