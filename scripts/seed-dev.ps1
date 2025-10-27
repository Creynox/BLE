Write-Host "Applying EF Core migrations..."
dotnet ef database update --project .\src\BLE.Data\BLE.Data.csproj

Write-Host "Seeding via tool..."
dotnet run --project .\src\BLE.Tools.Seed\BLE.Tools.Seed.csproj
