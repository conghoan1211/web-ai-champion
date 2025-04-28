using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class addIndexStudentSkillTopicaddUserIdQuizTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserID",
                table: "Quizzes",
                type: "nvarchar(36)",
                maxLength: 36,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Topics_TopicName",
                table: "Topics",
                column: "TopicName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentSkills_SkillID_TopicID",
                table: "StudentSkills",
                columns: new[] { "SkillID", "TopicID" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentSkills_SkillName_TopicID",
                table: "StudentSkills",
                columns: new[] { "SkillName", "TopicID" },
                unique: true,
                filter: "[TopicID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_UserID",
                table: "Quizzes",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Quizzes_Users_UserID",
                table: "Quizzes",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quizzes_Users_UserID",
                table: "Quizzes");

            migrationBuilder.DropIndex(
                name: "IX_Topics_TopicName",
                table: "Topics");

            migrationBuilder.DropIndex(
                name: "IX_StudentSkills_SkillID_TopicID",
                table: "StudentSkills");

            migrationBuilder.DropIndex(
                name: "IX_StudentSkills_SkillName_TopicID",
                table: "StudentSkills");

            migrationBuilder.DropIndex(
                name: "IX_Quizzes_UserID",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "Quizzes");
        }
    }
}
