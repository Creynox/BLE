using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using BLE.Domain.Entities;
using Microsoft.Extensions.Options;

namespace BLE.Services.Sts;

public class StsEvaluationService : IStsEvaluationService
{
    private static readonly Regex SieveSizeRegex = new(@"\d+([.,]\d+)?", RegexOptions.Compiled);
    private readonly StsGrenzwerte limits;

    public StsEvaluationService(IOptions<StsGrenzwerte> options)
    {
        limits = options?.Value ?? new StsGrenzwerte();
    }

    public StsErgebnis BerechneErgebnis(StsTest test)
    {
        var context = BuildContext(test);
        if (context.Errors.Count > 0)
        {
            throw new StsValidationException(context.Errors);
        }

        ApplyPercentages(context);

        var s1 = SumRueckgang(context, 5.6m, inclusive: false);
        var s2 = SumRueckgang(context, 0.063m, inclusive: true);
        var kornformIndex = CalculateKornformIndex(test);

        var result = test.Ergebnis ?? new StsErgebnis();
        result.StsTestId = test.Id;
        result.S1 = s1;
        result.S2 = s2;
        result.KornformIndex = kornformIndex ?? 0m;
        result.GrenzwerteOk = EvaluateLimits(s1, s2, kornformIndex);

        test.Ergebnis = result;

        return result;
    }

    public void BerechneSiebanalyse(StsTest test)
    {
        var context = BuildContext(test);
        if (context.Errors.Count > 0)
        {
            throw new StsValidationException(context.Errors);
        }

        ApplyPercentages(context);
    }

    public IReadOnlyList<string> Validieren(StsTest test)
    {
        var context = BuildContext(test);
        return context.Errors.AsReadOnly();
    }

    private static void ApplyPercentages(StsCalculationContext context)
    {
        foreach (var fraction in context.Fractions)
        {
            fraction.Source.DurchgangProzent = Round2(fraction.DurchgangProzent);
            fraction.Source.RueckgangProzent = Round2(fraction.RueckgangProzent);
        }
    }

    private bool EvaluateLimits(decimal s1, decimal s2, decimal? kornformIndex)
    {
        if (limits.S1Min.HasValue && s1 < limits.S1Min.Value)
        {
            return false;
        }

        if (limits.S1Max.HasValue && s1 > limits.S1Max.Value)
        {
            return false;
        }

        if (limits.S2Min.HasValue && s2 < limits.S2Min.Value)
        {
            return false;
        }

        if (limits.S2Max.HasValue && s2 > limits.S2Max.Value)
        {
            return false;
        }

        if (kornformIndex.HasValue && limits.KornformIndexMax.HasValue && kornformIndex.Value > limits.KornformIndexMax.Value)
        {
            return false;
        }

        return true;
    }

    private static decimal SumRueckgang(StsCalculationContext context, decimal threshold, bool inclusive)
    {
        var sum = context.Fractions
            .Where(f => MatchesThreshold(f.Source, threshold, inclusive))
            .Sum(f => f.RueckgangProzent);

        return Round2(sum);
    }

    private static decimal? CalculateKornformIndex(StsTest test)
    {
        if (test.Kornform is null)
        {
            return null;
        }

        if (test.Kornform.EinwaageGesamt <= 0 || test.Kornform.EinwaageSchlechtGeformt < 0)
        {
            return null;
        }

        var index = (test.Kornform.EinwaageSchlechtGeformt / test.Kornform.EinwaageGesamt) * 100m;
        return Round2(index);
    }

    private static bool MatchesThreshold(StsSiebanalyse fraction, decimal threshold, bool inclusive)
    {
        if (fraction is null)
        {
            return false;
        }

        if (!TryExtractSieveSize(fraction.SiebBezeichnung, out var size))
        {
            return false;
        }

        var isLessExpression = fraction.SiebBezeichnung?.Contains("<", StringComparison.Ordinal) ?? false;

        if (inclusive)
        {
            return size <= threshold;
        }

        return isLessExpression ? size <= threshold : size < threshold;
    }

