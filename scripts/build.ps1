param(
  [switch]$Publish
)

Write-Host "Restoring workloads..."
dotnet workload restore

Write-Host "Restoring solution..."
dotnet restore .\BLE.sln

Write-Host "Building solution (Windows target)..."
dotnet build .\BLE.sln -f net8.0-windows10.0.19041.0

if ($Publish) {
  Write-Host "Publishing MAUI app..."
  dotnet publish .\src\BLE.UI\BLE.UI.csproj -c Release -f net8.0-windows10.0.19041.0 -o .\publish
}
