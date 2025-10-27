using System;

namespace BLE.Domain.Entities;

public class Kornfraktion : BaseEntity
{
    public Guid ProbeId { get; set; }
    public Guid PruefverfahrenId { get; set; }
    public int FraktionIndex { get; set; }
    public decimal? KorngroesseMinMm { get; set; }
    public decimal? KorngroesseMaxMm { get; set; }
    public decimal? MasseG { get; set; }
    public decimal? AnteilPercent { get; set; }
    public decimal? DurchgangPercent { get; set; }
}

