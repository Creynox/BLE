using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BLE.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStsTest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    IsAdmin = table.Column<bool>(type: "INTEGER", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    SecurityStamp = table.Column<string>(type: "TEXT", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EtlFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Filename = table.Column<string>(type: "TEXT", nullable: false),
                    Source = table.Column<string>(type: "TEXT", nullable: false),
                    FileHash = table.Column<string>(type: "TEXT", nullable: false),
                    ImportedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    Message = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EtlFiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EtlLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    EtlFileId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SheetName = table.Column<string>(type: "TEXT", nullable: false),
                    RowIndex = table.Column<int>(type: "INTEGER", nullable: true),
                    ColumnName = table.Column<string>(type: "TEXT", nullable: true),
                    Severity = table.Column<string>(type: "TEXT", nullable: false),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    Message = table.Column<string>(type: "TEXT", nullable: false),
                    PayloadJson = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EtlLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Formeln",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PruefverfahrenId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Ausdruck = table.Column<string>(type: "TEXT", nullable: false),
                    EinheitErgebnis = table.Column<string>(type: "TEXT", nullable: true),
                    ValidierungSql = table.Column<string>(type: "TEXT", nullable: true),
                    GueltigAb = table.Column<DateTime>(type: "TEXT", nullable: false),
                    GueltigBis = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Formeln", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Grenzwerte",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PruefverfahrenId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Merkmal = table.Column<string>(type: "TEXT", nullable: false),
                    MinWert = table.Column<decimal>(type: "TEXT", nullable: true),
                    MaxWert = table.Column<decimal>(type: "TEXT", nullable: true),
                    Einheit = table.Column<string>(type: "TEXT", nullable: true),
                    Bedingung = table.Column<string>(type: "TEXT", nullable: true),
                    GueltigAb = table.Column<DateTime>(type: "TEXT", nullable: false),
                    GueltigBis = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Scope = table.Column<string>(type: "TEXT", nullable: false, defaultValue: "global"),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grenzwerte", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Kornfraktionen",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProbeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PruefverfahrenId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FraktionIndex = table.Column<int>(type: "INTEGER", nullable: false),
                    KorngroesseMinMm = table.Column<decimal>(type: "TEXT", nullable: true),
                    KorngroesseMaxMm = table.Column<decimal>(type: "TEXT", nullable: true),
                    MasseG = table.Column<decimal>(type: "TEXT", nullable: true),
                    AnteilPercent = table.Column<decimal>(type: "TEXT", nullable: true),
                    DurchgangPercent = table.Column<decimal>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kornfraktionen", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Kunden",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Adresse = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kunden", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Messungen",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProbeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PruefverfahrenId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Merkmal = table.Column<string>(type: "TEXT", nullable: false),
                    IstWert = table.Column<decimal>(type: "TEXT", nullable: true),
                    Einheit = table.Column<string>(type: "TEXT", nullable: true),
                    Messunsicherheit = table.Column<decimal>(type: "TEXT", nullable: true),
                    Messzeitpunkt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Pruefer = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messungen", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Normwerke",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Bezeichnung = table.Column<string>(type: "TEXT", nullable: false),
                    Version = table.Column<string>(type: "TEXT", nullable: false),
                    GueltigAb = table.Column<DateTime>(type: "TEXT", nullable: false),
                    GueltigBis = table.Column<DateTime>(type: "TEXT", nullable: true),
                    QuelleUrl = table.Column<string>(type: "TEXT", nullable: true),
                    Bemerkung = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Normwerke", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Proben",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProjektId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Probencode = table.Column<string>(type: "TEXT", nullable: false),
                    Materialtyp = table.Column<string>(type: "TEXT", nullable: false, defaultValue: "asphalt"),
                    Entnahmedatum = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Temperatur = table.Column<decimal>(type: "TEXT", nullable: true),
                    Feuchte = table.Column<decimal>(type: "TEXT", nullable: true),
                    Werk = table.Column<string>(type: "TEXT", nullable: true),
                    Produkt = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proben", x => x.Id);
                    table.CheckConstraint("ck_probe_materialtyp", "materialtyp IN ('asphalt','beton')");
                });

            migrationBuilder.CreateTable(
                name: "Projekte",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    KundeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Ort = table.Column<string>(type: "TEXT", nullable: true),
                    Startdatum = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Enddatum = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projekte", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pruefverfahren",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    NormwerkId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    Titel = table.Column<string>(type: "TEXT", nullable: false),
                    Beschreibung = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pruefverfahren", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StsTests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Probencode = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Entnahmedatum = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Werk = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    Produkt = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    Materialtyp = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    GesamtEinwaage = table.Column<decimal>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StsTests", x => x.Id);
                    table.CheckConstraint("ck_ststest_materialtyp", "materialtyp IN ('Asphalt','Beton')");
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderKey = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RoleId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StsErgebnisse",
                columns: table => new
                {
                    StsTestId = table.Column<Guid>(type: "TEXT", nullable: false),
                    S1 = table.Column<decimal>(type: "TEXT", nullable: false),
                    S2 = table.Column<decimal>(type: "TEXT", nullable: false),
                    KornformIndex = table.Column<decimal>(type: "TEXT", nullable: false),
                    GrenzwerteOk = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StsErgebnisse", x => x.StsTestId);
                    table.ForeignKey(
                        name: "FK_StsErgebnisse_StsTests_StsTestId",
                        column: x => x.StsTestId,
                        principalTable: "StsTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StsKochversuche",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StsTestId = table.Column<Guid>(type: "TEXT", nullable: false),
                    EinwaageVorKochen = table.Column<decimal>(type: "TEXT", nullable: false),
                    RueckwaageNachKochen = table.Column<decimal>(type: "TEXT", nullable: false),
                    Kochzeit = table.Column<TimeSpan>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StsKochversuche", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StsKochversuche_StsTests_StsTestId",
                        column: x => x.StsTestId,
                        principalTable: "StsTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StsKornformen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StsTestId = table.Column<Guid>(type: "TEXT", nullable: false),
                    EinwaageGesamt = table.Column<decimal>(type: "TEXT", nullable: false),
                    EinwaageSchlechtGeformt = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StsKornformen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StsKornformen_StsTests_StsTestId",
                        column: x => x.StsTestId,
                        principalTable: "StsTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StsSiebanalysen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StsTestId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SiebBezeichnung = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Einwaage = table.Column<decimal>(type: "TEXT", nullable: false),
                    Rueckwaage = table.Column<decimal>(type: "TEXT", nullable: false),
                    DurchgangProzent = table.Column<decimal>(type: "TEXT", nullable: false),
                    RueckgangProzent = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StsSiebanalysen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StsSiebanalysen_StsTests_StsTestId",
                        column: x => x.StsTestId,
                        principalTable: "StsTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_messung_probe",
                table: "Messungen",
                column: "ProbeId");

            migrationBuilder.CreateIndex(
                name: "ix_messung_pruef",
                table: "Messungen",
                column: "PruefverfahrenId");

            migrationBuilder.CreateIndex(
                name: "IX_Proben_Probencode",
                table: "Proben",
                column: "Probencode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StsKochversuche_StsTestId",
                table: "StsKochversuche",
                column: "StsTestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StsKornformen_StsTestId",
                table: "StsKornformen",
                column: "StsTestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StsSiebanalysen_StsTestId_SiebBezeichnung",
                table: "StsSiebanalysen",
                columns: new[] { "StsTestId", "SiebBezeichnung" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "EtlFiles");

            migrationBuilder.DropTable(
                name: "EtlLogs");

            migrationBuilder.DropTable(
                name: "Formeln");

            migrationBuilder.DropTable(
                name: "Grenzwerte");

            migrationBuilder.DropTable(
                name: "Kornfraktionen");

            migrationBuilder.DropTable(
                name: "Kunden");

            migrationBuilder.DropTable(
                name: "Messungen");

            migrationBuilder.DropTable(
                name: "Normwerke");

            migrationBuilder.DropTable(
                name: "Proben");

            migrationBuilder.DropTable(
                name: "Projekte");

            migrationBuilder.DropTable(
                name: "Pruefverfahren");

            migrationBuilder.DropTable(
                name: "StsErgebnisse");

            migrationBuilder.DropTable(
                name: "StsKochversuche");

            migrationBuilder.DropTable(
                name: "StsKornformen");

            migrationBuilder.DropTable(
                name: "StsSiebanalysen");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "StsTests");
        }
    }
}
