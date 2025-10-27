using System;

namespace BLE.Domain.Entities;

public class Messung : BaseEntity
{
    public Guid ProbeId { get; set; }
    public Guid PruefverfahrenId { get; set; }
    public string Merkmal { get; set; } = string.Empty;
    public decimal? IstWert { get; set; }
    public string? Einheit { get; set; }
    public decimal? Messunsicherheit { get; set; }
    public DateTime? Messzeitpunkt { get; set; }
    public string? Pruefer { get; set; }
}

