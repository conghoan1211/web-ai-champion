using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class quizTopicTablemanyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quizzes_Topics_TopicID",
                table: "Quizzes");

            migrationBuilder.DropIndex(
                name: "IX_Quizzes_TopicID",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "TopicID",
                table: "Quizzes");

            migrationBuilder.CreateTable(
                name: "QuizTopics",
                columns: table => new
                {
                    QuizTopicID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    QuizID = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    TopicID = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsPrimaryTopic = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizTopics", x => x.QuizTopicID);
                    table.ForeignKey(
                        name: "FK_QuizTopics_Quizzes_QuizID",
                        column: x => x.QuizID,
                        principalTable: "Quizzes",
                        principalColumn: "QuizID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuizTopics_Topics_TopicID",
                        column: x => x.TopicID,
                        principalTable: "Topics",
                        principalColumn: "TopicID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Leaderboards_Period_PeriodStart_Score",
                table: "Leaderboards",
                columns: new[] { "Period", "PeriodStart", "Score" });

            migrationBuilder.CreateIndex(
                name: "IX_Leaderboards_Period_UserID_PeriodStart",
                table: "Leaderboards",
                columns: new[] { "Period", "UserID", "PeriodStart" });

            migrationBuilder.CreateIndex(
                name: "IX_QuizTopics_QuizID",
                table: "QuizTopics",
                column: "QuizID");

            migrationBuilder.CreateIndex(
                name: "IX_QuizTopics_TopicID",
                table: "QuizTopics",
                column: "TopicID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuizTopics");

            migrationBuilder.DropIndex(
                name: "IX_Leaderboards_Period_PeriodStart_Score",
                table: "Leaderboards");

            migrationBuilder.DropIndex(
                name: "IX_Leaderboards_Period_UserID_PeriodStart",
                table: "Leaderboards");

            migrationBuilder.AddColumn<int>(
                name: "TopicID",
                table: "Quizzes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_TopicID",
                table: "Quizzes",
                column: "TopicID");

            migrationBuilder.AddForeignKey(
                name: "FK_Quizzes_Topics_TopicID",
                table: "Quizzes",
                column: "TopicID",
                principalTable: "Topics",
                principalColumn: "TopicID");
        }
    }
}
