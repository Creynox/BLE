# Installation (Windows, .NET 8, MAUI)

- Voraussetzungen:
  - .NET SDK 8.0.x (empfohlen 8.0.415)
  - Windows 10 SDK 19041+
  - MAUI Workloads: `dotnet workload install maui` und `dotnet workload install maui-windows`
  - Optional: WebView2 Runtime

- Build & Run:
  - `./scripts/build.ps1`
  - `./scripts/run-windows.ps1`

- Publish (Folder/MSIX vorbereitet als Folder-Publish):
  - `./scripts/build.ps1 -Publish`
  - Artefakte unter `./publish/`

- Datenbank:
  - SQLite-Datei unter `%LOCALAPPDATA%/ble.db`
  - Erster Start legt Schema an und seeding Admin-User (admin / admin123!)

