using System;
using System.Collections.Generic;

namespace BLE.Domain.Entities;

public class PruefplanVorlage : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public Guid? WerkId { get; set; }
    public Guid? ProduktId { get; set; }
    public Guid? MaterialtypId { get; set; }
    public string Version { get; set; } = string.Empty;
    public DateTime GueltigAbUtc { get; set; }
    public DateTime? GueltigBisUtc { get; set; }

    public Werk? Werk { get; set; }
    public Produkt? Produkt { get; set; }
    public Materialtyp? Materialtyp { get; set; }
    public ICollection<PruefplanVorlageItem> Items { get; set; } = new List<PruefplanVorlageItem>();
}
