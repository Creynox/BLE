using System;
using BLE.Data;
using BLE.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

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
    }
}

