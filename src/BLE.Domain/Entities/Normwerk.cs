using System;

namespace BLE.Domain.Entities;

public class Normwerk : BaseEntity
{
    public string Bezeichnung { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public DateTime GueltigAb { get; set; }
    public DateTime? GueltigBis { get; set; }
    public string? QuelleUrl { get; set; }
    public string? Bemerkung { get; set; }
}

