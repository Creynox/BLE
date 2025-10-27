using System;

namespace BLE.Domain.Entities;

public class Formel : BaseEntity
{
    public Guid PruefverfahrenId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Ausdruck { get; set; } = string.Empty; // C# expr or SQL
    public string? EinheitErgebnis { get; set; }
    public string? ValidierungSql { get; set; }
    public DateTime GueltigAb { get; set; }
    public DateTime? GueltigBis { get; set; }
}

