using System;
using System.Collections.Generic;

namespace BLE.Domain.Entities;

public class Produkt : BaseEntity
{
    public Guid WerkId { get; set; }
    public string Name { get; set; } = string.Empty;
    public ProduktKategorie Kategorie { get; set; } = ProduktKategorie.Gemisch;
    public string? SiebbereichText { get; set; }

    public Werk? Werk { get; set; }
    public ICollection<PruefplanVorlage> PruefplanVorlagen { get; set; } = new List<PruefplanVorlage>();
    public ICollection<Probe> Proben { get; set; } = new List<Probe>();
}
