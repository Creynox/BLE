using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using BLE.Data;
using BLE.Domain.Entities;
using BLE.Services.Reporting;
using BLE.Services.Sts;
using BLE.UI.Services.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Maui.Controls;

namespace BLE.UI.ViewModels;

public class StsTestWorkspaceViewModel : BaseViewModel
{
    private static readonly string[] DefaultSieveLabels = new[]
    {
        "56 mm",
        "32 mm",
        "16 mm",
        "8 mm",
        "5,6 mm",
        "2 mm",
        "0,063 mm",
        "< 5,6 mm",
        "< 0,063 mm"
    };

    private readonly IDbContextFactory<BLEDbContext> _dbFactory;
    private readonly ToastService _toastService;
    private readonly IStsEvaluationService _evaluationService;
    private readonly PdfService _pdfService;

    private readonly ObservableCollection<StsTest> _tests = new();
    private readonly ObservableCollection<StsSiebanalyse> _fractions = new();
    private readonly ObservableCollection<string> _validationErrors = new();

    private readonly Command _addFractionCommand;
    private readonly Command<StsSiebanalyse> _removeFractionCommand;
    private readonly Command _computeCommand;
    private readonly Command _saveCommand;
    private readonly Command _exportPdfCommand;

    private List<StsTest> _allTests = new();
    private StsTest? _current;
    private StsTest? _selectedListItem;
    private string _searchTerm = string.Empty;
    private bool _hasLoaded;

    public StsTestWorkspaceViewModel(
        IDbContextFactory<BLEDbContext> dbFactory,
        ToastService toastService,
        IStsEvaluationService evaluationService,
        PdfService pdfService)
    {
        _dbFactory = dbFactory;
        _toastService = toastService;
        _evaluationService = evaluationService;
        _pdfService = pdfService;

        Title = "STS-Tests";
        Materialtypen = Enum.GetValues<StsMaterialtyp>();

        LoadCommand = new Command(async () => await LoadAsync(force: true), () => !IsBusy);
        NewCommand = new Command(CreateNewTest, () => !IsBusy);
        OpenCommand = new Command<StsTest?>(async test => await OpenTestAsync(test));

        _addFractionCommand = new Command(AddFraction, () => Current is not null);
        _removeFractionCommand = new Command<StsSiebanalyse>(fraction => RemoveFraction(fraction), fraction => Current is not null && fraction is not null);
        _computeCommand = new Command(async () => await ComputeAsync(), () => Current is not null && !IsBusy);
        _saveCommand = new Command(async () => await SaveAsync(), () => Current is not null && !IsBusy);
        _exportPdfCommand = new Command(async () => await ExportPdfAsync(), () => Current is not null && !IsBusy);
        _validationErrors.CollectionChanged += (_, __) => OnPropertyChanged(nameof(HasValidationErrors));
    }

    public ObservableCollection<StsTest> Tests => _tests;

    public ObservableCollection<StsSiebanalyse> Fractions => _fractions;

    public ObservableCollection<string> ValidationErrors => _validationErrors;

    public Array Materialtypen { get; }

    public StsTest? Current
    {
        get => _current;
        private set
        {
            if (SetProperty(ref _current, value))
            {
                EnsureCurrentGraph(value);
                LoadFractions(value);
                Title = value?.Probencode ?? "STS-Tests";
                UpdateCommandStates();
                OnPropertyChanged(nameof(HasCurrent));
                if (value is null)
                {
                    ReplaceValidationErrors(Array.Empty<string>());
                }
            }
        }
    }

    public bool HasCurrent => Current is not null;

    public bool HasValidationErrors => _validationErrors.Count > 0;

    public StsTest? SelectedListItem
    {
        get => _selectedListItem;
        set => SetProperty(ref _selectedListItem, value);
    }

    public string SearchTerm
    {
        get => _searchTerm;
        set
        {
            if (SetProperty(ref _searchTerm, value))
            {
                ApplyFilter();
            }
        }
    }

    public ICommand LoadCommand { get; }

    public ICommand NewCommand { get; }

    public ICommand OpenCommand { get; }

    public ICommand AddFractionCommand => _addFractionCommand;

    public ICommand RemoveFractionCommand => _removeFractionCommand;

    public ICommand ComputeCommand => _computeCommand;

