using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using BLE.Data;
using BLE.UI.Services.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Maui.Controls;

namespace BLE.UI.ViewModels;

public class ProbenViewModel : BaseViewModel
{
    private readonly IDbContextFactory<BLEDbContext> _dbFactory;
    private readonly ToastService _toastService;

    public ProbenViewModel(IDbContextFactory<BLEDbContext> dbFactory, ToastService toastService)
    {
        _dbFactory = dbFactory;
        _toastService = toastService;
        Title = "Labor / Proben";

        LoadCommand = new Command(async () => await LoadAsync());
    }

    public ObservableCollection<ProbeListItem> Proben { get; } = new();
    public ICommand LoadCommand { get; }

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

            var items = await db.Proben
                .OrderByDescending(p => p.Entnahmedatum)
                .ThenByDescending(p => p.CreatedAt)
                .Select(p => new ProbeListItem(
                    p.Id,
                    p.Probencode,
                    p.Materialtyp,
                    p.Entnahmedatum,
                    p.Werk,
                    p.Produkt))
                .Take(25)
                .ToListAsync(cancellationToken);

            Proben.Clear();
            foreach (var probe in items)
            {
                Proben.Add(probe);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            await _toastService.ShowAsync("Proben konnten nicht geladen werden.");
        }
        finally
        {
            IsBusy = false;
        }
    }
}

public record ProbeListItem(Guid Id, string Code, string Material, DateTime? Date, string? Plant, string? Product);
