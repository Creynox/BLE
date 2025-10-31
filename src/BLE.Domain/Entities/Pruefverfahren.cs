using System;
using System.Collections.Generic;

namespace BLE.Domain.Entities;

public class Pruefverfahren : BaseEntity
{
    // Legacy fields (kept for backward compatibility)
    public Guid NormwerkId { get; set; }
    public string Titel { get; set; } = string.Empty;
    public string? Beschreibung { get; set; }

    // Workspace additions
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool Aktiv { get; set; } = true;

    public ICollection<PruefplanVorlageItem> VorlageItems { get; set; } = new List<PruefplanVorlageItem>();
    public ICollection<Pruefauftrag> Pruefauftraege { get; set; } = new List<Pruefauftrag>();
}
