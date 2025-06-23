
using Domain.Identity;

namespace Domain.Common;

public interface IAuditable
{
    DateTime CreatedAt { get; set; }
    Guid CreatedBy { get; set; }
    DateTime? LastModified { get; set; }
    Guid? LastModifiedBy { get; set; }
}