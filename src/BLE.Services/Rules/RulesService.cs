using System.Linq;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using RulesEngine.Models;
using RulesEngine;



namespace BLE.Services.Rules;

public class RulesService
{
    private readonly RulesEngine.RulesEngine _engine;

    public RulesService(string? configBase = null)
    {
        var basePath = configBase ?? ResolveConfigBase();
        var json = File.ReadAllText(Path.Combine(basePath, "rules", "rules.workflows.json"));
        var workflows = JsonSerializer.Deserialize<Workflow[]>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new Workflow[0];
        _engine = new RulesEngine.RulesEngine(workflows);
    }

    public async Task<RuleResultTree[]> EvaluateAsync(string workflow, object input)
    {
        var results = await _engine.ExecuteAllRulesAsync(workflow, input);
        return results.ToArray();
    }


    private static string ResolveConfigBase()
    {
        var cwd = AppContext.BaseDirectory;
        string[] candidates = new[]
        {
            Path.Combine(cwd, "config"),
            Path.Combine(cwd, "..", "..", "..", "config"),
            Path.Combine(cwd, "..", "..", "config"),
        };
        foreach (var p in candidates)
            if (Directory.Exists(p)) return Path.GetFullPath(p);
        return Directory.GetCurrentDirectory();
    }
}

