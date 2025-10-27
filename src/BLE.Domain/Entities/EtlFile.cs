using System;

namespace BLE.Domain.Entities;

public class EtlFile : BaseEntity
{
    public string Filename { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string FileHash { get; set; } = string.Empty;
    public DateTime ImportedAt { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "ok"; // ok | warn | error
    public string? Message { get; set; }
}

public class EtlLog : BaseEntity
{
    public Guid EtlFileId { get; set; }
    public string SheetName { get; set; } = string.Empty;
    public int? RowIndex { get; set; }
    public string? ColumnName { get; set; }
    public string Severity { get; set; } = "info"; // info|warn|error
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? PayloadJson { get; set; }
}

