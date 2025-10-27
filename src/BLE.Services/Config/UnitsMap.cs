using System.Collections.Generic;

namespace BLE.Services.Config;

public class UnitsMap
{
    public Dictionary<string, string[]> Map { get; set; } = new();

    public string Canonical(string unit)
    {
        foreach (var kv in Map)
        {
            foreach (var alias in kv.Value)
            {
                if (string.Equals(alias, unit, System.StringComparison.OrdinalIgnoreCase))
                    return kv.Key;
            }
        }
        return unit;
    }
}

