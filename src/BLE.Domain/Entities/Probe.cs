using System;

namespace BLE.Domain.Entities;

public class Probe : BaseEntity
{
    public Guid ProjektId { get; set; }
    public string Probencode { get; set; } = string.Empty;
    public string Materialtyp { get; set; } = "asphalt"; // 'asphalt'|'beton'
    public DateTime? Entnahmedatum { get; set; }
    public decimal? Temperatur { get; set; }
    public decimal? Feuchte { get; set; }
    public string? Werk { get; set; }
    public string? Produkt { get; set; }
}

