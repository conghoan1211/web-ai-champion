using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class addTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserDailyUsage_Users_UserId",
                table: "UserDailyUsage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserDailyUsage",
                table: "UserDailyUsage");

            migrationBuilder.DropIndex(
                name: "IX_UserDailyUsage_UserId",
                table: "UserDailyUsage");

            migrationBuilder.RenameTable(
                name: "UserDailyUsage",
                newName: "UserDailyUsages");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserDailyUsages",
                table: "UserDailyUsages",
                column: "UsageId");

            migrationBuilder.CreateTable(
                name: "Classes",
                columns: table => new
                {
                    ClassID = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    ClassName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TeacherID = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classes", x => x.ClassID);
                    table.ForeignKey(
                        name: "FK_Classes_Users_TeacherID",
                        column: x => x.TeacherID,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Leaderboards",
                columns: table => new
                {
                    LeaderboardID = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    UserID = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    Score = table.Column<float>(type: "real", nullable: false),
                    Rank = table.Column<int>(type: "int", nullable: false),
                    Period = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leaderboards", x => x.LeaderboardID);
                    table.ForeignKey(
                        name: "FK_Leaderboards_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    NotificationID = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    UserID = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    RelatedID = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.NotificationID);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SocialPosts",
                columns: table => new
                {
                    SocialPostID = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    UserID = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MediaURL = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialPosts", x => x.SocialPostID);
                    table.ForeignKey(
                        name: "FK_SocialPosts_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Topics",
                columns: table => new
                {
                    TopicID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TopicName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Topics", x => x.TopicID);
                });

            migrationBuilder.CreateTable(
                name: "ClassMemberships",
                columns: table => new
                {
                    MembershipID = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    ClassID = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    StudentID = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    JoinDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassMemberships", x => x.MembershipID);
                    table.ForeignKey(
                        name: "FK_ClassMemberships_Classes_ClassID",
                        column: x => x.ClassID,
                        principalTable: "Classes",
                        principalColumn: "ClassID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassMemberships_Users_StudentID",
                        column: x => x.StudentID,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "SocialComments",
                columns: table => new
                {
                    CommentID = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    SocialPostID = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    UserID = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialComments", x => x.CommentID);
                    table.ForeignKey(
                        name: "FK_SocialComments_SocialPosts_SocialPostID",
                        column: x => x.SocialPostID,
                        principalTable: "SocialPosts",
                        principalColumn: "SocialPostID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SocialComments_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "SocialLikes",
                columns: table => new
                {
                    LikeID = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    SocialPostID = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId1 = table.Column<string>(type: "nvarchar(36)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialLikes", x => x.LikeID);
                    table.ForeignKey(
                        name: "FK_SocialLikes_SocialPosts_SocialPostID",
                        column: x => x.SocialPostID,
                        principalTable: "SocialPosts",
                        principalColumn: "SocialPostID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SocialLikes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_SocialLikes_Users_UserId1",
                        column: x => x.UserId1,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Exercises",
                columns: table => new
                {
                    ExerciseID = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DifficultyLevel = table.Column<int>(type: "int", nullable: false),
                    TopicID = table.Column<int>(type: "int", nullable: true),
                    InputFormat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OutputFormat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SampleInput = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SampleOutput = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TestCases = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaxTimeLimit = table.Column<int>(type: "int", nullable: true),
                    MaxMemoryLimit = table.Column<int>(type: "int", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exercises", x => x.ExerciseID);
                    table.ForeignKey(
                        name: "FK_Exercises_Topics_TopicID",
                        column: x => x.TopicID,
                        principalTable: "Topics",
                        principalColumn: "TopicID");
                });

            migrationBuilder.CreateTable(
                name: "LearningResources",
                columns: table => new
                {
                    ResourceID = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    ContentURL = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TopicID = table.Column<int>(type: "int", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LearningResources", x => x.ResourceID);
                    table.ForeignKey(
                        name: "FK_LearningResources_Topics_TopicID",
                        column: x => x.TopicID,
                        principalTable: "Topics",
                        principalColumn: "TopicID");
                });

            migrationBuilder.CreateTable(
                name: "Quizzes",
                columns: table => new
                {
                    QuizID = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DifficultyLevel = table.Column<int>(type: "int", nullable: false),
                    TopicID = table.Column<int>(type: "int", nullable: true),
                    TimeLimit = table.Column<int>(type: "int", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quizzes", x => x.QuizID);
                    table.ForeignKey(
                        name: "FK_Quizzes_Topics_TopicID",
                        column: x => x.TopicID,
                        principalTable: "Topics",
                        principalColumn: "TopicID");
                });

            migrationBuilder.CreateTable(
                name: "StudentSkills",
                columns: table => new
                {
                    SkillID = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    SkillName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TopicID = table.Column<int>(type: "int", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(36)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentSkills", x => x.SkillID);
                    table.ForeignKey(
                        name: "FK_StudentSkills_Topics_TopicID",
                        column: x => x.TopicID,
                        principalTable: "Topics",
                        principalColumn: "TopicID");
                    table.ForeignKey(
                        name: "FK_StudentSkills_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Submissions",
                columns: table => new
                {
                    SubmissionID = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    UserID = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    ExerciseID = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Language = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Score = table.Column<float>(type: "real", nullable: true),
                    ExecutionTime = table.Column<int>(type: "int", nullable: true),
                    MemoryUsed = table.Column<int>(type: "int", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Feedback = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubmitAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Submissions", x => x.SubmissionID);
                    table.ForeignKey(
                        name: "FK_Submissions_Exercises_ExerciseID",
                        column: x => x.ExerciseID,
                        principalTable: "Exercises",
                        principalColumn: "ExerciseID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Submissions_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClassQuizzes",
                columns: table => new
                {
                    ClassQuizID = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    ClassID = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    QuizID = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassQuizzes", x => x.ClassQuizID);
                    table.ForeignKey(
                        name: "FK_ClassQuizzes_Classes_ClassID",
                        column: x => x.ClassID,
                        principalTable: "Classes",
                        principalColumn: "ClassID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassQuizzes_Quizzes_QuizID",
                        column: x => x.QuizID,
                        principalTable: "Quizzes",
                        principalColumn: "QuizID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LearningProgresses",
                columns: table => new
                {
                    ProgressID = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    UserID = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    ExerciseID = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    QuizID = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    Score = table.Column<float>(type: "real", nullable: true),
                    TimeSpent = table.Column<int>(type: "int", nullable: true),
                    CompletionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LearningProgresses", x => x.ProgressID);
                    table.ForeignKey(
                        name: "FK_LearningProgresses_Exercises_ExerciseID",
                        column: x => x.ExerciseID,
                        principalTable: "Exercises",
                        principalColumn: "ExerciseID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LearningProgresses_Quizzes_QuizID",
                        column: x => x.QuizID,
                        principalTable: "Quizzes",
                        principalColumn: "QuizID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LearningProgresses_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuizQuestions",
                columns: table => new
                {
                    QuestionID = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    QuizID = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuestionType = table.Column<int>(type: "int", nullable: false),
                    Options = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CorrectAnswer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Explanation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DifficultyLevel = table.Column<int>(type: "int", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsAICreated = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizQuestions", x => x.QuestionID);
                    table.ForeignKey(
                        name: "FK_QuizQuestions_Quizzes_QuizID",
                        column: x => x.QuizID,
                        principalTable: "Quizzes",
                        principalColumn: "QuizID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentSkillProgresses",
                columns: table => new
                {
                    ProgressID = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    UserID = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    SkillID = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    ProficiencyScore = table.Column<float>(type: "real", nullable: true),
                    SkillLevel = table.Column<int>(type: "int", nullable: true),
                    ProbabilityKnown = table.Column<float>(type: "real", nullable: false),
                    Attempts = table.Column<int>(type: "int", nullable: false),
                    SuccessRate = table.Column<float>(type: "real", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentSkillProgresses", x => x.ProgressID);
                    table.ForeignKey(
                        name: "FK_StudentSkillProgresses_StudentSkills_SkillID",
                        column: x => x.SkillID,
                        principalTable: "StudentSkills",
                        principalColumn: "SkillID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentSkillProgresses_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionHistories",
                columns: table => new
                {
                    HistoryID = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    UserID = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    QuestionID = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    SkillID = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    QuestionContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserAnswer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: true),
                    Score = table.Column<float>(type: "real", nullable: true),
                    TimeTaken = table.Column<int>(type: "int", nullable: true),
                    DifficultyLevel = table.Column<int>(type: "int", nullable: false),
                    IsAIGenerated = table.Column<bool>(type: "bit", nullable: false),
                    SubmitAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionHistories", x => x.HistoryID);
                    table.ForeignKey(
                        name: "FK_QuestionHistories_QuizQuestions_QuestionID",
                        column: x => x.QuestionID,
                        principalTable: "QuizQuestions",
                        principalColumn: "QuestionID");
                    table.ForeignKey(
                        name: "FK_QuestionHistories_StudentSkills_SkillID",
                        column: x => x.SkillID,
                        principalTable: "StudentSkills",
                        principalColumn: "SkillID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionHistories_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuizSubmissions",
                columns: table => new
                {
                    QuizSubmissionID = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    UserID = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    QuizID = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    QuestionID = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    UserAnswer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false),
                    Score = table.Column<float>(type: "real", nullable: true),
                    SubmitAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Feedback = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizSubmissions", x => x.QuizSubmissionID);
                    table.ForeignKey(
                        name: "FK_QuizSubmissions_QuizQuestions_QuestionID",
                        column: x => x.QuestionID,
                        principalTable: "QuizQuestions",
                        principalColumn: "QuestionID");
                    table.ForeignKey(
                        name: "FK_QuizSubmissions_Quizzes_QuizID",
                        column: x => x.QuizID,
                        principalTable: "Quizzes",
                        principalColumn: "QuizID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuizSubmissions_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email_Username",
                table: "Users",
                columns: new[] { "Email", "Username" },
                unique: true,
                filter: "[Email] IS NOT NULL AND [Username] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserDailyUsages_UserId_UsageDate",
                table: "UserDailyUsages",
                columns: new[] { "UserId", "UsageDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Classes_TeacherID",
                table: "Classes",
                column: "TeacherID");

            migrationBuilder.CreateIndex(
                name: "IX_ClassMemberships_ClassID_StudentID",
                table: "ClassMemberships",
                columns: new[] { "ClassID", "StudentID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClassMemberships_StudentID",
                table: "ClassMemberships",
                column: "StudentID");

            migrationBuilder.CreateIndex(
                name: "IX_ClassQuizzes_ClassID_QuizID",
                table: "ClassQuizzes",
                columns: new[] { "ClassID", "QuizID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClassQuizzes_QuizID",
                table: "ClassQuizzes",
                column: "QuizID");

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_TopicID",
                table: "Exercises",
                column: "TopicID");

            migrationBuilder.CreateIndex(
                name: "IX_Leaderboards_UserID",
                table: "Leaderboards",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_LearningProgresses_ExerciseID",
                table: "LearningProgresses",
                column: "ExerciseID");

            migrationBuilder.CreateIndex(
                name: "IX_LearningProgresses_QuizID",
                table: "LearningProgresses",
                column: "QuizID");

            migrationBuilder.CreateIndex(
                name: "IX_LearningProgresses_UserID",
                table: "LearningProgresses",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_LearningResources_TopicID",
                table: "LearningResources",
                column: "TopicID");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserID",
                table: "Notifications",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionHistories_QuestionID",
                table: "QuestionHistories",
                column: "QuestionID");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionHistories_SkillID",
                table: "QuestionHistories",
                column: "SkillID");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionHistories_UserID",
                table: "QuestionHistories",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_QuizQuestions_QuizID",
                table: "QuizQuestions",
                column: "QuizID");

            migrationBuilder.CreateIndex(
                name: "IX_QuizSubmission_UserID_QuizID",
                table: "QuizSubmissions",
                columns: new[] { "UserID", "QuizID" });

            migrationBuilder.CreateIndex(
                name: "IX_QuizSubmissions_QuestionID",
                table: "QuizSubmissions",
                column: "QuestionID");

            migrationBuilder.CreateIndex(
                name: "IX_QuizSubmissions_QuizID",
                table: "QuizSubmissions",
                column: "QuizID");

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_TopicID",
                table: "Quizzes",
                column: "TopicID");

            migrationBuilder.CreateIndex(
                name: "IX_SocialComments_SocialPostID",
                table: "SocialComments",
                column: "SocialPostID");

            migrationBuilder.CreateIndex(
                name: "IX_SocialComments_UserID",
                table: "SocialComments",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_SocialLikes_SocialPostID_UserId",
                table: "SocialLikes",
                columns: new[] { "SocialPostID", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SocialLikes_UserId",
                table: "SocialLikes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SocialLikes_UserId1",
                table: "SocialLikes",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_SocialPosts_UserID",
                table: "SocialPosts",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_StudentSkillProgresses_SkillID",
                table: "StudentSkillProgresses",
                column: "SkillID");

            migrationBuilder.CreateIndex(
                name: "IX_StudentSkillProgresses_UserID",
                table: "StudentSkillProgresses",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_StudentSkills_TopicID",
                table: "StudentSkills",
                column: "TopicID");

            migrationBuilder.CreateIndex(
                name: "IX_StudentSkills_UserId",
                table: "StudentSkills",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Submission_UserID_ExerciseID",
                table: "Submissions",
                columns: new[] { "UserID", "ExerciseID" });

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_ExerciseID",
                table: "Submissions",
                column: "ExerciseID");

            migrationBuilder.AddForeignKey(
                name: "FK_UserDailyUsages_Users_UserId",
                table: "UserDailyUsages",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserDailyUsages_Users_UserId",
                table: "UserDailyUsages");

            migrationBuilder.DropTable(
                name: "ClassMemberships");

            migrationBuilder.DropTable(
                name: "ClassQuizzes");

            migrationBuilder.DropTable(
                name: "Leaderboards");

            migrationBuilder.DropTable(
                name: "LearningProgresses");

            migrationBuilder.DropTable(
                name: "LearningResources");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "QuestionHistories");

            migrationBuilder.DropTable(
                name: "QuizSubmissions");

            migrationBuilder.DropTable(
                name: "SocialComments");

            migrationBuilder.DropTable(
                name: "SocialLikes");

            migrationBuilder.DropTable(
                name: "StudentSkillProgresses");

            migrationBuilder.DropTable(
                name: "Submissions");

            migrationBuilder.DropTable(
                name: "Classes");

            migrationBuilder.DropTable(
                name: "QuizQuestions");

            migrationBuilder.DropTable(
                name: "SocialPosts");

            migrationBuilder.DropTable(
                name: "StudentSkills");

            migrationBuilder.DropTable(
                name: "Exercises");

            migrationBuilder.DropTable(
                name: "Quizzes");

            migrationBuilder.DropTable(
                name: "Topics");

            migrationBuilder.DropIndex(
                name: "IX_Users_Email_Username",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserDailyUsages",
                table: "UserDailyUsages");

            migrationBuilder.DropIndex(
                name: "IX_UserDailyUsages_UserId_UsageDate",
                table: "UserDailyUsages");

            migrationBuilder.RenameTable(
                name: "UserDailyUsages",
                newName: "UserDailyUsage");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserDailyUsage",
                table: "UserDailyUsage",
                column: "UsageId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDailyUsage_UserId",
                table: "UserDailyUsage",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserDailyUsage_Users_UserId",
                table: "UserDailyUsage",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
