using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using BLE.Data;
using BLE.Domain.Entities;
using BLE.UI.Services.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Maui.Controls;

namespace BLE.UI.ViewModels;

public class NewProbeViewModel : BaseViewModel
{
    private readonly IDbContextFactory<BLEDbContext> _dbFactory;
    private readonly ToastService _toastService;

    private string _code = string.Empty;
    private string _material = "asphalt";
    private DateTime? _samplingDate = DateTime.Today;
    private string? _plant;
    private string? _product;

    public NewProbeViewModel(IDbContextFactory<BLEDbContext> dbFactory, ToastService toastService)
    {
        _dbFactory = dbFactory;
        _toastService = toastService;
        Title = "Neue Probe";

        CreateProbeCommand = new Command(async () => await CreateProbeAsync(), () => !IsBusy);
    }

    public string Code
    {
        get => _code;
        set => SetProperty(ref _code, value);
    }

    public string Material
    {
        get => _material;
        set => SetProperty(ref _material, value);
    }

    public DateTime? SamplingDate
    {
        get => _samplingDate;
        set => SetProperty(ref _samplingDate, value);
    }

    public string? Plant
    {
        get => _plant;
        set => SetProperty(ref _plant, value);
    }

    public string? Product
    {
        get => _product;
        set => SetProperty(ref _product, value);
    }

    public ICommand CreateProbeCommand { get; }

    private async Task CreateProbeAsync(CancellationToken cancellationToken = default)
    {
        if (IsBusy)
        {
            return;
        }

        IsBusy = true;
        ((Command)CreateProbeCommand).ChangeCanExecute();

        try
        {
            if (string.IsNullOrWhiteSpace(Code))
            {
                await _toastService.ShowAsync("Ein Probencode wird benoetigt.");
                return;
            }

            await using var db = await _dbFactory.CreateDbContextAsync(cancellationToken);

            var exists = await db.Proben.AnyAsync(p => p.Probencode == Code, cancellationToken);
            if (exists)
            {
                await _toastService.ShowAsync("Eine Probe mit diesem Code existiert bereits.");
                return;
            }

            var probe = new Probe
            {
                Probencode = Code,
                Materialtyp = Material,
                Entnahmedatum = SamplingDate,
                Werk = Plant,
                Produkt = Product
            };

            db.Proben.Add(probe);
            await db.SaveChangesAsync(cancellationToken);

            await _toastService.ShowAsync("Probe gespeichert.");
            await Shell.Current.GoToAsync("//proben");

            Code = string.Empty;
            Material = "asphalt";
            SamplingDate = DateTime.Today;
            Plant = null;
            Product = null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            await _toastService.ShowAsync("Probe konnte nicht gespeichert werden.");
        }
        finally
        {
            IsBusy = false;
            ((Command)CreateProbeCommand).ChangeCanExecute();
        }
    }
}