    public ICommand SaveCommand => _saveCommand;

    public ICommand ExportPdfCommand => _exportPdfCommand;

    public async Task LoadAsync(bool force = false, Guid? selectId = null)
    {
        if (IsBusy)
        {
            return;
        }

        if (!force && _hasLoaded)
        {
            return;
        }

        try
        {
            IsBusy = true;
            UpdateCommandStates();

            await using var db = await _dbFactory.CreateDbContextAsync();

            var query = db.StsTests
                .Include(t => t.Siebanalysen)
                .Include(t => t.Kornform)
                .Include(t => t.Kochversuch)
                .Include(t => t.Ergebnis)
                .AsNoTracking();

            var items = await query
                .OrderByDescending(t => t.Entnahmedatum ?? DateTime.MinValue)
                .ThenByDescending(t => t.CreatedAt)
                .ToListAsync();

            _allTests = items;
            _hasLoaded = true;

            ApplyFilter(selectId);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
        }
        finally
        {
            IsBusy = false;
            UpdateCommandStates();
        }
    }

    private void CreateNewTest()
    {
        var test = new StsTest
        {
            Id = Guid.NewGuid(),
            Probencode = string.Empty,
            Entnahmedatum = DateTime.Today,
            Werk = string.Empty,
            Produkt = string.Empty,
            Materialtyp = StsMaterialtyp.Asphalt,
            Status = "Entwurf",
            Siebanalysen = new List<StsSiebanalyse>(),
            Kornform = new StsKornform(),
            Kochversuch = new StsKochversuch(),
            Ergebnis = new StsErgebnis()
        };

        foreach (var label in DefaultSieveLabels)
        {
            test.Siebanalysen.Add(new StsSiebanalyse
            {
                SiebBezeichnung = label,
                StsTestId = test.Id
            });
        }

        SelectedListItem = null;
        Current = test;
        ReplaceValidationErrors(Array.Empty<string>());
        UpdateCommandStates();
    }

    private Task OpenTestAsync(StsTest? test)
    {
        if (test is null)
        {
            Current = null;
            ReplaceValidationErrors(Array.Empty<string>());
            UpdateCommandStates();
            return Task.CompletedTask;
        }

        SelectedListItem = test;

        var model = CloneForEditing(test);
        Current = model;
        ReplaceValidationErrors(Array.Empty<string>());
        UpdateCommandStates();
        return Task.CompletedTask;
    }

    private void ApplyFilter(Guid? selectId = null)
    {
        IEnumerable<StsTest> query = _allTests;

        if (!string.IsNullOrWhiteSpace(SearchTerm))
        {
            query = query.Where(t =>
                !string.IsNullOrWhiteSpace(t.Probencode) &&
                t.Probencode.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase));
        }

        var filtered = query
            .OrderByDescending(t => t.Entnahmedatum ?? DateTime.MinValue)
            .ThenByDescending(t => t.CreatedAt)
            .ToList();

        SyncTests(filtered);

        var currentIsUnsaved = Current is not null && filtered.All(t => t.Id != Current.Id);
        if (currentIsUnsaved && !selectId.HasValue)
        {
            SelectedListItem = null;
            return;
        }

        StsTest? selected = null;
        if (selectId.HasValue)
        {
            selected = filtered.FirstOrDefault(t => t.Id == selectId.Value);
        }
        else if (SelectedListItem is not null)
        {
            selected = filtered.FirstOrDefault(t => t.Id == SelectedListItem.Id);
        }

