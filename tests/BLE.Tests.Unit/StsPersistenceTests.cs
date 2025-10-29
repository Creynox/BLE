using System;
using System.Linq;
using System.Threading.Tasks;
using BLE.Data;
using BLE.Domain.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BLE.Tests.Unit;

public class StsPersistenceTests
{
    [Fact]
    public async Task Can_Persist_And_Load_StsTest_With_Related_Data()
    {
        using var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<BLEDbContext>()
            .UseSqlite(connection)
            .Options;

        var stsTestId = Guid.NewGuid();

        using (var arrangeContext = new BLEDbContext(options))
        {
            arrangeContext.Database.EnsureCreated();

            var test = new StsTest
            {
                Id = stsTestId,
                Probencode = "STS-001",
                Entnahmedatum = new DateTime(2025, 10, 29),
                Werk = "Werk A",
                Produkt = "Sandsplitt 0-45",
                Materialtyp = StsMaterialtyp.Asphalt,
                GesamtEinwaage = 1000m,
                Status = "erfasst",
                Kornform = new StsKornform
                {
                    EinwaageGesamt = 200m,
                    EinwaageSchlechtGeformt = 25m
                },
                Kochversuch = new StsKochversuch
                {
                    EinwaageVorKochen = 150m,
                    RueckwaageNachKochen = 145m,
                    Kochzeit = TimeSpan.FromMinutes(30)
                },
                Ergebnis = new StsErgebnis
                {
                    S1 = 12.3m,
                    S2 = 8.7m,
                    KornformIndex = 0.85m,
                    GrenzwerteOk = true
                }
            };

            test.Siebanalysen.Add(new StsSiebanalyse
            {
                SiebBezeichnung = "45 mm",
                Einwaage = 200m,
                Rueckwaage = 20m,
                DurchgangProzent = 90m,
                RueckgangProzent = 10m
            });

            test.Siebanalysen.Add(new StsSiebanalyse
            {
                SiebBezeichnung = "0.063 mm",
                Einwaage = 150m,
                Rueckwaage = 5m,
                DurchgangProzent = 96m,
                RueckgangProzent = 4m
            });

            arrangeContext.StsTests.Add(test);
            await arrangeContext.SaveChangesAsync();
        }

        using var assertContext = new BLEDbContext(options);

        var loaded = await assertContext.StsTests
            .Include(x => x.Siebanalysen)
            .Include(x => x.Kornform)
            .Include(x => x.Kochversuch)
            .Include(x => x.Ergebnis)
            .SingleAsync();

        Assert.Equal(stsTestId, loaded.Id);
        Assert.Equal("STS-001", loaded.Probencode);
        Assert.Equal(2, loaded.Siebanalysen.Count);
        Assert.Contains(loaded.Siebanalysen, s => s.SiebBezeichnung == "45 mm");
        Assert.NotNull(loaded.Kornform);
        Assert.Equal(25m, loaded.Kornform!.EinwaageSchlechtGeformt);
        Assert.NotNull(loaded.Kochversuch);
        Assert.Equal(TimeSpan.FromMinutes(30), loaded.Kochversuch!.Kochzeit);
        Assert.NotNull(loaded.Ergebnis);
        Assert.True(loaded.Ergebnis!.GrenzwerteOk);
    }
}
