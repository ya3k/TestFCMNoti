using Domain.Identity;
using System.Text.Json.Serialization;

namespace Domain.Common;

public interface IDeletable
{

    Guid? DeletedBy { get; set; }

    DateTime? DeletedOn { get; set; }

    bool IsDeleted { get; set; }
}