        if (selected is not null)
        {
            SelectedListItem = selected;
            Current = CloneForEditing(selected);
            ReplaceValidationErrors(Array.Empty<string>());
        }
        else if (filtered.Count > 0)
        {
            SelectedListItem = filtered.First();
            Current = CloneForEditing(filtered.First());
            ReplaceValidationErrors(Array.Empty<string>());
        }
        else
        {
            SelectedListItem = null;
            Current = null;
        }
    }

    private void SyncTests(IReadOnlyList<StsTest> items)
    {
        _tests.Clear();
        foreach (var item in items)
        {
            _tests.Add(item);
        }
    }

    private void EnsureCurrentGraph(StsTest? test)
    {
        if (test is null)
        {
            return;
        }

        test.Siebanalysen ??= new List<StsSiebanalyse>();
        test.Kornform ??= new StsKornform();
        test.Kornform.StsTestId = test.Id;
        test.Kochversuch ??= new StsKochversuch();
        test.Kochversuch.StsTestId = test.Id;
        test.Ergebnis ??= new StsErgebnis { StsTestId = test.Id };
    }

    private void LoadFractions(StsTest? test)
    {
        _fractions.Clear();
        if (test?.Siebanalysen is null)
        {
            return;
        }

        foreach (var fraction in test.Siebanalysen.OrderBy(f => f.Id == 0 ? int.MaxValue : f.Id))
        {
            _fractions.Add(new StsSiebanalyse
            {
                Id = fraction.Id,
                StsTestId = test.Id,
                SiebBezeichnung = fraction.SiebBezeichnung,
                Einwaage = fraction.Einwaage,
                Rueckwaage = fraction.Rueckwaage,
                DurchgangProzent = fraction.DurchgangProzent,
                RueckgangProzent = fraction.RueckgangProzent
            });
        }
    }

    private void AddFraction()
    {
        if (Current is null)
        {
            return;
        }

        var fraction = new StsSiebanalyse
        {
            StsTestId = Current.Id,
            SiebBezeichnung = "Neue Fraktion"
        };

        _fractions.Add(fraction);
        UpdateCommandStates();
    }

    private void RemoveFraction(StsSiebanalyse? fraction)
    {
        if (fraction is null)
        {
            return;
        }

        if (_fractions.Contains(fraction))
        {
            _fractions.Remove(fraction);
        }

        UpdateCommandStates();
    }

    private async Task ComputeAsync()
    {
        await ComputeInternalAsync(showSuccessToast: true);
    }

    private async Task<bool> ComputeInternalAsync(bool showSuccessToast)
    {
        if (Current is null)
        {
            ReplaceValidationErrors(Array.Empty<string>());
            return false;
        }

        try
        {
            IsBusy = true;
            UpdateCommandStates();

            UpdateCurrentModelFromForm();
            var errors = _evaluationService.Validieren(Current);
            ReplaceValidationErrors(errors);

            if (errors.Count > 0)
            {
                await _toastService.ShowAsync("Eingabefehler vorhanden.");
                return false;
            }

            _evaluationService.BerechneSiebanalyse(Current);
            var result = _evaluationService.BerechneErgebnis(Current);
            Current.Ergebnis = result;

            LoadFractions(Current);
            OnPropertyChanged(nameof(Current));

            if (showSuccessToast)
            {
                await _toastService.ShowAsync("Berechnung abgeschlossen.");
            }

            return true;
        }
        catch (StsValidationException ex)
        {
            ReplaceValidationErrors(ex.Errors);
            await _toastService.ShowAsync("Eingabefehler vorhanden.");
            return false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            await _toastService.ShowAsync("Berechnung fehlgeschlagen.");
            return false;
        }
        finally
        {
            IsBusy = false;
            UpdateCommandStates();
        }
    }

    private async Task SaveAsync()
    {
        if (Current is null)
        {
            return;
        }

        var computed = await ComputeInternalAsync(showSuccessToast: false);
        if (!computed)
        {
            return;
        }

        var targetId = Current.Id;

        try
        {
            IsBusy = true;
            UpdateCommandStates();

            await using var db = await _dbFactory.CreateDbContextAsync();

            var existing = await db.StsTests
                .Include(t => t.Siebanalysen)
                .Include(t => t.Kornform)
                .Include(t => t.Kochversuch)
                .Include(t => t.Ergebnis)
                .FirstOrDefaultAsync(t => t.Id == Current.Id);

            if (existing is null)
            {
                var model = CloneForPersistence(Current);
                db.StsTests.Add(model);
            }
            else
            {
                db.Entry(existing).CurrentValues.SetValues(Current);

                SynchronizeFractions(existing, Current);
                SynchronizeKornform(existing, Current);
                SynchronizeKochversuch(existing, Current);
                SynchronizeErgebnis(existing, Current);
            }

            await db.SaveChangesAsync();
            await _toastService.ShowAsync("STS-Test gespeichert.");

            await LoadAsync(force: true, selectId: targetId);
            ReplaceValidationErrors(Array.Empty<string>());
        }
        catch (StsValidationException ex)
        {
            await _toastService.ShowAsync(ex.Message);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            await _toastService.ShowAsync("Speichern fehlgeschlagen.");
        }
        finally
        {
            IsBusy = false;
            UpdateCommandStates();
        }
    }

    private async Task ExportPdfAsync()
    {
        if (Current is null)
        {
            return;
        }

        var computed = await ComputeInternalAsync(showSuccessToast: false);
        if (!computed)
        {
            return;
        }

        try
        {
            IsBusy = true;
            UpdateCommandStates();

            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var targetDir = Path.Combine(documents, "BLE", "Reports");
            Directory.CreateDirectory(targetDir);

            var safeCode = string.IsNullOrWhiteSpace(Current.Probencode) ? "unbenannt" : Current.Probencode.Replace(' ', '_');
            var fileName = $"STS_{safeCode}_{DateTime.Now:yyyyMMdd_HHmm}.pdf";
            var outputPath = Path.Combine(targetDir, fileName);

            _pdfService.GenerateStsReport(Current, outputPath);
            await _toastService.ShowAsync($"PDF erzeugt: {outputPath}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            await _toastService.ShowAsync("PDF-Export fehlgeschlagen.");
        }
        finally
        {
            IsBusy = false;
            UpdateCommandStates();
        }
    }

    private void UpdateCurrentModelFromForm()
    {
        if (Current is null)
        {
            return;
        }

        Current.Siebanalysen = _fractions
            .Select(f => new StsSiebanalyse
            {
                Id = f.Id,
                StsTestId = Current.Id,
                SiebBezeichnung = f.SiebBezeichnung,
                Einwaage = f.Einwaage,
                Rueckwaage = f.Rueckwaage,
                DurchgangProzent = f.DurchgangProzent,
                RueckgangProzent = f.RueckgangProzent
            })
            .ToList();

        Current.Kornform ??= new StsKornform();
        Current.Kornform.StsTestId = Current.Id;

        Current.Kochversuch ??= new StsKochversuch();
        Current.Kochversuch.StsTestId = Current.Id;

        Current.Ergebnis ??= new StsErgebnis { StsTestId = Current.Id };
    }

    private void ReplaceValidationErrors(IEnumerable<string>? errors)
    {
        _validationErrors.Clear();
        if (errors is not null)
        {
            foreach (var error in errors)
            {
                if (!string.IsNullOrWhiteSpace(error))
                {
                    _validationErrors.Add(error);
                }
            }
        }

        OnPropertyChanged(nameof(ValidationErrors));
        OnPropertyChanged(nameof(HasValidationErrors));
    }

    private static void SynchronizeFractions(StsTest existing, StsTest source)
    {
        var currentFractions = source.Siebanalysen?.ToList() ?? new List<StsSiebanalyse>();
        var toRemove = existing.Siebanalysen.Where(f => currentFractions.All(cf => cf.Id == 0 || cf.Id != f.Id)).ToList();
        foreach (var remove in toRemove)
        {
            existing.Siebanalysen.Remove(remove);
        }

        foreach (var fraction in currentFractions)
        {
            var target = existing.Siebanalysen.FirstOrDefault(f => f.Id == fraction.Id && fraction.Id != 0);
            if (target is null)
            {
                existing.Siebanalysen.Add(new StsSiebanalyse
                {
                    StsTestId = existing.Id,
                    SiebBezeichnung = fraction.SiebBezeichnung,
                    Einwaage = fraction.Einwaage,
                    Rueckwaage = fraction.Rueckwaage,
                    DurchgangProzent = fraction.DurchgangProzent,
                    RueckgangProzent = fraction.RueckgangProzent
                });
            }
            else
            {
                target.SiebBezeichnung = fraction.SiebBezeichnung;
                target.Einwaage = fraction.Einwaage;
                target.Rueckwaage = fraction.Rueckwaage;
                target.DurchgangProzent = fraction.DurchgangProzent;
                target.RueckgangProzent = fraction.RueckgangProzent;
            }
        }
    }

    private static void SynchronizeKornform(StsTest existing, StsTest source)
    {
        if (source.Kornform is null)
        {
            existing.Kornform = null;
            return;
        }

        if (existing.Kornform is null)
        {
            existing.Kornform = new StsKornform();
        }

        existing.Kornform.StsTestId = existing.Id;
        existing.Kornform.EinwaageGesamt = source.Kornform.EinwaageGesamt;
        existing.Kornform.EinwaageSchlechtGeformt = source.Kornform.EinwaageSchlechtGeformt;
    }

    private static void SynchronizeKochversuch(StsTest existing, StsTest source)
    {
        if (source.Kochversuch is null)
        {
            existing.Kochversuch = null;
            return;
        }

        if (existing.Kochversuch is null)
        {
            existing.Kochversuch = new StsKochversuch();
        }

        existing.Kochversuch.StsTestId = existing.Id;
        existing.Kochversuch.EinwaageVorKochen = source.Kochversuch.EinwaageVorKochen;
        existing.Kochversuch.RueckwaageNachKochen = source.Kochversuch.RueckwaageNachKochen;
        existing.Kochversuch.Kochzeit = source.Kochversuch.Kochzeit;
    }

    private static void SynchronizeErgebnis(StsTest existing, StsTest source)
    {
        if (source.Ergebnis is null)
        {
            existing.Ergebnis = null;
            return;
        }

        if (existing.Ergebnis is null)
        {
            existing.Ergebnis = new StsErgebnis();
        }

        existing.Ergebnis.StsTestId = existing.Id;
        existing.Ergebnis.S1 = source.Ergebnis.S1;
        existing.Ergebnis.S2 = source.Ergebnis.S2;
        existing.Ergebnis.KornformIndex = source.Ergebnis.KornformIndex;
        existing.Ergebnis.GrenzwerteOk = source.Ergebnis.GrenzwerteOk;
    }

    private static StsTest CloneForEditing(StsTest source)
    {
        var clone = new StsTest
        {
            Id = source.Id,
            Probencode = source.Probencode,
            Entnahmedatum = source.Entnahmedatum,
            Werk = source.Werk,
            Produkt = source.Produkt,
            Materialtyp = source.Materialtyp,
            GesamtEinwaage = source.GesamtEinwaage,
            Status = source.Status,
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt,
            Siebanalysen = source.Siebanalysen?
                .Select(f => new StsSiebanalyse
                {
                    Id = f.Id,
                    StsTestId = source.Id,
                    SiebBezeichnung = f.SiebBezeichnung,
                    Einwaage = f.Einwaage,
                    Rueckwaage = f.Rueckwaage,
                    DurchgangProzent = f.DurchgangProzent,
                    RueckgangProzent = f.RueckgangProzent
                })
                .ToList() ?? new List<StsSiebanalyse>(),
            Kornform = source.Kornform is null
                ? new StsKornform()
                : new StsKornform
                {
                    Id = source.Kornform.Id,
                    StsTestId = source.Id,
                    EinwaageGesamt = source.Kornform.EinwaageGesamt,
                    EinwaageSchlechtGeformt = source.Kornform.EinwaageSchlechtGeformt
                },
            Kochversuch = source.Kochversuch is null
                ? new StsKochversuch()
                : new StsKochversuch
                {
                    Id = source.Kochversuch.Id,
                    StsTestId = source.Id,
                    EinwaageVorKochen = source.Kochversuch.EinwaageVorKochen,
                    RueckwaageNachKochen = source.Kochversuch.RueckwaageNachKochen,
                    Kochzeit = source.Kochversuch.Kochzeit
                },
            Ergebnis = source.Ergebnis is null
                ? new StsErgebnis { StsTestId = source.Id }
                : new StsErgebnis
                {
                    StsTestId = source.Id,
                    S1 = source.Ergebnis.S1,
                    S2 = source.Ergebnis.S2,
                    KornformIndex = source.Ergebnis.KornformIndex,
                    GrenzwerteOk = source.Ergebnis.GrenzwerteOk
                }
        };

        return clone;
    }

    private static StsTest CloneForPersistence(StsTest source)
    {
        return CloneForEditing(source);
    }

    private void UpdateCommandStates()
    {
        ((Command)LoadCommand).ChangeCanExecute();
        ((Command)NewCommand).ChangeCanExecute();
        ((Command)OpenCommand).ChangeCanExecute();
        _addFractionCommand.ChangeCanExecute();
        _removeFractionCommand.ChangeCanExecute();
        _computeCommand.ChangeCanExecute();
        _saveCommand.ChangeCanExecute();
        _exportPdfCommand.ChangeCanExecute();
    }
}
