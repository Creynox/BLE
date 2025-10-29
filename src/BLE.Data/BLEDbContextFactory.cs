using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BLE.Data;

public class BLEDbContextFactory : IDesignTimeDbContextFactory<BLEDbContext>
{
    public BLEDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BLEDbContext>();

        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var dataDirectory = Path.Combine(localAppData, "BLE");
        Directory.CreateDirectory(dataDirectory);
        var dbPath = Path.Combine(dataDirectory, "ble.db");

        optionsBuilder.UseSqlite($"Data Source={dbPath}");

        return new BLEDbContext(optionsBuilder.Options);
    }
}
