using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BLE.Data;
using BLE.Domain.Entities;
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

    public void GenerateStsReport(StsTest test, string outputPath)
    {
        if (test is null)
        {
            throw new ArgumentNullException(nameof(test));
        }

        if (string.IsNullOrWhiteSpace(outputPath))
        {
            throw new ArgumentException("Output path is required.", nameof(outputPath));
        }

        var fractions = (test.Siebanalysen ?? Enumerable.Empty<StsSiebanalyse>())
            .OrderByDescending(f => f.Einwaage)
            .ToList();
        var kornform = test.Kornform;
        var koch = test.Kochversuch;
        var ergebnis = test.Ergebnis;

        var formattedMaterial = test.Materialtyp.ToString();
        var formattedDate = test.Entnahmedatum?.ToString("dd.MM.yyyy") ?? "-";

        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(25);
                page.Header().Column(col =>
                {
                    col.Spacing(2);
                    col.Item().Text("BLE Baustofflabor").SemiBold().FontSize(18);
                    col.Item().Text($"STS-Prüfbericht – {test.Probencode}").FontSize(14);
                });

                page.Content().Column(stack =>
                {
                    stack.Spacing(15);

                    stack.Item().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(10).Column(section =>
                    {
                        section.Spacing(5);
                        section.Item().Text($"Probencode: {test.Probencode}");
                        section.Item().Text($"Entnahmedatum: {formattedDate}");
                        section.Item().Text($"Werk / Produkt: {test.Werk ?? "-"} / {test.Produkt ?? "-"}");
                        section.Item().Text($"Materialtyp: {formattedMaterial}");
                        section.Item().Text($"Status: {test.Status ?? "-"}");
                    });

                    if (fractions.Count > 0)
                    {
                        stack.Item().Text("Siebanalyse").SemiBold();
                        stack.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("Sieb");
                                header.Cell().AlignRight().Text("Einwaage (g)");
                                header.Cell().AlignRight().Text("Rückwaage (g)");
                                header.Cell().AlignRight().Text("Durchgang %");
                                header.Cell().AlignRight().Text("Rückgang %");
                            });

                            foreach (var fraction in fractions)
                            {
                                table.Cell().Text(fraction.SiebBezeichnung ?? "-");
                                table.Cell().AlignRight().Text(fraction.Einwaage.ToString("0.###"));
                                table.Cell().AlignRight().Text(fraction.Rueckwaage.ToString("0.###"));
                                table.Cell().AlignRight().Text(fraction.DurchgangProzent.ToString("0.##"));
                                table.Cell().AlignRight().Text(fraction.RueckgangProzent.ToString("0.##"));
                            }
                        });
                    }

                    stack.Item().Row(row =>
                    {
                        row.ConstantItem(260).Element(left =>
                        {
                            left.Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(10).Column(section =>
                            {
                                section.Spacing(5);
                                section.Item().Text("Kornform").SemiBold();
                                section.Item().Text($"Einwaage gesamt: {kornform?.EinwaageGesamt.ToString("0.###") ?? "-"} g");
                                section.Item().Text($"Schlecht geformt: {kornform?.EinwaageSchlechtGeformt.ToString("0.###") ?? "-"} g");
                            });
                        });

                        row.RelativeItem().Element(right =>
                        {
                            right.Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(10).Column(section =>
                            {
                                section.Spacing(5);
                                section.Item().Text("Kochversuch").SemiBold();
                                section.Item().Text($"Einwaage vor Kochen: {koch?.EinwaageVorKochen.ToString("0.###") ?? "-"} g");
                                section.Item().Text($"Rückwaage nach Kochen: {koch?.RueckwaageNachKochen.ToString("0.###") ?? "-"} g");
                                var kochZeitText = koch is null ? "-" : (koch.Kochzeit == TimeSpan.Zero ? "-" : koch.Kochzeit.ToString(@"hh\:mm"));
                                section.Item().Text($"Kochzeit: {kochZeitText}");
                            });
                        });
                    });

                    stack.Item().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(10).Column(section =>
                    {
                        section.Spacing(5);
                        section.Item().Text("Ergebnis").SemiBold();
                        section.Item().Text($"S1 (< 5,6 mm): {ergebnis?.S1.ToString("0.##") ?? "-"} %");
                        section.Item().Text($"S2 (< 0,063 mm): {ergebnis?.S2.ToString("0.##") ?? "-"} %");
                        section.Item().Text($"Kornform-Index: {ergebnis?.KornformIndex.ToString("0.##") ?? "-"} %");
                        section.Item().Text($"Grenzwerte OK: {(ergebnis?.GrenzwerteOk == true ? "Ja" : "Nein")}");
                    });
                });
            });
        }).GeneratePdf(outputPath);
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

