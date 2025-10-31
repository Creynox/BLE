using System;

namespace BLE.Domain.Entities;

public class AuditLog : BaseEntity
{
    public string Entity { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public string Action { get; set; } = string.Empty;
    public Guid? UserId { get; set; }
    public DateTime AtUtc { get; set; }
    public string? PayloadJson { get; set; }
}
