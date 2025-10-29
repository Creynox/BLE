using System;
using System.Collections.Generic;
using System.Linq;
using BLE.Domain.Entities;
using BLE.Services.Sts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace BLE.Tests.Unit;

public class StsEvaluationServiceTests
{
    [Fact]
    public void Berechnet_Prozente_und_Ergebnis_Korrekt()
    {
        var service = CreateService();
        var test = CreateValidTest();

        var ergebnis = service.BerechneErgebnis(test);

        Assert.NotNull(test.Ergebnis);
        Assert.Same(test.Ergebnis, ergebnis);

        AssertDecimal(88.89m, GetFraction(test, "56 mm").DurchgangProzent);
        AssertDecimal(11.11m, GetFraction(test, "56 mm").RueckgangProzent);
        AssertDecimal(65.56m, ergebnis.S1);
        AssertDecimal(34.44m, ergebnis.S2);
        AssertDecimal(12m, ergebnis.KornformIndex);
        Assert.True(ergebnis.GrenzwerteOk);

        var sumRueckgang = test.Siebanalysen.Sum(f => f.RueckgangProzent);
        Assert.InRange(sumRueckgang, 99.5m, 100.5m);
    }

    [Fact]
    public void Markiert_Grenzwertverletzungen()
    {
        var limits = new StsGrenzwerte
        {
            S1Min = 0m,
            S1Max = 100m,
            S2Min = 0m,
            S2Max = 30m,
            KornformIndexMax = 20m
        };

        var service = CreateService(limits);
        var test = CreateValidTest();

        var ergebnis = service.BerechneErgebnis(test);

        Assert.False(ergebnis.GrenzwerteOk);
        Assert.True(ergebnis.S2 > limits.S2Max);
    }

    [Fact]
    public void Meldet_Validierungsfehler_bei_Unplausiblen_Daten()
    {
        var service = CreateService();
        var test = new StsTest
        {
            Id = Guid.NewGuid(),
            Probencode = "STS-ERR",
            GesamtEinwaage = 100m
        };

        test.Siebanalysen.Add(new StsSiebanalyse
        {
            SiebBezeichnung = "4 mm",
            Einwaage = 50m,
            Rueckwaage = 60m
        });

        var errors = service.Validieren(test);
        Assert.NotEmpty(errors);
        Assert.Contains(errors, e => e.Contains("4 mm", StringComparison.Ordinal));
        Assert.Throws<StsValidationException>(() => service.BerechneErgebnis(test));
    }

    [Fact]
    public void Meldet_Validierungsfehler_fuer_Kochversuch()
    {
        var service = CreateService();
        var test = CreateValidTest();
        test.Kochversuch = new StsKochversuch
        {
            EinwaageVorKochen = 10m,
            RueckwaageNachKochen = 15m,
            Kochzeit = TimeSpan.FromMinutes(10)
        };

        var errors = service.Validieren(test);
        Assert.NotEmpty(errors);
        Assert.Contains(errors, e => e.Contains("Kochversuch", StringComparison.Ordinal));
    }

    [Fact]
    public void Service_Ist_Ueber_DI_Verfuegbar()
    {
        var services = new ServiceCollection();
        services.AddOptions();
        services.Configure<StsGrenzwerte>(_ => { });
        services.AddScoped<IStsEvaluationService, StsEvaluationService>();

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        var service = scope.ServiceProvider.GetRequiredService<IStsEvaluationService>();
        var test = CreateValidTest();

        var ergebnis = service.BerechneErgebnis(test);

        Assert.True(ergebnis.GrenzwerteOk);
    }

    private static StsTest CreateValidTest()
    {
        var test = new StsTest
        {
            Id = Guid.NewGuid(),
            Probencode = "STS-100",
            GesamtEinwaage = 450m,
            Materialtyp = StsMaterialtyp.Asphalt,
            Kornform = new StsKornform
            {
                EinwaageGesamt = 200m,
                EinwaageSchlechtGeformt = 24m
            },
            Kochversuch = new StsKochversuch
            {
                EinwaageVorKochen = 50m,
                RueckwaageNachKochen = 40m,
                Kochzeit = TimeSpan.FromMinutes(20)
            }
        };

        foreach (var fraction in CreateFractions())
        {
            test.Siebanalysen.Add(fraction);
        }

        return test;
    }

    private static IEnumerable<StsSiebanalyse> CreateFractions()
    {
        yield return new StsSiebanalyse { SiebBezeichnung = "56 mm", Einwaage = 450m, Rueckwaage = 50m };
        yield return new StsSiebanalyse { SiebBezeichnung = "32 mm", Einwaage = 400m, Rueckwaage = 40m };
        yield return new StsSiebanalyse { SiebBezeichnung = "16 mm", Einwaage = 360m, Rueckwaage = 30m };
        yield return new StsSiebanalyse { SiebBezeichnung = "8 mm", Einwaage = 330m, Rueckwaage = 35m };
        yield return new StsSiebanalyse { SiebBezeichnung = "< 5,6 mm", Einwaage = 295m, Rueckwaage = 60m };
        yield return new StsSiebanalyse { SiebBezeichnung = "2 mm", Einwaage = 235m, Rueckwaage = 80m };
        yield return new StsSiebanalyse { SiebBezeichnung = "0,063 mm", Einwaage = 155m, Rueckwaage = 70m };
        yield return new StsSiebanalyse { SiebBezeichnung = "< 0,063 mm", Einwaage = 85m, Rueckwaage = 85m };
    }

    private static StsSiebanalyse GetFraction(StsTest test, string label) =>
        test.Siebanalysen.Single(x => string.Equals(x.SiebBezeichnung, label, StringComparison.Ordinal));

    private static IStsEvaluationService CreateService(StsGrenzwerte? limits = null) =>
        new StsEvaluationService(Options.Create(limits ?? new StsGrenzwerte
        {
            S1Min = 50m,
            S1Max = 80m,
            S2Min = 20m,
            S2Max = 40m,
            KornformIndexMax = 15m
        }));

    private static void AssertDecimal(decimal expected, decimal actual) =>
        Assert.Equal(expected, actual);
}
