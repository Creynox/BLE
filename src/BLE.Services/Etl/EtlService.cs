using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using BLE.Data;
using BLE.Domain.Entities;
using BLE.Services.Config;
using ExcelDataReader;
using Microsoft.EntityFrameworkCore;

namespace BLE.Services.Etl;

public class EtlService
{
    private readonly BLEDbContext _db;
    private readonly string _baseConfigPath;

    public EtlService(BLEDbContext db, string? baseConfigPath = null)
    {
        _db = db;
        _baseConfigPath = baseConfigPath ?? ResolveConfigBase();
        System.Text.Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    private static string ResolveConfigBase()
    {
        // Try typical locations: ./config, ../../config from bin
        var cwd = AppContext.BaseDirectory;
        string[] candidates = new[]
        {
            Path.Combine(cwd, "config"),
            Path.Combine(cwd, "..", "..", "..", "config"),
            Path.Combine(cwd, "..", "..", "config"),
        };
        foreach (var p in candidates)
        {
            if (Directory.Exists(p)) return Path.GetFullPath(p);
        }
        return Directory.GetCurrentDirectory();
    }

    public async Task<(Guid etlFileId, string status)> ImportSieveExcelAsync(string filePath, string source = "local", CancellationToken ct = default)
    {
        var mapping = LoadMapping(Path.Combine(_baseConfigPath, "etl", "mapping.sieb.json"));
        var units = LoadUnits(Path.Combine(_baseConfigPath, "etl", "units.map.json"));

        var hash = ComputeSha256(filePath);
        if (await _db.EtlFiles.AnyAsync(f => f.FileHash == hash, ct))
        {
            var existing = await _db.EtlFiles.FirstAsync(f => f.FileHash == hash, ct);
            return (existing.Id, existing.Status);
        }

        using var tx = await _db.Database.BeginTransactionAsync(ct);
        var etl = new EtlFile { Filename = Path.GetFileName(filePath), Source = source, FileHash = hash, Status = "ok" };
        _db.EtlFiles.Add(etl);
        await _db.SaveChangesAsync(ct);

        try
        {
            using var stream = File.OpenRead(filePath);
            using var reader = ExcelReaderFactory.CreateReader(stream);
            var ds = reader.AsDataSet(new ExcelDataSetConfiguration
            {
                ConfigureDataTable = _ => new ExcelDataTableConfiguration { UseHeaderRow = false }
            });

            foreach (DataTable table in ds.Tables)
            {
                var sheet = table.TableName ?? "Sheet";
                if (!Matches(mapping, sheet)) continue;

                int headerRow = Math.Max(mapping.Header_Row_Hint - 1, 0);
                var headers = GetHeaders(table, headerRow);

                int rows = table.Rows.Count;
                for (int r = headerRow + 1; r < rows; r++)
                {
                    var row = table.Rows[r];
                    if (IsRowEmpty(row))
                    {
                        // Heuristic: allow a run of empties to end
                        continue;
                    }

                    try
                    {
                        var (probe, fraktionen, messungen) = MapRow(mapping, units, headers, row);

                        // Upsert Probe by Probencode
                        var existingProbe = await _db.Proben.FirstOrDefaultAsync(p => p.Probencode == probe.Probencode, ct);
                        if (existingProbe == null)
                        {
                            _db.Proben.Add(probe);
                            await _db.SaveChangesAsync(ct);
                        }
                        else
                        {
                            // simple upsert of some fields
                            existingProbe.Werk = probe.Werk;
                            existingProbe.Produkt = probe.Produkt;
                            existingProbe.Entnahmedatum = probe.Entnahmedatum ?? existingProbe.Entnahmedatum;
                            existingProbe.Feuchte = probe.Feuchte ?? existingProbe.Feuchte;
                            await _db.SaveChangesAsync(ct);
                            probe = existingProbe;
                        }

                        if (fraktionen.Count > 0)
                        {
                            foreach (var f in fraktionen)
                            {
                                f.ProbeId = probe.Id;
                                _db.Kornfraktionen.Add(f);
                            }
                        }

                        if (messungen.Count > 0)
                        {
                            foreach (var m in messungen)
                            {
                                m.ProbeId = probe.Id;
                                _db.Messungen.Add(m);
                            }
                        }

                        await _db.SaveChangesAsync(ct);
                    }
                    catch (Exception exRow)
                    {
                        _db.EtlLogs.Add(new EtlLog
                        {
                            EtlFileId = etl.Id,
                            SheetName = sheet,
                            RowIndex = r + 1,
                            Severity = "error",
                            Code = "ROW_MAP",
                            Message = exRow.Message
                        });
                        etl.Status = "error";
                    }
                }
            }

            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync(ct);
            etl.Status = "error";
            etl.Message = ex.Message;
            await _db.SaveChangesAsync(ct);
            return (etl.Id, etl.Status);
        }

        return (etl.Id, etl.Status);
    }

    private static bool Matches(EtlMapping mapping, string sheetName)
        => mapping.Sheet_Patterns.Length == 0 || mapping.Sheet_Patterns.Any(p => sheetName.Contains(p, StringComparison.OrdinalIgnoreCase));

    private static string[] GetHeaders(DataTable table, int headerRow)
    {
        var headers = new List<string>();
        for (int c = 0; c < table.Columns.Count; c++)
        {
            var val = table.Rows[headerRow][c]?.ToString()?.Trim() ?? string.Empty;
            headers.Add(val);
        }
        return headers.ToArray();
    }

    private static bool IsRowEmpty(DataRow row)
    {
        foreach (var item in row.ItemArray)
            if (item != null && !string.IsNullOrWhiteSpace(item.ToString()))
                return false;
        return true;
    }

    private static (Probe probe, List<Kornfraktion> fraktionen, List<Messung> messungen) MapRow(EtlMapping mapping, UnitsMap units, string[] headers, DataRow row)
    {
        var probe = new Probe { Probencode = Guid.NewGuid().ToString("N").Substring(0,8) }; // fallback
        var fraktionen = new List<Kornfraktion>();
        var messungen = new List<Messung>();

        foreach (var kv in mapping.Columns)
        {
            var srcHeader = kv.Key;
            var cfg = kv.Value;
            int idx = Array.FindIndex(headers, h => string.Equals(h, srcHeader, StringComparison.OrdinalIgnoreCase));
            if (idx < 0) continue;
            var raw = row[idx]?.ToString() ?? string.Empty;
            if (cfg.Trim == true) raw = raw.Trim();

            object? val = ParseValue(cfg, raw);

            if (cfg.Target.StartsWith("probe.", StringComparison.OrdinalIgnoreCase))
            {
                var prop = cfg.Target[6..];
                switch (prop)
                {
                    case "probencode": probe.Probencode = Convert.ToString(val) ?? probe.Probencode; break;
                    case "werk": probe.Werk = Convert.ToString(val); break;
                    case "produkt": probe.Produkt = Convert.ToString(val); break;
                    case "entnahmedatum": probe.Entnahmedatum = val as DateTime?; break;
                    case "feuchte": probe.Feuchte = ToDecimal(val); break;
                }
            }
            else if (cfg.Target.StartsWith("messung[", StringComparison.OrdinalIgnoreCase))
            {
                var end = cfg.Target.IndexOf(']');
                var key = cfg.Target.Substring(8, end - 8);
                var m = new Messung { Merkmal = key, IstWert = ToDecimal(val), Einheit = units.Canonical(cfg.Unit ?? string.Empty) };
                messungen.Add(m);
            }
        }

        if (mapping.Fraction_Table is { } ft)
        {
            // Find starting row (after header)
            int startRow = Array.IndexOf(headers, headers.FirstOrDefault()) + 1; // naive: start at next row
            // Iterate rows until N empty rows
            int emptyRun = 0;
            for (int r = startRow; r < row.Table.Rows.Count; r++)
            {
                var dataRow = row.Table.Rows[r];
                if (IsRowEmpty(dataRow))
                {
                    emptyRun++;
                    if (emptyRun >= ft.StopWhenEmptyRows) break;
                    continue;
                }
                else emptyRun = 0;

                Kornfraktion k = new();
                if (ft.Columns.TryGetValue("Korngröße_min_mm", out var cmin))
                    k.KorngroesseMinMm = ToDecimal(ParseValue(new EtlColumnMap { Type = cmin.Type, Decimal = cmin.Decimal }, GetByHeader(headers, dataRow, cmin.Source)));
                if (ft.Columns.TryGetValue("Korngröße_max_mm", out var cmax))
                    k.KorngroesseMaxMm = ToDecimal(ParseValue(new EtlColumnMap { Type = cmax.Type, Decimal = cmax.Decimal }, GetByHeader(headers, dataRow, cmax.Source)));
                if (ft.Columns.TryGetValue("Masse_g", out var mcol))
                    k.MasseG = ToDecimal(ParseValue(new EtlColumnMap { Type = mcol.Type }, GetByHeader(headers, dataRow, mcol.Source)));
                if (ft.Columns.TryGetValue("Anteil_%", out var acol))
                    k.AnteilPercent = ToDecimal(ParseValue(new EtlColumnMap { Type = acol.Type, Decimal = acol.Decimal }, GetByHeader(headers, dataRow, acol.Source)));
                if (ft.Columns.TryGetValue("Durchgang_%", out var dcol))
                    k.DurchgangPercent = ToDecimal(ParseValue(new EtlColumnMap { Type = dcol.Type, Decimal = dcol.Decimal }, GetByHeader(headers, dataRow, dcol.Source)));

                k.FraktionIndex = fraktionen.Count;
                fraktionen.Add(k);
            }
        }

        return (probe, fraktionen, messungen);
    }

    private static string GetByHeader(string[] headers, DataRow row, string header)
    {
        int idx = Array.FindIndex(headers, h => string.Equals(h, header, StringComparison.OrdinalIgnoreCase));
        return idx >= 0 ? (row[idx]?.ToString() ?? string.Empty) : string.Empty;
    }

    private static object? ParseValue(EtlColumnMap cfg, string raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return null;
        switch (cfg.Type)
        {
            case "number":
                var style = NumberStyles.Any;
                var provider = (cfg.Decimal == ",") ? (IFormatProvider)new CultureInfo("de-DE") : CultureInfo.InvariantCulture;
                if (decimal.TryParse(raw, style, provider, out var dec)) return dec;
                if (double.TryParse(raw, style, provider, out var dbl)) return Convert.ToDecimal(dbl);
                return null;
            case "date":
                if (cfg.Formats != null && cfg.Formats.Length > 0)
                {
                    foreach (var fmt in cfg.Formats)
                        if (DateTime.TryParseExact(raw, fmt, new CultureInfo("de-DE"), DateTimeStyles.None, out var dt)) return dt;
                }
                if (DateTime.TryParse(raw, out var any)) return any;
                return null;
            default:
                return raw;
        }
    }

    private static decimal? ToDecimal(object? val)
        => val == null ? null : Convert.ToDecimal(val, CultureInfo.InvariantCulture);

    private static EtlMapping LoadMapping(string path)
    {
        var json = File.ReadAllText(path);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<EtlMapping>(json, options) ?? new EtlMapping();
    }

    private static UnitsMap LoadUnits(string path)
    {
        var json = File.ReadAllText(path);
        var dict = JsonSerializer.Deserialize<Dictionary<string, string[]>>(json) ?? new();
        return new UnitsMap { Map = dict };
    }

    private static string ComputeSha256(string filePath)
    {
        using var sha = SHA256.Create();
        using var stream = File.OpenRead(filePath);
        var hash = sha.ComputeHash(stream);
        return BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();
    }
}

