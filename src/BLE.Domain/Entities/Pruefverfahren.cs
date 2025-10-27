using System;

namespace BLE.Domain.Entities;

public class Pruefverfahren : BaseEntity
{
    public Guid NormwerkId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Titel { get; set; } = string.Empty;
    public string? Beschreibung { get; set; }
}

