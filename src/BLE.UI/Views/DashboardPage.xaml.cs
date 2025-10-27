using System;
using System.Threading.Tasks;

using BLE.Data;
using BLE.Services;

using BLE.Services.Etl;
using BLE.Services.Reporting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Maui.Controls;

namespace BLE.UI.Views;

public partial class DashboardPage : ContentPage
{
    private readonly IServiceProvider _sp;

    public DashboardPage(IServiceProvider sp)
    {
        InitializeComponent();
        _sp = sp;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadImportsAsync();
    }

    private async Task LoadImportsAsync()
    {
        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BLEDbContext>();
        var items = await db.EtlFiles.OrderByDescending(x => x.ImportedAt).Take(50).ToListAsync();
        _imports.ItemsSource = items;
    }

    private async void OnImportClicked(object sender, EventArgs e)
    {
        var path = _filePath.Text?.Trim();
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
        {
            _status.TextColor = Colors.Red;
            _status.Text = "Datei nicht gefunden";
            return;
        }
        using var scope = _sp.CreateScope();
        var etl = scope.ServiceProvider.GetRequiredService<EtlService>();
        var (id, status) = await etl.ImportSieveExcelAsync(path);
        _status.TextColor = status == "error" ? Colors.Red : Colors.Green;
        _status.Text = $"Import {status} (ID: {id})";
        await LoadImportsAsync();
    }

    private async void OnExportPdfClicked(object sender, EventArgs e)
    {
        var probecode = _probencode.Text?.Trim();
        if (string.IsNullOrWhiteSpace(probecode))
        {
            _status.TextColor = Colors.Red;
            _status.Text = "Probencode eingeben";
            return;
        }
        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BLEDbContext>();
        var pdf = scope.ServiceProvider.GetRequiredService<PdfService>();
        var probe = await db.Proben.FirstOrDefaultAsync(p => p.Probencode == probecode);
        if (probe == null)
        {
            _status.TextColor = Colors.Red;
            _status.Text = "Probe nicht gefunden";
            return;
        }
        var dir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        var file = await pdf.ExportProbeAsync(probe.Id, dir);
        _status.TextColor = Colors.Green;
        _status.Text = $"PDF gespeichert: {file}";
    }
}

