using System;
using System.Collections.Generic;

namespace BLE.Domain.Entities;

public class Probe : BaseEntity
{
    public Guid ProjektId { get; set; }
    public string Probencode { get; set; } = string.Empty;
    public string Materialtyp { get; set; } = "asphalt";
    public DateTime? Entnahmedatum { get; set; }
    public decimal? Temperatur { get; set; }
    public decimal? Feuchte { get; set; }
    public string? Werk { get; set; }
    public string? Produkt { get; set; }

    // Workspace-specific fields (optional, nullable to remain backward-compatible)
    public Guid? WerkId { get; set; }
    public Guid? ProduktId { get; set; }
    public Guid? MaterialtypId { get; set; }
    public Guid? KundeId { get; set; }
    public DateTime? EingangAmUtc { get; set; }
    public Guid? BearbeiterId { get; set; }
    public ProbeStatus Status { get; set; } = ProbeStatus.Neu;
    public DateTime? CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }

    public Werk? WorkspaceWerk { get; set; }
    public Produkt? WorkspaceProdukt { get; set; }
    public Materialtyp? WorkspaceMaterialtyp { get; set; }
    public ICollection<Pruefauftrag> Pruefauftraege { get; set; } = new List<Pruefauftrag>();
}
