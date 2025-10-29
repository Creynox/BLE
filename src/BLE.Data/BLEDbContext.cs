using System;
using BLE.Data;
using BLE.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BLE.Data;

public class BLEDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public BLEDbContext(DbContextOptions<BLEDbContext> options) : base(options) { }

    public DbSet<Normwerk> Normwerke => Set<Normwerk>();
    public DbSet<Pruefverfahren> Pruefverfahren => Set<Pruefverfahren>();
    public DbSet<Grenzwert> Grenzwerte => Set<Grenzwert>();
    public DbSet<Formel> Formeln => Set<Formel>();
    public DbSet<Kunde> Kunden => Set<Kunde>();
    public DbSet<Projekt> Projekte => Set<Projekt>();
    public DbSet<Probe> Proben => Set<Probe>();
    public DbSet<Messung> Messungen => Set<Messung>();
    public DbSet<Kornfraktion> Kornfraktionen => Set<Kornfraktion>();
    public DbSet<EtlFile> EtlFiles => Set<EtlFile>();
    public DbSet<EtlLog> EtlLogs => Set<EtlLog>();
    public DbSet<StsTest> StsTests => Set<StsTest>();
    public DbSet<StsSiebanalyse> StsSiebanalysen => Set<StsSiebanalyse>();
    public DbSet<StsKornform> StsKornformen => Set<StsKornform>();
    public DbSet<StsKochversuch> StsKochversuche => Set<StsKochversuch>();
    public DbSet<StsErgebnis> StsErgebnisse => Set<StsErgebnis>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        b.Entity<Probe>().HasIndex(x => x.Probencode).IsUnique();
        b.Entity<Probe>().Property(x => x.Materialtyp).HasDefaultValue("asphalt");
        b.Entity<Probe>().HasCheckConstraint("ck_probe_materialtyp", "materialtyp IN ('asphalt','beton')");

        b.Entity<Messung>().HasIndex(x => x.ProbeId).HasDatabaseName("ix_messung_probe");
        b.Entity<Messung>().HasIndex(x => x.PruefverfahrenId).HasDatabaseName("ix_messung_pruef");

        b.Entity<Grenzwert>().Property(x => x.Scope).HasDefaultValue("global");

        // Simple required configuration for strings length could be added here

        var materialtypConverter = new EnumToStringConverter<StsMaterialtyp>();

        b.Entity<StsTest>(entity =>
        {
            entity.Property(x => x.Probencode).HasMaxLength(128);
            entity.Property(x => x.Werk).HasMaxLength(128);
            entity.Property(x => x.Produkt).HasMaxLength(128);
            entity.Property(x => x.Status).HasMaxLength(64);
            entity.Property(x => x.Materialtyp)
                .HasConversion(materialtypConverter)
                .HasMaxLength(32);

            entity.HasCheckConstraint("ck_ststest_materialtyp", "materialtyp IN ('Asphalt','Beton')");
            entity.HasMany(x => x.Siebanalysen)
                .WithOne(x => x.StsTest!)
                .HasForeignKey(x => x.StsTestId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Kornform)
                .WithOne(x => x.StsTest!)
                .HasForeignKey<StsKornform>(x => x.StsTestId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Kochversuch)
                .WithOne(x => x.StsTest!)
                .HasForeignKey<StsKochversuch>(x => x.StsTestId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Ergebnis)
                .WithOne(x => x.StsTest!)
                .HasForeignKey<StsErgebnis>(x => x.StsTestId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        b.Entity<StsSiebanalyse>(entity =>
        {
            entity.Property(x => x.SiebBezeichnung).HasMaxLength(64);
            entity.HasIndex(x => new { x.StsTestId, x.SiebBezeichnung }).IsUnique();
        });
    }
}
