using System;

namespace BLE.Domain.Entities;

public class Pruefauftrag : BaseEntity
{
    public Guid ProbeId { get; set; }
    public Guid PruefverfahrenId { get; set; }
    public AuftragStatus Status { get; set; } = AuftragStatus.Offen;
    public Guid? BearbeiterId { get; set; }
    public DateTime? StartedAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }

    public Probe? Probe { get; set; }
    public Pruefverfahren? Pruefverfahren { get; set; }
}
