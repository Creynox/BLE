using System;

namespace BLE.Domain.Entities;

public class Projekt : BaseEntity
{
    public Guid KundeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Ort { get; set; }
    public DateTime? Startdatum { get; set; }
    public DateTime? Enddatum { get; set; }
}

