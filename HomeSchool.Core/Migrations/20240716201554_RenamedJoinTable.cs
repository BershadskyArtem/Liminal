using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeSchool.Core.Migrations
{
    /// <inheritdoc />
    public partial class RenamedJoinTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TestsQuestions",
                columns: table => new
                {
                    QuestionsId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TestsId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestsQuestions", x => new { x.QuestionsId, x.TestsId });
                    table.ForeignKey(
                        name: "FK_TestsQuestions_Questions_QuestionsId",
                        column: x => x.QuestionsId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestsQuestions_Tests_TestsId",
                        column: x => x.TestsId,
                        principalTable: "Tests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestsQuestions_TestsId",
                table: "TestsQuestions",
                column: "TestsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestsQuestions");
        }
    }
}
