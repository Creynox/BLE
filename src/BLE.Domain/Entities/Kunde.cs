using System;

namespace BLE.Domain.Entities;

public class Kunde : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Adresse { get; set; }
}