    private static bool TryExtractSieveSize(string? label, out decimal size)
    {
        size = 0m;
        if (string.IsNullOrWhiteSpace(label))
        {
            return false;
        }

        var match = SieveSizeRegex.Match(label);
        if (!match.Success)
        {
            return false;
        }

        var normalized = match.Value.Replace(',', '.');
        return decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.InvariantCulture, out size);
    }

    private static StsCalculationContext BuildContext(StsTest? test)
    {
        if (test is null)
        {
            throw new ArgumentNullException(nameof(test));
        }

        var context = new StsCalculationContext(test);

        var siebanalysen = test.Siebanalysen?.Where(x => x is not null).ToList() ?? new List<StsSiebanalyse>();
        if (siebanalysen.Count == 0)
        {
            context.Errors.Add("Es wurden keine Siebanalyse-Fraktionen hinterlegt.");
            return context;
        }

        var total = ResolveTotal(test, siebanalysen);
        if (total <= 0)
        {
            context.Errors.Add("Die Gesamt-Einwaage muss größer als 0 sein.");
            return context;
        }

        context.Total = total;

        decimal sumRueckProzent = 0m;
        foreach (var fraction in siebanalysen)
        {
            var label = string.IsNullOrWhiteSpace(fraction.SiebBezeichnung)
                ? "<unbenannt>"
                : fraction.SiebBezeichnung!;

            if (fraction.Einwaage < 0)
            {
                context.Errors.Add($"Fraktion '{label}' besitzt eine negative Einwaage.");
            }

            if (fraction.Rueckwaage < 0)
            {
                context.Errors.Add($"Fraktion '{label}' besitzt eine negative Rückwaage.");
            }

            if (fraction.Einwaage > total)
            {
                context.Errors.Add($"Fraktion '{label}' überschreitet die Gesamt-Einwaage ({total}).");
            }

            if (fraction.Rueckwaage > fraction.Einwaage)
            {
                context.Errors.Add($"Fraktion '{label}' hat eine Rückwaage, die größer als die Einwaage ist.");
            }

            var passMass = fraction.Einwaage - fraction.Rueckwaage;
            if (passMass < 0)
            {
                context.Errors.Add($"Fraktion '{label}' würde einen negativen Durchgang ergeben.");
            }

            var passPercent = total > 0 ? (passMass / total) * 100m : 0m;
            var backPercent = total > 0 ? (fraction.Rueckwaage / total) * 100m : 0m;

            sumRueckProzent += backPercent;

            context.Fractions.Add(new StsCalculatedFraction(fraction, passPercent, backPercent));
        }

        if (Math.Abs(sumRueckProzent - 100m) > 0.5m)
        {
            context.Errors.Add($"Die Summe der Rückgangsprozente liegt bei {sumRueckProzent:F2} % und damit außerhalb der Toleranz von 100 ± 0,5 %.");
        }

        ValidateKornform(test, context);
        ValidateKochversuch(test, context);

        return context;
    }

    private static void ValidateKornform(StsTest test, StsCalculationContext context)
    {
        if (test.Kornform is null)
        {
            return;
        }

        var kornform = test.Kornform;
        if (kornform.EinwaageGesamt < 0)
        {
            context.Errors.Add("Die Kornform-Gesamteinwaage darf nicht negativ sein.");
        }

        if (kornform.EinwaageSchlechtGeformt < 0)
        {
            context.Errors.Add("Die Kornform-Einwaage schlechter Kornformen darf nicht negativ sein.");
        }

        if (kornform.EinwaageSchlechtGeformt > kornform.EinwaageGesamt)
        {
            context.Errors.Add("Die Kornform-Einwaage schlechter Kornformen darf die Gesamteinwaage nicht überschreiten.");
        }
    }

    private static void ValidateKochversuch(StsTest test, StsCalculationContext context)
    {
        if (test.Kochversuch is null)
        {
            return;
        }

        var koch = test.Kochversuch;
        if (koch.EinwaageVorKochen < 0)
        {
            context.Errors.Add("Die Einwaage vor dem Kochversuch darf nicht negativ sein.");
        }

        if (koch.RueckwaageNachKochen < 0)
        {
            context.Errors.Add("Die Rückwaage nach dem Kochversuch darf nicht negativ sein.");
        }

        if (koch.RueckwaageNachKochen > koch.EinwaageVorKochen)
        {
            context.Errors.Add("Die Rückwaage nach dem Kochversuch darf die Einwaage vor dem Kochen nicht überschreiten.");
        }
    }

    private static decimal ResolveTotal(StsTest test, IReadOnlyCollection<StsSiebanalyse> siebanalysen)
    {
        if (test.GesamtEinwaage.HasValue && test.GesamtEinwaage.Value > 0)
        {
            return test.GesamtEinwaage.Value;
        }

        return siebanalysen.Sum(x => x.Einwaage);
    }

    private static decimal Round2(decimal value) =>
        Math.Round(value, 2, MidpointRounding.AwayFromZero);

    private sealed class StsCalculationContext
    {
        public StsCalculationContext(StsTest test)
        {
            Test = test;
        }

        public StsTest Test { get; }

        public decimal Total { get; set; }

        public List<string> Errors { get; } = new();

        public List<StsCalculatedFraction> Fractions { get; } = new();
    }

    private sealed class StsCalculatedFraction
    {
        public StsCalculatedFraction(StsSiebanalyse source, decimal durchgangProzent, decimal rueckgangProzent)
        {
            Source = source;
            DurchgangProzent = durchgangProzent;
            RueckgangProzent = rueckgangProzent;
        }

        public StsSiebanalyse Source { get; }

        public decimal DurchgangProzent { get; }

        public decimal RueckgangProzent { get; }
    }
}
