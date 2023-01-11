using Microsoft.EntityFrameworkCore.Migrations;

namespace NetProc.Data.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Audits",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    Info = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Audits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Coils",
                columns: table => new
                {
                    Number = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    PulseTime = table.Column<int>(nullable: false),
                    Bus = table.Column<string>(nullable: true),
                    Polarity = table.Column<bool>(nullable: false),
                    Tags = table.Column<string>(nullable: true),
                    Search = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coils", x => x.Number);
                });

            migrationBuilder.CreateTable(
                name: "GameAudit",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameAudit", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Leds",
                columns: table => new
                {
                    Number = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Bus = table.Column<string>(nullable: true),
                    Polarity = table.Column<bool>(nullable: false),
                    Tags = table.Column<string>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leds", x => x.Number);
                });

            migrationBuilder.CreateTable(
                name: "Machine",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MachineType = table.Column<int>(nullable: false),
                    NumBalls = table.Column<int>(nullable: false),
                    DisplayMonitor = table.Column<bool>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    Version = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Machine", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Initials = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Default = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    Info = table.Column<string>(nullable: true),
                    Parent = table.Column<string>(nullable: true),
                    Options = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Switches",
                columns: table => new
                {
                    Number = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Tags = table.Column<string>(nullable: true),
                    SearchReset = table.Column<string>(nullable: true),
                    SearchStop = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Switches", x => x.Number);
                });

            migrationBuilder.CreateTable(
                name: "GameLog",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PlayerId = table.Column<int>(nullable: false),
                    BallsPlayed = table.Column<int>(nullable: false),
                    Score = table.Column<long>(nullable: false),
                    GameAuditId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameLog_GameAudit_GameAuditId",
                        column: x => x.GameAuditId,
                        principalTable: "GameAudit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameLog_GameAuditId",
                table: "GameLog",
                column: "GameAuditId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Audits");

            migrationBuilder.DropTable(
                name: "Coils");

            migrationBuilder.DropTable(
                name: "GameLog");

            migrationBuilder.DropTable(
                name: "Leds");

            migrationBuilder.DropTable(
                name: "Machine");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "Switches");

            migrationBuilder.DropTable(
                name: "GameAudit");
        }
    }
}
