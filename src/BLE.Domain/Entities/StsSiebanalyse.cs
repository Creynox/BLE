using System;

namespace BLE.Domain.Entities;

public class StsSiebanalyse
{
    public int Id { get; set; }
    public Guid StsTestId { get; set; }
    public string SiebBezeichnung { get; set; } = string.Empty;
    public decimal Einwaage { get; set; }
    public decimal Rueckwaage { get; set; }
    public decimal DurchgangProzent { get; set; }
    public decimal RueckgangProzent { get; set; }

    public StsTest? StsTest { get; set; }
}
