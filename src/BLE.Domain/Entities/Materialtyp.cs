using System;
using System.Collections.Generic;

namespace BLE.Domain.Entities;

public class Materialtyp : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public ICollection<PruefplanVorlage> PruefplanVorlagen { get; set; } = new List<PruefplanVorlage>();
    public ICollection<Probe> Proben { get; set; } = new List<Probe>();
}
