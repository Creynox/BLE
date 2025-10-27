using BLE.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BLE.Tests.Integration;

public class DbContextSmokeTests
{
    [Fact]
    public void Can_Create_InMemory_DB()
    {
        var options = new DbContextOptionsBuilder<BLEDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;
        using var db = new BLEDbContext(options);
        db.Database.OpenConnection();
        db.Database.EnsureCreated();
        Assert.True(db.Database.CanConnect());
    }
}

