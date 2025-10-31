using System;

namespace BLE.Domain.Entities;

public class PruefplanVorlageItem : BaseEntity
{
    public Guid VorlageId { get; set; }
    public Guid PruefverfahrenId { get; set; }
    public bool Pflicht { get; set; } = true;
    public int Reihenfolge { get; set; }

    public PruefplanVorlage? Vorlage { get; set; }
    public Pruefverfahren? Pruefverfahren { get; set; }
}
