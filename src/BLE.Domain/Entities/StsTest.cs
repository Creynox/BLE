using System;
using System.Collections.Generic;

namespace BLE.Domain.Entities;

public class StsTest : BaseEntity
{
    public string Probencode { get; set; } = string.Empty;
    public DateTime? Entnahmedatum { get; set; }
    public string? Werk { get; set; }
    public string? Produkt { get; set; }
    public StsMaterialtyp Materialtyp { get; set; } = StsMaterialtyp.Asphalt;
    public decimal? GesamtEinwaage { get; set; }
    public string? Status { get; set; }

    public ICollection<StsSiebanalyse> Siebanalysen { get; set; } = new List<StsSiebanalyse>();
    public StsKornform? Kornform { get; set; }
    public StsKochversuch? Kochversuch { get; set; }
    public StsErgebnis? Ergebnis { get; set; }
}
