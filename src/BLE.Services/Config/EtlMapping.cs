using System.Collections.Generic;

namespace BLE.Services.Config;

public class EtlColumnMap
{
    public string Target { get; set; } = string.Empty;
    public string Type { get; set; } = "string";
    public bool? Required { get; set; }
    public bool? Trim { get; set; }
    public string? Unit { get; set; }
    public string[]? Formats { get; set; }
    public string? Decimal { get; set; }
}

public class EtlFractionTable
{
    public bool StartAfterHeader { get; set; }
    public Dictionary<string, EtlFractionColumn> Columns { get; set; } = new();
    public int StopWhenEmptyRows { get; set; } = 2;
}

public class EtlFractionColumn
{
    public string Source { get; set; } = string.Empty;
    public string Type { get; set; } = "number";
    public string? Decimal { get; set; }
}

public class EtlMapping
{
    public string[] Sheet_Patterns { get; set; } = []; // e.g., STS,HST
    public int Header_Row_Hint { get; set; } = 1;
    public string[] Required_Fields { get; set; } = [];
    public Dictionary<string, EtlColumnMap> Columns { get; set; } = new();
    public EtlFractionTable? Fraction_Table { get; set; }
}

