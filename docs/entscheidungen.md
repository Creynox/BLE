# Entscheidungsprotokoll

- Keine Rückfragen: Annahmen getroffen und hier dokumentiert.
- Identity: Nutzung der EF-Tabellen lokal, Login in MAUI ohne ASP.NET Host. Rollen-UI wird später ergänzt, Tabellen vorhanden.
- ETL: Start mit Sieb-Mapping. ExcelDataReader gewählt (unterstützt .xls und .xlsx). Dezimaltrennzeichen über Mapping steuerbar ("," bevorzugt für DE).
- Konfiguration: Alle Mappings/Units/Rules in `config/` abgelegt, zur Laufzeit geladen. Keine Hardcodierung von Normen/Grenzwerten.
- Idempotenz: SHA-256 pro Datei; Re-Import verhindert (Duplikatschutz) über `etl_file.file_hash`.
- Transaktionen: Import läuft in DB-Transaktion; bei Fehlern Rollback und Protokollierung in `etl_log`.
- PDF: Minimaler Report mit Tabellen; Diagramme folgen (Siebkurven zeichnen).
- DB: Default SQLite im Benutzerprofil (`LocalApplicationData/ble.db`). PostgreSQL via ConnectionString möglich (künftiger Umschalter per appsettings).
- UI: Windows-first, Touch-geeignete einfache Controls. Login + Dashboard initial.
- Tests: Platzhalter-Projekte angelegt, in diesem Inkrement einfache Smoke-Tests vorgesehen.
- Workloads: Keine PackageReference auf MAUI-Controls; Build über MAUI-Workload.

*** Ende ***
