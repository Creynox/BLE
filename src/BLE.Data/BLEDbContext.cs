using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BLE.Data;
using BLE.Data.Seed;
using BLE.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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
    public DbSet<Werk> WorkspaceWerke => Set<Werk>();
    public DbSet<Produkt> WorkspaceProdukte => Set<Produkt>();
    public DbSet<Materialtyp> WorkspaceMaterialtypen => Set<Materialtyp>();
    public DbSet<PruefplanVorlage> WorkspacePruefplanVorlagen => Set<PruefplanVorlage>();
    public DbSet<PruefplanVorlageItem> WorkspacePruefplanVorlageItems => Set<PruefplanVorlageItem>();
    public DbSet<Pruefauftrag> WorkspacePruefauftraege => Set<Pruefauftrag>();
    public DbSet<AuditLog> WorkspaceAuditLogs => Set<AuditLog>();

    public DbSet<StsSiebanalyse> StsSiebanalysen => Set<StsSiebanalyse>();
    public DbSet<StsKornform> StsKornformen => Set<StsKornform>();
    public DbSet<StsKochversuch> StsKochversuche => Set<StsKochversuch>();
    public DbSet<StsErgebnis> StsErgebnisse => Set<StsErgebnis>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        var stsMaterialtypConverter = new EnumToStringConverter<StsMaterialtyp>();
        var produktKategorieConverter = new EnumToStringConverter<ProduktKategorie>();
        var probeStatusConverter = new EnumToStringConverter<ProbeStatus>();
        var auftragStatusConverter = new EnumToStringConverter<AuftragStatus>();

        b.Entity<Werk>(entity =>
        {
            entity.ToTable("Workspace_Werk");
            entity.Property(x => x.Name).IsRequired().HasMaxLength(100);
            entity.HasIndex(x => x.Name).IsUnique();
        });

        b.Entity<Materialtyp>(entity =>
        {
            entity.ToTable("Workspace_Materialtyp");
            entity.Property(x => x.Name).IsRequired().HasMaxLength(80);
        });

        b.Entity<Produkt>(entity =>
        {
            entity.ToTable("Workspace_Produkt");
            entity.Property(x => x.Name).IsRequired().HasMaxLength(120);
            entity.Property(x => x.SiebbereichText).HasMaxLength(60);
            entity.Property(x => x.Kategorie)
                .HasConversion(produktKategorieConverter)
                .HasMaxLength(32);
            entity.HasIndex(x => new { x.WerkId, x.Name }).IsUnique();
            entity.HasOne(x => x.Werk)
                .WithMany(x => x.Produkte)
                .HasForeignKey(x => x.WerkId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        b.Entity<Pruefverfahren>(entity =>
        {
            entity.ToTable("Pruefverfahren");
            entity.Property(x => x.Code).HasMaxLength(20);
            entity.Property(x => x.Name).HasMaxLength(120);
            entity.Property(x => x.Titel).HasMaxLength(120);
            entity.HasIndex(x => x.Code).IsUnique();
        });

        b.Entity<PruefplanVorlage>(entity =>
        {
            entity.ToTable("Workspace_PruefplanVorlage");
            entity.Property(x => x.Name).IsRequired().HasMaxLength(140);
            entity.Property(x => x.Version).IsRequired().HasMaxLength(40);
            entity.Property(x => x.GueltigAbUtc).HasColumnType("datetime2");
            entity.Property(x => x.GueltigBisUtc).HasColumnType("datetime2");
            entity.HasOne(x => x.Werk)
                .WithMany(x => x.PruefplanVorlagen)
                .HasForeignKey(x => x.WerkId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.Produkt)
                .WithMany(x => x.PruefplanVorlagen)
                .HasForeignKey(x => x.ProduktId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.Materialtyp)
                .WithMany(x => x.PruefplanVorlagen)
                .HasForeignKey(x => x.MaterialtypId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(x => new { x.WerkId, x.ProduktId, x.MaterialtypId, x.Version })
                .HasDatabaseName("ux_workspace_vorlage_scope")
                .IsUnique();
        });

        b.Entity<PruefplanVorlageItem>(entity =>
        {
            entity.ToTable("Workspace_PruefplanVorlageItem");
            entity.Property(x => x.Pflicht).HasDefaultValue(true);
            entity.HasOne(x => x.Vorlage)
                .WithMany(x => x.Items)
                .HasForeignKey(x => x.VorlageId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Pruefverfahren)
                .WithMany(x => x.VorlageItems)
                .HasForeignKey(x => x.PruefverfahrenId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(x => new { x.VorlageId, x.PruefverfahrenId })
                .IsUnique();
        });

        b.Entity<Pruefauftrag>(entity =>
        {
            entity.ToTable("Workspace_Pruefauftrag");
            entity.Property(x => x.Status)
                .HasConversion(auftragStatusConverter)
                .HasMaxLength(40)
                .HasDefaultValue(AuftragStatus.Offen);
            entity.Property(x => x.StartedAtUtc).HasColumnType("datetime2");
            entity.Property(x => x.CompletedAtUtc).HasColumnType("datetime2");
            entity.HasOne(x => x.Probe)
                .WithMany(x => x.Pruefauftraege)
                .HasForeignKey(x => x.ProbeId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Pruefverfahren)
                .WithMany(x => x.Pruefauftraege)
                .HasForeignKey(x => x.PruefverfahrenId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(x => new { x.ProbeId, x.PruefverfahrenId }).IsUnique();
        });

        b.Entity<AuditLog>(entity =>
        {
            entity.ToTable("Workspace_AuditLog");
            entity.Property(x => x.Entity).IsRequired().HasMaxLength(80);
            entity.Property(x => x.Action).IsRequired().HasMaxLength(60);
            entity.Property(x => x.AtUtc).HasColumnType("datetime2");
        });

        b.Entity<Probe>(entity =>
        {
            entity.HasIndex(x => x.Probencode).IsUnique();
            entity.Property(x => x.Materialtyp).HasDefaultValue("asphalt");
            entity.HasCheckConstraint("ck_probe_materialtyp", "materialtyp IN ('asphalt','beton')");
            entity.Property(x => x.Status)
                .HasConversion(probeStatusConverter)
                .HasMaxLength(32)
                .HasDefaultValue(ProbeStatus.Neu);
            entity.Property(x => x.CreatedAtUtc)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETUTCDATE()");
            entity.Property(x => x.UpdatedAtUtc).HasColumnType("datetime2");
            entity.Property(x => x.EingangAmUtc).HasColumnType("datetime2");
            entity.HasIndex(x => new { x.WerkId, x.ProduktId, x.MaterialtypId, x.Status })
                .HasDatabaseName("ix_workspace_probe_scope");
            entity.HasOne(x => x.WorkspaceWerk)
                .WithMany(x => x.Proben)
                .HasForeignKey(x => x.WerkId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.WorkspaceProdukt)
                .WithMany(x => x.Proben)
                .HasForeignKey(x => x.ProduktId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.WorkspaceMaterialtyp)
                .WithMany(x => x.Proben)
                .HasForeignKey(x => x.MaterialtypId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        b.Entity<Messung>().HasIndex(x => x.ProbeId).HasDatabaseName("ix_messung_probe");
        b.Entity<Messung>().HasIndex(x => x.PruefverfahrenId).HasDatabaseName("ix_messung_pruef");

        b.Entity<Grenzwert>().Property(x => x.Scope).HasDefaultValue("global");

        b.Entity<StsTest>(entity =>
        {
            entity.Property(x => x.Probencode).HasMaxLength(128);
            entity.Property(x => x.Werk).HasMaxLength(128);
            entity.Property(x => x.Produkt).HasMaxLength(128);
            entity.Property(x => x.Status).HasMaxLength(64);
            entity.Property(x => x.Materialtyp)
                .HasConversion(stsMaterialtypConverter)
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

        b.Entity<Pruefverfahren>().HasData(WorkspaceSeedData.Pruefverfahren);
        b.Entity<PruefplanVorlage>().HasData(WorkspaceSeedData.PruefplanVorlagen);
        b.Entity<PruefplanVorlageItem>().HasData(WorkspaceSeedData.PruefplanVorlageItems);

    }
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        ApplyWorkspaceTimestamps();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        ApplyWorkspaceTimestamps();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void ApplyWorkspaceTimestamps()
    {
        var utcNow = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Deleted)
            {
                continue;
            }

            if (entry.Entity is Probe probe)
            {
                if (entry.State == EntityState.Added)
                {
                    probe.CreatedAtUtc ??= utcNow;
                    probe.UpdatedAtUtc ??= utcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    probe.UpdatedAtUtc = utcNow;
                }
            }
        }
    }
}









