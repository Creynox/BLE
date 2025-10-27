using System.IO;
using System.Text.Json;
using BLE.Services.Config;
using Xunit;

namespace BLE.Tests.Unit;

public class EtlMappingTests
{
    [Fact]
    public void Can_Load_Sieb_Mapping()
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "config", "etl", "mapping.sieb.json");
        var json = File.ReadAllText(path);
        var map = JsonSerializer.Deserialize<EtlMapping>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.NotNull(map);
        Assert.True(map!.Columns.ContainsKey("Probencode"));
    }
}

