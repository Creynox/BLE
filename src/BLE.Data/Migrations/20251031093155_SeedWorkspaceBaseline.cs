using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BLE.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedWorkspaceBaseline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Pruefverfahren",
                columns: new[] { "Id", "Aktiv", "Beschreibung", "Code", "CreatedAt", "Name", "NormwerkId", "Titel", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("32ad7066-76a4-4b53-8387-49753d1953a8"), true, null, "FROST", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Frost/Tausalz", new Guid("00000000-0000-0000-0000-000000000000"), "Frost/Tausalz", null },
                    { new Guid("36a8607f-92d3-4d9c-b6d5-2db8a6355409"), true, null, "SIEB", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Siebanalyse", new Guid("00000000-0000-0000-0000-000000000000"), "Siebanalyse", null },
                    { new Guid("8b66f6d4-3d6f-478a-bf42-4cf2f5a9a0e2"), true, null, "BIND", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Bindemittelgehalt", new Guid("00000000-0000-0000-0000-000000000000"), "Bindemittelgehalt", null },
                    { new Guid("9adf7c86-409b-46f0-957b-0d9c4b7b64f0"), true, null, "SAE", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Siebäquivalent", new Guid("00000000-0000-0000-0000-000000000000"), "Siebäquivalent", null },
                    { new Guid("a6c0c49c-3f3c-4f66-a4b6-9efc0266f9b9"), true, null, "KOCH", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Kochversuch", new Guid("00000000-0000-0000-0000-000000000000"), "Kochversuch", null },
                    { new Guid("d4f5389d-2195-4e1b-9568-7f7d4f7fdedd"), true, null, "KORN", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Kornform", new Guid("00000000-0000-0000-0000-000000000000"), "Kornform", null }
                });

            migrationBuilder.InsertData(
                table: "Workspace_PruefplanVorlage",
                columns: new[] { "Id", "CreatedAt", "GueltigAbUtc", "GueltigBisUtc", "MaterialtypId", "Name", "ProduktId", "UpdatedAt", "Version", "WerkId" },
                values: new object[,]
                {
                    { new Guid("0b6a1e44-7009-4a63-bf4d-5b1b6c2c81c0"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "STS 0-45 – Standard", null, null, "v2025-10-A", null },
                    { new Guid("0d54c3d1-7f28-4e3e-a8be-02f6e9e37f2a"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "BS 0-2 abgemagert (Enzberg)", null, null, "v2025-10-D", null },
                    { new Guid("3e6a0732-91e5-4703-a8e2-2452b8b9e046"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "2-8 Beton – Standard", null, null, "v2025-10-B", null },
                    { new Guid("48f1b1c5-8c4d-49f7-8e21-f712b857d0e2"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "KSM-MB – Füller", null, null, "v2025-10-C", null }
                });

            migrationBuilder.InsertData(
                table: "Workspace_PruefplanVorlageItem",
                columns: new[] { "Id", "CreatedAt", "PruefverfahrenId", "Reihenfolge", "UpdatedAt", "VorlageId" },
                values: new object[,]
                {
                    { new Guid("0bd473c1-3933-4b97-9e26-9f4b0b1747ab"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("9adf7c86-409b-46f0-957b-0d9c4b7b64f0"), 3, null, new Guid("0b6a1e44-7009-4a63-bf4d-5b1b6c2c81c0") },
                    { new Guid("4e661d63-7060-4709-ad20-c8bcbf9e247d"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("d4f5389d-2195-4e1b-9568-7f7d4f7fdedd"), 2, null, new Guid("0d54c3d1-7f28-4e3e-a8be-02f6e9e37f2a") }
                });

            migrationBuilder.InsertData(
                table: "Workspace_PruefplanVorlageItem",
                columns: new[] { "Id", "CreatedAt", "Pflicht", "PruefverfahrenId", "Reihenfolge", "UpdatedAt", "VorlageId" },
                values: new object[,]
                {
                    { new Guid("69f3bb40-0c6a-4f10-815b-8ac749d5b1e6"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("d4f5389d-2195-4e1b-9568-7f7d4f7fdedd"), 2, null, new Guid("3e6a0732-91e5-4703-a8e2-2452b8b9e046") },
                    { new Guid("7e128e41-9eb4-461f-be99-9c0ae3d02961"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("9adf7c86-409b-46f0-957b-0d9c4b7b64f0"), 1, null, new Guid("48f1b1c5-8c4d-49f7-8e21-f712b857d0e2") }
                });

            migrationBuilder.InsertData(
                table: "Workspace_PruefplanVorlageItem",
                columns: new[] { "Id", "CreatedAt", "PruefverfahrenId", "Reihenfolge", "UpdatedAt", "VorlageId" },
                values: new object[] { new Guid("8e3bf7cc-d79f-483b-b2f4-3f1ff19d4f50"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("8b66f6d4-3d6f-478a-bf42-4cf2f5a9a0e2"), 2, null, new Guid("48f1b1c5-8c4d-49f7-8e21-f712b857d0e2") });

            migrationBuilder.InsertData(
                table: "Workspace_PruefplanVorlageItem",
                columns: new[] { "Id", "CreatedAt", "Pflicht", "PruefverfahrenId", "Reihenfolge", "UpdatedAt", "VorlageId" },
                values: new object[,]
                {
                    { new Guid("959c37b2-1b41-469f-ba4a-6c4505e6da93"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("d4f5389d-2195-4e1b-9568-7f7d4f7fdedd"), 2, null, new Guid("0b6a1e44-7009-4a63-bf4d-5b1b6c2c81c0") },
                    { new Guid("af9f0245-417e-43c4-83ae-567f62a8ede1"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("9adf7c86-409b-46f0-957b-0d9c4b7b64f0"), 3, null, new Guid("3e6a0732-91e5-4703-a8e2-2452b8b9e046") },
                    { new Guid("bb3d0294-3103-4948-9f35-2e0f5b85f14c"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("a6c0c49c-3f3c-4f66-a4b6-9efc0266f9b9"), 4, null, new Guid("0b6a1e44-7009-4a63-bf4d-5b1b6c2c81c0") },
                    { new Guid("c0e98539-9bb2-48cf-9c0d-5d39b4fc4008"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("36a8607f-92d3-4d9c-b6d5-2db8a6355409"), 1, null, new Guid("3e6a0732-91e5-4703-a8e2-2452b8b9e046") },
                    { new Guid("d36f6f73-da64-428b-8d3e-4be4e2affbd6"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("36a8607f-92d3-4d9c-b6d5-2db8a6355409"), 1, null, new Guid("0d54c3d1-7f28-4e3e-a8be-02f6e9e37f2a") },
                    { new Guid("f8fce8cb-ee7e-4c3b-9a1e-1607d4f3f258"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("36a8607f-92d3-4d9c-b6d5-2db8a6355409"), 1, null, new Guid("0b6a1e44-7009-4a63-bf4d-5b1b6c2c81c0") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Pruefverfahren",
                keyColumn: "Id",
                keyValue: new Guid("32ad7066-76a4-4b53-8387-49753d1953a8"));

            migrationBuilder.DeleteData(
                table: "Workspace_PruefplanVorlageItem",
                keyColumn: "Id",
                keyValue: new Guid("0bd473c1-3933-4b97-9e26-9f4b0b1747ab"));

            migrationBuilder.DeleteData(
                table: "Workspace_PruefplanVorlageItem",
                keyColumn: "Id",
                keyValue: new Guid("4e661d63-7060-4709-ad20-c8bcbf9e247d"));

            migrationBuilder.DeleteData(
                table: "Workspace_PruefplanVorlageItem",
                keyColumn: "Id",
                keyValue: new Guid("69f3bb40-0c6a-4f10-815b-8ac749d5b1e6"));

            migrationBuilder.DeleteData(
                table: "Workspace_PruefplanVorlageItem",
                keyColumn: "Id",
                keyValue: new Guid("7e128e41-9eb4-461f-be99-9c0ae3d02961"));

            migrationBuilder.DeleteData(
                table: "Workspace_PruefplanVorlageItem",
                keyColumn: "Id",
                keyValue: new Guid("8e3bf7cc-d79f-483b-b2f4-3f1ff19d4f50"));

            migrationBuilder.DeleteData(
                table: "Workspace_PruefplanVorlageItem",
                keyColumn: "Id",
                keyValue: new Guid("959c37b2-1b41-469f-ba4a-6c4505e6da93"));

            migrationBuilder.DeleteData(
                table: "Workspace_PruefplanVorlageItem",
                keyColumn: "Id",
                keyValue: new Guid("af9f0245-417e-43c4-83ae-567f62a8ede1"));

            migrationBuilder.DeleteData(
                table: "Workspace_PruefplanVorlageItem",
                keyColumn: "Id",
                keyValue: new Guid("bb3d0294-3103-4948-9f35-2e0f5b85f14c"));

            migrationBuilder.DeleteData(
                table: "Workspace_PruefplanVorlageItem",
                keyColumn: "Id",
                keyValue: new Guid("c0e98539-9bb2-48cf-9c0d-5d39b4fc4008"));

            migrationBuilder.DeleteData(
                table: "Workspace_PruefplanVorlageItem",
                keyColumn: "Id",
                keyValue: new Guid("d36f6f73-da64-428b-8d3e-4be4e2affbd6"));

            migrationBuilder.DeleteData(
                table: "Workspace_PruefplanVorlageItem",
                keyColumn: "Id",
                keyValue: new Guid("f8fce8cb-ee7e-4c3b-9a1e-1607d4f3f258"));

            migrationBuilder.DeleteData(
                table: "Pruefverfahren",
                keyColumn: "Id",
                keyValue: new Guid("36a8607f-92d3-4d9c-b6d5-2db8a6355409"));

            migrationBuilder.DeleteData(
                table: "Pruefverfahren",
                keyColumn: "Id",
                keyValue: new Guid("8b66f6d4-3d6f-478a-bf42-4cf2f5a9a0e2"));

            migrationBuilder.DeleteData(
                table: "Pruefverfahren",
                keyColumn: "Id",
                keyValue: new Guid("9adf7c86-409b-46f0-957b-0d9c4b7b64f0"));

            migrationBuilder.DeleteData(
                table: "Pruefverfahren",
                keyColumn: "Id",
                keyValue: new Guid("a6c0c49c-3f3c-4f66-a4b6-9efc0266f9b9"));

            migrationBuilder.DeleteData(
                table: "Pruefverfahren",
                keyColumn: "Id",
                keyValue: new Guid("d4f5389d-2195-4e1b-9568-7f7d4f7fdedd"));

            migrationBuilder.DeleteData(
                table: "Workspace_PruefplanVorlage",
                keyColumn: "Id",
                keyValue: new Guid("0b6a1e44-7009-4a63-bf4d-5b1b6c2c81c0"));

            migrationBuilder.DeleteData(
                table: "Workspace_PruefplanVorlage",
                keyColumn: "Id",
                keyValue: new Guid("0d54c3d1-7f28-4e3e-a8be-02f6e9e37f2a"));

            migrationBuilder.DeleteData(
                table: "Workspace_PruefplanVorlage",
                keyColumn: "Id",
                keyValue: new Guid("3e6a0732-91e5-4703-a8e2-2452b8b9e046"));

            migrationBuilder.DeleteData(
                table: "Workspace_PruefplanVorlage",
                keyColumn: "Id",
                keyValue: new Guid("48f1b1c5-8c4d-49f7-8e21-f712b857d0e2"));
        }
    }
}
