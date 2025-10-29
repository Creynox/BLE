using System;
using System.ComponentModel.DataAnnotations;

namespace BLE.Domain.Entities;

public class StsErgebnis
{
    [Key]
    public Guid StsTestId { get; set; }
    public decimal S1 { get; set; }
    public decimal S2 { get; set; }
    public decimal KornformIndex { get; set; }
    public bool GrenzwerteOk { get; set; }

    public StsTest? StsTest { get; set; }
}
