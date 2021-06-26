using System;

namespace GozemApi.Models {
    public interface IBaseEntity
    {
        string Id { get; set; }
        DateTime? DateCreated { get; set; }
        DateTime? LastUpdated { get; set; }
    }
}