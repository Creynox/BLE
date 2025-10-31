using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BLE.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkspaceNonBreaking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Aktiv",
                table: "Pruefverfahren",
                type: "INTEGER",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Pruefverfahren",
                type: "TEXT",
                maxLength: 120,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "BearbeiterId",
                table: "Proben",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "Proben",
                type: "datetime2",
                nullable: true,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "EingangAmUtc",
                table: "Proben",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "KundeId",
                table: "Proben",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MaterialtypId",
                table: "Proben",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProduktId",
                table: "Proben",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Proben",
                type: "TEXT",
                maxLength: 32,
                nullable: false,
                defaultValue: "Neu");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "Proben",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "WerkId",
                table: "Proben",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Workspace_AuditLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Entity = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    EntityId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Action = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    AtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PayloadJson = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workspace_AuditLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Workspace_Materialtyp",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workspace_Materialtyp", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Workspace_Pruefauftrag",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProbeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PruefverfahrenId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false, defaultValue: "Offen"),
                    BearbeiterId = table.Column<Guid>(type: "TEXT", nullable: true),
                    StartedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workspace_Pruefauftrag", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Workspace_Pruefauftrag_Proben_ProbeId",
                        column: x => x.ProbeId,
                        principalTable: "Proben",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Workspace_Pruefauftrag_Pruefverfahren_PruefverfahrenId",
                        column: x => x.PruefverfahrenId,
                        principalTable: "Pruefverfahren",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Workspace_Werk",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workspace_Werk", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Workspace_Produkt",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    WerkId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Kategorie = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    SiebbereichText = table.Column<string>(type: "TEXT", maxLength: 60, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workspace_Produkt", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Workspace_Produkt_Workspace_Werk_WerkId",
                        column: x => x.WerkId,
                        principalTable: "Workspace_Werk",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Workspace_PruefplanVorlage",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 140, nullable: false),
                    WerkId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ProduktId = table.Column<Guid>(type: "TEXT", nullable: true),
                    MaterialtypId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Version = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    GueltigAbUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GueltigBisUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workspace_PruefplanVorlage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Workspace_PruefplanVorlage_Workspace_Materialtyp_MaterialtypId",
                        column: x => x.MaterialtypId,
                        principalTable: "Workspace_Materialtyp",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Workspace_PruefplanVorlage_Workspace_Produkt_ProduktId",
                        column: x => x.ProduktId,
                        principalTable: "Workspace_Produkt",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Workspace_PruefplanVorlage_Workspace_Werk_WerkId",
                        column: x => x.WerkId,
                        principalTable: "Workspace_Werk",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Workspace_PruefplanVorlageItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    VorlageId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PruefverfahrenId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Pflicht = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    Reihenfolge = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workspace_PruefplanVorlageItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Workspace_PruefplanVorlageItem_Pruefverfahren_PruefverfahrenId",
                        column: x => x.PruefverfahrenId,
                        principalTable: "Pruefverfahren",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Workspace_PruefplanVorlageItem_Workspace_PruefplanVorlage_VorlageId",
                        column: x => x.VorlageId,
                        principalTable: "Workspace_PruefplanVorlage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pruefverfahren_Code",
                table: "Pruefverfahren",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Proben_MaterialtypId",
                table: "Proben",
                column: "MaterialtypId");

            migrationBuilder.CreateIndex(
                name: "IX_Proben_ProduktId",
                table: "Proben",
                column: "ProduktId");

            migrationBuilder.CreateIndex(
                name: "ix_workspace_probe_scope",
                table: "Proben",
                columns: new[] { "WerkId", "ProduktId", "MaterialtypId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Workspace_Produkt_WerkId_Name",
                table: "Workspace_Produkt",
                columns: new[] { "WerkId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Workspace_Pruefauftrag_ProbeId_PruefverfahrenId",
                table: "Workspace_Pruefauftrag",
                columns: new[] { "ProbeId", "PruefverfahrenId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Workspace_Pruefauftrag_PruefverfahrenId",
                table: "Workspace_Pruefauftrag",
                column: "PruefverfahrenId");

            migrationBuilder.CreateIndex(
                name: "IX_Workspace_PruefplanVorlage_MaterialtypId",
                table: "Workspace_PruefplanVorlage",
                column: "MaterialtypId");

            migrationBuilder.CreateIndex(
                name: "IX_Workspace_PruefplanVorlage_ProduktId",
                table: "Workspace_PruefplanVorlage",
                column: "ProduktId");

            migrationBuilder.Sql("CREATE UNIQUE INDEX ux_workspace_vorlage_scope ON Workspace_PruefplanVorlage(COALESCE(WerkId, '00000000-0000-0000-0000-000000000000'), COALESCE(ProduktId, '00000000-0000-0000-0000-000000000000'), COALESCE(MaterialtypId, '00000000-0000-0000-0000-000000000000'), Version);");

            migrationBuilder.CreateIndex(
                name: "IX_Workspace_PruefplanVorlageItem_PruefverfahrenId",
                table: "Workspace_PruefplanVorlageItem",
                column: "PruefverfahrenId");

            migrationBuilder.CreateIndex(
                name: "IX_Workspace_PruefplanVorlageItem_VorlageId_PruefverfahrenId",
                table: "Workspace_PruefplanVorlageItem",
                columns: new[] { "VorlageId", "PruefverfahrenId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Workspace_Werk_Name",
                table: "Workspace_Werk",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Proben_Workspace_Materialtyp_MaterialtypId",
                table: "Proben",
                column: "MaterialtypId",
                principalTable: "Workspace_Materialtyp",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Proben_Workspace_Produkt_ProduktId",
                table: "Proben",
                column: "ProduktId",
                principalTable: "Workspace_Produkt",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Proben_Workspace_Werk_WerkId",
                table: "Proben",
                column: "WerkId",
                principalTable: "Workspace_Werk",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Proben_Workspace_Materialtyp_MaterialtypId",
                table: "Proben");

            migrationBuilder.DropForeignKey(
                name: "FK_Proben_Workspace_Produkt_ProduktId",
                table: "Proben");

            migrationBuilder.DropForeignKey(
                name: "FK_Proben_Workspace_Werk_WerkId",
                table: "Proben");

            migrationBuilder.DropTable(
                name: "Workspace_AuditLog");

            migrationBuilder.DropTable(
                name: "Workspace_Pruefauftrag");

            migrationBuilder.DropTable(
                name: "Workspace_PruefplanVorlageItem");

            migrationBuilder.Sql("DROP INDEX IF EXISTS ux_workspace_vorlage_scope;");

            migrationBuilder.DropTable(
                name: "Workspace_PruefplanVorlage");

            migrationBuilder.DropTable(
                name: "Workspace_Materialtyp");

            migrationBuilder.DropTable(
                name: "Workspace_Produkt");

            migrationBuilder.DropTable(
                name: "Workspace_Werk");

            migrationBuilder.DropIndex(
                name: "IX_Pruefverfahren_Code",
                table: "Pruefverfahren");

            migrationBuilder.DropIndex(
                name: "IX_Proben_MaterialtypId",
                table: "Proben");

            migrationBuilder.DropIndex(
                name: "IX_Proben_ProduktId",
                table: "Proben");

            migrationBuilder.DropIndex(
                name: "ix_workspace_probe_scope",
                table: "Proben");

            migrationBuilder.DropColumn(
                name: "Aktiv",
                table: "Pruefverfahren");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Pruefverfahren");

            migrationBuilder.DropColumn(
                name: "BearbeiterId",
                table: "Proben");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "Proben");

            migrationBuilder.DropColumn(
                name: "EingangAmUtc",
                table: "Proben");

            migrationBuilder.DropColumn(
                name: "KundeId",
                table: "Proben");

            migrationBuilder.DropColumn(
                name: "MaterialtypId",
                table: "Proben");

            migrationBuilder.DropColumn(
                name: "ProduktId",
                table: "Proben");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Proben");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                table: "Proben");

            migrationBuilder.DropColumn(
                name: "WerkId",
                table: "Proben");
        }
    }
}







