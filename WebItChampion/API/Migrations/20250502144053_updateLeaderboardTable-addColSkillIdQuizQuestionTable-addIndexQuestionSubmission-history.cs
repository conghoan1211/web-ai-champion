using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class updateLeaderboardTableaddColSkillIdQuizQuestionTableaddIndexQuestionSubmissionhistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_QuizSubmissions_QuizID",
                table: "QuizSubmissions");

            migrationBuilder.DropIndex(
                name: "IX_QuestionHistories_UserID",
                table: "QuestionHistories");

            migrationBuilder.AddColumn<string>(
                name: "SkillID",
                table: "QuizQuestions",
                type: "nvarchar(36)",
                maxLength: 36,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<float>(
                name: "AccuracyRate",
                table: "Leaderboards",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<DateTime>(
                name: "PeriodEnd",
                table: "Leaderboards",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "PeriodStart",
                table: "Leaderboards",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "TotalQuizzes",
                table: "Leaderboards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_QuizSubmissions_QuizID_UserID",
                table: "QuizSubmissions",
                columns: new[] { "QuizID", "UserID" });

            migrationBuilder.CreateIndex(
                name: "IX_QuizQuestions_SkillID",
                table: "QuizQuestions",
                column: "SkillID");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionHistories_UserID_SkillID",
                table: "QuestionHistories",
                columns: new[] { "UserID", "SkillID" });

            migrationBuilder.AddForeignKey(
                name: "FK_QuizQuestions_StudentSkills_SkillID",
                table: "QuizQuestions",
                column: "SkillID",
                principalTable: "StudentSkills",
                principalColumn: "SkillID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizQuestions_StudentSkills_SkillID",
                table: "QuizQuestions");

            migrationBuilder.DropIndex(
                name: "IX_QuizSubmissions_QuizID_UserID",
                table: "QuizSubmissions");

            migrationBuilder.DropIndex(
                name: "IX_QuizQuestions_SkillID",
                table: "QuizQuestions");

            migrationBuilder.DropIndex(
                name: "IX_QuestionHistories_UserID_SkillID",
                table: "QuestionHistories");

            migrationBuilder.DropColumn(
                name: "SkillID",
                table: "QuizQuestions");

            migrationBuilder.DropColumn(
                name: "AccuracyRate",
                table: "Leaderboards");

            migrationBuilder.DropColumn(
                name: "PeriodEnd",
                table: "Leaderboards");

            migrationBuilder.DropColumn(
                name: "PeriodStart",
                table: "Leaderboards");

            migrationBuilder.DropColumn(
                name: "TotalQuizzes",
                table: "Leaderboards");

            migrationBuilder.CreateIndex(
                name: "IX_QuizSubmissions_QuizID",
                table: "QuizSubmissions",
                column: "QuizID");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionHistories_UserID",
                table: "QuestionHistories",
                column: "UserID");
        }
    }
}
