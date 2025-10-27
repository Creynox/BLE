# Architekturüberblick

- Plattform: .NET 8 LTS, MAUI (Windows-first), EF Core 8, RulesEngine 5.0.0, QuestPDF 2024.3.0.
- Projekte: Domain (Entitäten), Data (EF Core + Identity), Services (ETL/Rules/PDF), UI (MAUI), Tools.Seed.
- Konfiguration: JSON unter `config/` (ETL-Mappings, Units, Rules) — Konfig-first.
- Datenbank: SQLite lokal (Entwickler), optional PostgreSQL via ConnectionString.
- ETL: ExcelDataReader, robust gegen .xls/.xlsx, Idempotenz via SHA-256, Logging in `etl_log`.
- Regeln: RulesEngine Workflows aus JSON, Auswertung zur Laufzeit.
- Reporting: QuestPDF, modulare Abschnitte, Siebkurven-Tabellen; Diagramme später.
- Security: ASP.NET Identity Tabellen (lokal), Login in MAUI, Rollenvorbereitung.

Startpfad: MAUI UI mit Login → Dashboard (Import, PDF-Export, Verlauf).

