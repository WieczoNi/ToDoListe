using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToDoListe2.Migrations
{
    /// <inheritdoc />
    public partial class CreateDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SavedDB",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MyTask = table.Column<string>(type: "TEXT", nullable: false),
                    Erledigt = table.Column<bool>(type: "INTEGER", nullable: false),
                    TabIndex = table.Column<int>(type: "INTEGER", nullable: false),
                    DueDateString = table.Column<string>(type: "TEXT", nullable: false),
                    DueDate = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedDB", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SavedDB");
        }
    }
}
