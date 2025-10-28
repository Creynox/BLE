using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using BLE.Data;
using BLE.UI.Services.UI;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BLE.UI.ViewModels;

public class DashboardViewModel : BaseViewModel
{
    private readonly IDbContextFactory<BLEDbContext> _dbFactory;
    private readonly ToastService _toastService;

    public DashboardViewModel(IDbContextFactory<BLEDbContext> dbFactory, ToastService toastService)
    {
        _dbFactory = dbFactory;
        _toastService = toastService;

        Title = "Dashboard";
    }

    public ObservableCollection<DashboardMetric> Metrics { get; } = new();
    public ObservableCollection<RecentImportItem> RecentImports { get; } = new();

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        if (IsBusy)
        {
            return;
        }

        IsBusy = true;
        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync(cancellationToken);

            var totalProben = await db.Proben.CountAsync(cancellationToken);
            var totalCustomers = await db.Kunden.CountAsync(cancellationToken);
            var totalProjects = await db.Projekte.CountAsync(cancellationToken);
            var newestImport = await db.EtlFiles.OrderByDescending(f => f.ImportedAt).FirstOrDefaultAsync(cancellationToken);

            Metrics.Clear();
            Metrics.Add(new DashboardMetric("Proben gesamt", totalProben.ToString("N0")));
            Metrics.Add(new DashboardMetric("Kunden", totalCustomers.ToString("N0")));
            Metrics.Add(new DashboardMetric("Projekte", totalProjects.ToString("N0")));
            Metrics.Add(new DashboardMetric("Letzter Import", newestImport?.ImportedAt.ToLocalTime().ToString("g") ?? "–"));

            var imports = await db.EtlFiles
                .OrderByDescending(f => f.ImportedAt)
                .Take(5)
                .Select(f => new RecentImportItem(f.Filename, f.Source, f.Status ?? string.Empty, f.ImportedAt))
                .ToListAsync(cancellationToken);

            RecentImports.Clear();
            foreach (var item in imports)
            {
                RecentImports.Add(item);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            await _toastService.ShowAsync("Dashboard-Daten konnten nicht geladen werden.");
        }
        finally
        {
            IsBusy = false;
        }
    }
}

public record DashboardMetric(string Title, string Value);

public record RecentImportItem(string Filename, string Source, string Status, DateTime ImportedAt);
