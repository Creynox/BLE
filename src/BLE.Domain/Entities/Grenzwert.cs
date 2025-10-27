using System;

namespace BLE.Domain.Entities;

public class Grenzwert : BaseEntity
{
    public Guid PruefverfahrenId { get; set; }
    public string Merkmal { get; set; } = string.Empty;
    public decimal? MinWert { get; set; }
    public decimal? MaxWert { get; set; }
    public string? Einheit { get; set; }
    public string? Bedingung { get; set; }
    public DateTime GueltigAb { get; set; }
    public DateTime? GueltigBis { get; set; }
    public string Scope { get; set; } = "global"; // global/werk/produkt/auftrag
}

