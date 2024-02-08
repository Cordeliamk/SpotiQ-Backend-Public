using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace spotiq_backend.Migrations
{
    /// <inheritdoc />
    public partial class Firste2004 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SpotifyHost",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeviceId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ClientId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ClientSecret = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AccessToken = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    RefreshToken = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Url = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpotifyHost", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Poll",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ArchivedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WinnerId = table.Column<int>(type: "int", nullable: false),
                    TrackSpotifyId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpotifyHostId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Poll", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Poll_SpotifyHost_SpotifyHostId",
                        column: x => x.SpotifyHostId,
                        principalTable: "SpotifyHost",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Songwish",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SpotifyId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ArtistName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UserSession = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EnteredTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    QueuedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SelectedForVoteTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ArchivedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SpotifyHostId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Songwish", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Songwish_SpotifyHost_SpotifyHostId",
                        column: x => x.SpotifyHostId,
                        principalTable: "SpotifyHost",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PollSong",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VotesCount = table.Column<int>(type: "int", nullable: false),
                    PollId = table.Column<int>(type: "int", nullable: false),
                    SongwishId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PollSong", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PollSong_Poll_PollId",
                        column: x => x.PollId,
                        principalTable: "Poll",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PollSong_Songwish_SongwishId",
                        column: x => x.SongwishId,
                        principalTable: "Songwish",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Poll_SpotifyHostId",
                table: "Poll",
                column: "SpotifyHostId");

            migrationBuilder.CreateIndex(
                name: "IX_PollSong_PollId",
                table: "PollSong",
                column: "PollId");

            migrationBuilder.CreateIndex(
                name: "IX_PollSong_SongwishId",
                table: "PollSong",
                column: "SongwishId");

            migrationBuilder.CreateIndex(
                name: "IX_Songwish_SpotifyHostId",
                table: "Songwish",
                column: "SpotifyHostId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PollSong");

            migrationBuilder.DropTable(
                name: "Poll");

            migrationBuilder.DropTable(
                name: "Songwish");

            migrationBuilder.DropTable(
                name: "SpotifyHost");
        }
    }
}
