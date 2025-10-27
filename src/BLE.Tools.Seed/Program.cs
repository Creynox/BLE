using System;
using System.IO;
using BLE.Data;
using BLE.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ble.db");
var options = new DbContextOptionsBuilder<BLEDbContext>().UseSqlite($"Data Source={dbPath}").Options;
using var db = new BLEDbContext(options);
db.Database.Migrate();

if (!db.Users.Any())
{
    var admin = new ApplicationUser { Id = Guid.NewGuid(), UserName = "admin", NormalizedUserName = "ADMIN", DisplayName = "Administrator" };
    var hasher = new PasswordHasher<ApplicationUser>();
    admin.PasswordHash = hasher.HashPassword(admin, "admin123!");
    db.Users.Add(admin);
}

if (!db.Normwerke.Any())
{
    var nw = new Normwerk { Bezeichnung = "BLE-Grundnorm", Version = "2025", GueltigAb = DateTime.UtcNow.Date };
    db.Normwerke.Add(nw);
}

if (!db.Projekte.Any())
{
    var kunde = new Kunde { Name = "Demo GmbH" };
    db.Kunden.Add(kunde);
    var proj = new Projekt { KundeId = kunde.Id, Name = "Demo-Projekt", Ort = "Berlin" };
    db.Projekte.Add(proj);
}

await db.SaveChangesAsync();

Console.WriteLine("Seed abgeschlossen.");
