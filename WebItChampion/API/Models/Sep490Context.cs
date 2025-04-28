using Microsoft.EntityFrameworkCore;
using System;

namespace API.Models
{
    public class Sep490Context : DbContext
    {
        public Sep490Context() { }

        public Sep490Context(DbContextOptions<Sep490Context> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<LearningProgress> LearningProgresses { get; set; }
        public DbSet<ExerciseCode> Exercises { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<SubmissionCode> Submissions { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<QuizQuestion> QuizQuestions { get; set; }
        public DbSet<QuizSubmission> QuizSubmissions { get; set; }
        public DbSet<LearningResource> LearningResources { get; set; }
        public DbSet<StudentSkill> StudentSkills { get; set; }
        public DbSet<StudentSkillProgress> StudentSkillProgresses { get; set; }
        public DbSet<UserQuestionHistory> QuestionHistories { get; set; }
        public DbSet<SocialPost> SocialPosts { get; set; }
        public DbSet<SocialComment> SocialComments { get; set; }
        public DbSet<SocialLike> SocialLikes { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Leaderboard> Leaderboards { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<ClassMembership> ClassMemberships { get; set; }
        public DbSet<ClassQuiz> ClassQuizzes { get; set; }
        //public DbSet<LearningPath> LearningPaths { get; set; }
        //public DbSet<LearningPathItem> LearningPathItems { get; set; }
        public DbSet<UserDailyUsage> UserDailyUsages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Unique constraints
            modelBuilder.Entity<SocialLike>()
                .HasIndex(l => new { l.SocialPostID, l.UserId })
                .IsUnique();

            modelBuilder.Entity<ClassMembership>()
                .HasIndex(m => new { m.ClassID, m.StudentID })
                .IsUnique();

            modelBuilder.Entity<ClassQuiz>()
                .HasIndex(cq => new { cq.ClassID, cq.QuizID })
                .IsUnique();

            modelBuilder.Entity<UserDailyUsage>()
                .HasIndex(u => new { u.UserId, u.UsageDate })
                .IsUnique();

            // Configure indexes
            modelBuilder.Entity<SubmissionCode>()
                .HasIndex(s => new { s.UserID, s.ExerciseID })
                .HasDatabaseName("IX_Submission_UserID_ExerciseID");

            modelBuilder.Entity<QuizSubmission>()
                .HasIndex(qs => new { qs.UserID, qs.QuizID })
                .HasDatabaseName("IX_QuizSubmission_UserID_QuizID");

            // Fix for ClassMembership: StudentID reference to User
            modelBuilder.Entity<ClassMembership>()
                .HasOne(cm => cm.Student)
                .WithMany(u => u.ClassMemberships)
                .HasForeignKey(cm => cm.StudentID)
                .OnDelete(DeleteBehavior.NoAction); // Changed from Cascade to NoAction

            // Fix other potential cascade delete issues
            modelBuilder.Entity<QuizSubmission>()
                .HasOne(qs => qs.Question)
                .WithMany(qq => qq.Submissions)
                .HasForeignKey(qs => qs.QuestionID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserQuestionHistory>()
                .HasOne(qh => qh.Question)
                .WithMany(qq => qq.QuestionHistories)
                .HasForeignKey(qh => qh.QuestionID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<SocialLike>()
                .HasOne(l => l.User)
                .WithMany()
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            //modelBuilder.Entity<LearningPathItem>()
            //    .HasOne(lpi => lpi.Exercise)
            //    .WithMany(e => e.LearningPathItems)
            //    .HasForeignKey(lpi => lpi.ExerciseID)
            //    .OnDelete(DeleteBehavior.NoAction);

            //modelBuilder.Entity<LearningPathItem>()
            //    .HasOne(lpi => lpi.Quiz)
            //    .WithMany(q => q.LearningPathItems)
            //    .HasForeignKey(lpi => lpi.QuizID)
            //    .OnDelete(DeleteBehavior.NoAction);

            //modelBuilder.Entity<LearningPathItem>()
            //    .HasOne(lpi => lpi.Resource)
            //    .WithMany(lr => lr.LearningPathItems)
            //    .HasForeignKey(lpi => lpi.ResourceID)
            //    .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<SocialComment>()
                .HasOne(sc => sc.User)
                .WithMany(u => u.SocialComments)
                .HasForeignKey(sc => sc.UserID)
                .OnDelete(DeleteBehavior.NoAction);

            // Keep this cascade delete behavior as it's explicitly mentioned in SQL
            modelBuilder.Entity<UserDailyUsage>()
                .HasOne(u => u.User)
                .WithMany(u => u.UserDailyUsages)
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Quiz>()
                .HasOne(q => q.User)  
                .WithMany(u => u.Quizzes)  
                .HasForeignKey(q => q.UserID)
                .OnDelete(DeleteBehavior.NoAction); // <-- Không cascade delete
        }
    }
}