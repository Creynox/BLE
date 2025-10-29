using System;

namespace BLE.Domain.Entities;

public class StsKornform
{
    public int Id { get; set; }
    public Guid StsTestId { get; set; }
    public decimal EinwaageGesamt { get; set; }
    public decimal EinwaageSchlechtGeformt { get; set; }

    public StsTest? StsTest { get; set; }
}
