# BLE Laborsoftware (Pilot)

- Ziel: Desktop-App (.NET 8, MAUI, Windows-first) für ETL aus Excel, Regeln, PDF-Reporting, Rollen/Identity.
- Struktur: siehe `ble/` – Startpunkt `BLE.sln`.

Schnellstart (Windows):
- `dotnet --list-sdks` (8.0.x)
- `dotnet workload install maui`
- `dotnet workload install maui-windows`
- `./scripts/build.ps1`
- `./scripts/run-windows.ps1`

Konfiguration:
- `config/etl/*.json` (Mappings, Units)
- `config/rules/rules.workflows.json` (RulesEngine)

Login:
- Benutzer: `admin`
- Passwort: `admin123!`

Import/Export:
- Dashboard → Excel-Pfad einfügen → Import
- Probencode eingeben → PDF Export (Dokumente-Ordner)

