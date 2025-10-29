using System;

namespace BLE.Domain.Entities;

public class StsKochversuch
{
    public int Id { get; set; }
    public Guid StsTestId { get; set; }
    public decimal EinwaageVorKochen { get; set; }
    public decimal RueckwaageNachKochen { get; set; }
    public TimeSpan Kochzeit { get; set; }

    public StsTest? StsTest { get; set; }
}
