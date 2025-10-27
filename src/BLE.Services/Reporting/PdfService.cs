using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BLE.Data;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BLE.Services.Reporting;

public class PdfService
{
    private readonly BLEDbContext _db;

    public PdfService(BLEDbContext db)
    {
        _db = db;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<string> ExportProbeAsync(Guid probeId, string outputDir)
    {
        var probe = await _db.Proben.FirstAsync(p => p.Id == probeId);
        var fraktionen = await _db.Kornfraktionen.Where(k => k.ProbeId == probeId).OrderBy(k => k.FraktionIndex).ToListAsync();
        var messungen = await _db.Messungen.Where(m => m.ProbeId == probeId).ToListAsync();

        var file = Path.Combine(outputDir, $"BLE_{probe.Probencode}_{DateTime.Now:yyyyMMddHHmm}.pdf");

        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(20);
                page.Header().Row(row =>
                {
                    row.RelativeItem().Text("BLE Baustofflabor").SemiBold().FontSize(18);
                    row.ConstantItem(100).Height(20).Background(Colors.Grey.Lighten2);
                });
                page.Content().Stack(stack =>
                {
                    stack.Item().Text($"Probe: {probe.Probencode}").Bold();
                    stack.Spacing(5);
                    stack.Item().Text($"Werk: {probe.Werk}  Produkt: {probe.Produkt}");
                    stack.Item().Text($"Entnahmedatum: {probe.Entnahmedatum:dd.MM.yyyy}");

                    if (messungen.Count > 0)
                    {
                        stack.Item().Text("Messungen").Bold();
                        stack.Item().Table(t =>
                        {
                            t.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn(3);
                                c.RelativeColumn(1);
                                c.RelativeColumn(1);
                            });
                            t.Header(h =>
                            {
                                h.Cell().Text("Merkmal");
                                h.Cell().Text("Wert");
                                h.Cell().Text("Einheit");
                            });
                            foreach (var m in messungen)
                            {
                                t.Cell().Text(m.Merkmal);
                                t.Cell().Text(m.IstWert?.ToString("0.###"));
                                t.Cell().Text(m.Einheit);
                            }
                        });
                    }

                    if (fraktionen.Count > 0)
                    {
                        stack.Item().Text("Siebanalyse").Bold();
                        stack.Item().Table(t =>
                        {
                            t.ColumnsDefinition(c =>
                            {
                                c.ConstantColumn(40);
                                c.RelativeColumn();
                                c.RelativeColumn();
                                c.RelativeColumn();
                            });
                            t.Header(h =>
                            {
                                h.Cell().Text("Idx");
                                h.Cell().Text("Min mm");
                                h.Cell().Text("Max mm");
                                h.Cell().Text("Durchgang %");
                            });
                            foreach (var k in fraktionen)
                            {
                                t.Cell().Text(k.FraktionIndex.ToString());
                                t.Cell().Text(k.KorngroesseMinMm?.ToString("0.###"));
                                t.Cell().Text(k.KorngroesseMaxMm?.ToString("0.###"));
                                t.Cell().Text(k.DurchgangPercent?.ToString("0.##"));
                            }
                        });
                    }
                });
            });
        }).GeneratePdf(file);

        return file;
    }
}

