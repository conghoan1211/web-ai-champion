using API.Helper;
using API.Models;
using API.ViewModels;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;

namespace API.Services
{
    public interface IQuizSubmissionService
    {
        public Task<string> ProcessQuizSubmission(QuizSubmissionRequest submissionRequest);
        Task UpdateAllSkillProgress(string userId, List<QuizSubmission> submissions, List<QuizQuestion> questions);
        Task UpdateLeaderboard(string userId, string quizId);
        Task RecalculateLeaderboardRanks(string periodType, DateTime periodStart);
        //Task UpdateLearningProgress(string userId, string quizId);
    }
    public class QuizSubmissionService : IQuizSubmissionService
    {
        private readonly Sep490Context _context;
        private readonly IMapper _mapper;
        public QuizSubmissionService(Sep490Context context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        /// <summary>
        /// Process quiz submission and update all relevant tables.
        /// </summary>
        public async Task<string> ProcessQuizSubmission(QuizSubmissionRequest submissionRequest)
        {
            if (string.IsNullOrEmpty(submissionRequest.UserId) || string.IsNullOrEmpty(submissionRequest.QuizId))
                return "UserId and QuizId are required.";
            if (submissionRequest.Answers == null || !submissionRequest.Answers.Any())
                return "No answers provided.";

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var questionIds = submissionRequest.Answers.Select(a => a.QuestionId).ToList();
                var questions = await _context.QuizQuestions
                    .Where(q => questionIds.Contains(q.QuestionID) && q.QuizID == submissionRequest.QuizId)
                    .ToListAsync();

                var userSubmissions = new List<QuizSubmission>();
                var histories = new List<UserQuestionHistory>();

                foreach (var answer in submissionRequest.Answers)
                {
                    var question = questions.FirstOrDefault(q => q.QuestionID == answer.QuestionId);
                    if (question == null)
                        return $"Question {answer.QuestionId} not found in quiz {submissionRequest.QuizId}.";

                    bool isCorrect = answer.UserAnswer.Trim().Equals(question.CorrectAnswer.Trim(), StringComparison.OrdinalIgnoreCase);
                    int score = isCorrect ? question.DifficultyLevel * 5 : 0;

                    var submission = new QuizSubmission
                    {
                        QuizSubmissionID = Guid.NewGuid().ToString(),
                        QuizID = submissionRequest.QuizId,
                        QuestionID = answer.QuestionId,
                        UserID = submissionRequest.UserId,
                        UserAnswer = answer.UserAnswer,
                        IsCorrect = isCorrect,
                        Score = score,
                        SubmitAt = DateTime.UtcNow
                    };
                    userSubmissions.Add(submission);

                    var history = new UserQuestionHistory
                    {
                        HistoryID = Guid.NewGuid().ToString(),
                        UserID = submissionRequest.UserId,
                        QuestionID = answer.QuestionId,
                        SkillID = question.SkillID,
                        QuestionContent = question.Content,
                        UserAnswer = answer.UserAnswer,
                        IsCorrect = isCorrect,
                        Score = score,
                        DifficultyLevel = question.DifficultyLevel,
                        SubmitAt = DateTime.UtcNow,
                        TimeTaken = 0 // TODO: Implement if needed
                    };
                    histories.Add(history);
                }

                await _context.QuizSubmissions.AddRangeAsync(userSubmissions);
                await _context.QuestionHistories.AddRangeAsync(histories);

                await UpdateAllSkillProgress(submissionRequest.UserId, userSubmissions, questions);

                await UpdateLeaderboard(submissionRequest.UserId, submissionRequest.QuizId);
                //await UpdateLearningProgress(submissionRequest.UserId, submissionRequest.QuizId);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return "";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Failed to process quiz submission: {ex.Message}");
            }
        }

        /// <summary>
        /// Update skill progress for all skills involved in the quiz submission.
        /// </summary>
        public async Task UpdateAllSkillProgress(string userId, List<QuizSubmission> submissions, List<QuizQuestion> questions)
        {
            var skillGroups = submissions
                .Join(questions, s => s.QuestionID, q => q.QuestionID, (s, q) => new { Submission = s, Question = q })
                .GroupBy(x => x.Question.SkillID);

            // Preload all recent histories once
            var skillIds = questions.Select(q => q.SkillID).Distinct().ToList();

            var allRecentHistories = await _context.QuestionHistories
                .Where(h => h.UserID == userId && skillIds.Contains(h.SkillID))
                .OrderByDescending(h => h.SubmitAt)
                .Take(100) // Limit to avoid memory issues
                .ToListAsync();

            foreach (var skillGroup in skillGroups)
            {
                string skillId = skillGroup.Key;
                var attempts = skillGroup.ToList();

                // Filter preloaded histories for this skill
                var recentHistories = allRecentHistories
                    .Where(h => h.SkillID == skillId)
                    .OrderByDescending(h => h.SubmitAt)
                    .Take(10)
                    .ToList();

                var progress = await _context.StudentSkillProgresses
                    .FirstOrDefaultAsync(sp => sp.UserID == userId && sp.SkillID == skillId);

                if (progress == null)
                {
                    progress = new StudentSkillProgress
                    {
                        ProgressID = Guid.NewGuid().ToString(),
                        UserID = userId,
                        SkillID = skillId,
                        ProficiencyScore = 1500, // Elo baseline
                        ProbabilityKnown = 0.1f,
                        SuccessRate = 0,
                        SkillLevel = (int)SkillLevel.Beginner,
                        Attempts = 0,
                        LastUpdated = DateTime.UtcNow
                    };
                    _context.StudentSkillProgresses.Add(progress);
                }
                // Update attempts and proficiency (Elo-based)
                foreach (var att in attempts)
                {
                    progress.Attempts += 1;
                    var question = att.Question;
                    var submission = att.Submission;

                    // Elo update for ProficiencyScore
                    int kFactor = progress.Attempts < 30 ? 32 : 16;
                    float expected = (float)(1.0 / (1.0 + Math.Pow(10, (question.DifficultyLevel - progress.ProficiencyScore.Value) / 400.0)));
                    progress.ProficiencyScore += kFactor * (submission.IsCorrect ? 1 - expected : 0 - expected);

                    // Bayesian Knowledge Tracing (BKT) cho ProbabilityKnown
                    //float pL = progress.ProbabilityKnown;
                    //float pG = 0.25f; // Guessing probability
                    //float pS = 0.1f;  // Slip probability
                    //float pCorrect = submission.IsCorrect
                    //    ? (pL * (1 - pS)) + ((1 - pL) * pG)
                    //    : (pL * pS) + ((1 - pL) * (1 - pG));
                    //progress.ProbabilityKnown = submission.IsCorrect
                    //    ? (pL * (1 - pS)) / pCorrect
                    //    : (pL * pS) / pCorrect;
                }

                // Adaptive alpha dựa trên số attempt hiện có
                float baseAlpha = 0.2f;
                float adaptiveAlpha = baseAlpha / MathF.Log(progress.Attempts + 2, 2); // +2 để tránh chia 0
                adaptiveAlpha = MathF.Max(0.01f, adaptiveAlpha);

                // Chỉ tính time decay 1 lần cho cả batch attempts
                float daysSinceLast = (float)(DateTime.UtcNow - progress.LastUpdated).TotalDays;
                float timeDecay = MathF.Exp(-0.1f * daysSinceLast);

                foreach (var attempt in attempts)
                {
                    progress.ProbabilityKnown = (1 - adaptiveAlpha) * progress.ProbabilityKnown * timeDecay +
                        adaptiveAlpha * (attempt.Submission.IsCorrect ? 1 : 0);
                }

                progress.SuccessRate = recent.Any()
                    ? (float)recent.Count(h => h.IsCorrect == true) / recent.Count * 100
                    : 0;

                // Update SkillLevel based on ProficiencyScore (Elo scale)
                progress.SkillLevel = progress.ProficiencyScore switch
                {
                    >= 2000 => (int)SkillLevel.Expert,
                    >= 1800 => (int)SkillLevel.Advanced,
                    >= 1600 => (int)SkillLevel.Intermediate,
                    >= 1400 => (int)SkillLevel.Elementary,
                    _ => (int)SkillLevel.Beginner
                };
                progress.LastUpdated = DateTime.UtcNow;
            }
        }


        /// <summary>
        /// Update Leaderboard after a quiz submission. 
        ///  Tính tổng điểm từ các bài thi (QuizSubmissions), và xếp hạng theo tổng điểm.
        /// </summary>
        public async Task UpdateLeaderboard(string userId, string quizId)
        {
            try
            {
                var quizScore = await _context.QuizSubmissions
                    .Where(s => s.UserID == userId && s.QuizID == quizId)
                    .SumAsync(s => s.Score ?? 0);

                var now = DateTime.UtcNow.Date;

                // Define leaderboard periods
                var periods = new[]
                {
                    new { Type = "daily", Start = now, End = now.AddDays(1).AddTicks(-1) },
                    new { Type = "weekly", Start = now.AddDays(-(int)now.DayOfWeek), End = now.AddDays(7 - (int)now.DayOfWeek).AddTicks(-1) },
                    new { Type = "monthly", Start = new DateTime(now.Year, now.Month, 1), End = new DateTime(now.Year, now.Month, 1).AddMonths(1).AddTicks(-1) }
                };

                var affectedLeaderboards = new List<(string PeriodType, DateTime PeriodStart)>();

                foreach (var period in periods)
                {
                    var entry = await _context.Leaderboards.FirstOrDefaultAsync(l =>
                        l.UserID == userId &&
                        l.Period == period.Type &&
                        l.PeriodStart == period.Start);

                    if (entry == null)
                    {
                        entry = new Leaderboard
                        {
                            LeaderboardID = Guid.NewGuid().ToString(),
                            UserID = userId,
                            Score = 0,
                            Rank = 0,
                            Period = period.Type,
                            PeriodStart = period.Start,
                            PeriodEnd = period.End,
                            AccuracyRate = 0,
                            TotalQuizzes = 0,
                            UpdateAt = DateTime.UtcNow
                        };
                        _context.Leaderboards.Add(entry);
                    }
                    entry.TotalQuizzes += 1;
                    entry.AccuracyRate = (entry.AccuracyRate * (entry.TotalQuizzes - 1) + quizScore) / entry.TotalQuizzes;
                    entry.Score += quizScore;
                    entry.UpdateAt = DateTime.UtcNow;

                    affectedLeaderboards.Add((period.Type, period.Start));
                }

                await _context.SaveChangesAsync();

                // Recalculate ranks by affected leaderboard groups
                foreach (var (periodType, periodStart) in affectedLeaderboards)
                {
                    await RecalculateLeaderboardRanks(periodType, periodStart);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update leaderboard: {ex.Message}");
            }
        }
        /// <summary>
        /// Recalculate leaderboard ranks for a specific period.
        /// </summary>
        /// <param name="periodType"></param>
        /// <param name="periodStart"></param>
        /// <returns></returns>
        public async Task RecalculateLeaderboardRanks(string periodType, DateTime periodStart)
        {
            var leaderboards = await _context.Leaderboards
                .Where(l => l.Period == periodType && l.PeriodStart == periodStart)
                .OrderByDescending(l => l.Score)
                .ThenBy(l => l.UpdateAt) // optional tie-breaker
                .ToListAsync();

            int rank = 1;
            float? previousScore = null;
            int duplicateCount = 0;

            for (int i = 0; i < leaderboards.Count; i++)
            {
                var lb = leaderboards[i];

                if (lb.Score != previousScore)
                {
                    rank = i + 1;
                    duplicateCount = 1;
                }
                else
                {
                    duplicateCount++;
                }

                lb.Rank = rank;
                previousScore = lb.Score;
            }

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Update LearningProgress after a quiz submission (optional).
        /// </summary>
        /// <param name="userId">Student's UserID</param>
        /// <param name="quizId">QuizID</param>
        //public async Task UpdateLearningProgress(string userId, string quizId)
        //{
        //    try
        //    {
        //        // Get topics involved in the quiz
        //        var topics = await _context.QuizQuestions
        //            .Where(q => q.QuizID == quizId)
        //            .Join(_context.StudentSkills,
        //                q => q.SkillID,
        //                s => s.SkillID,
        //                (q, s) => s.TopicID)
        //            .Distinct()
        //            .ToListAsync();

        //        // Calculate quiz score
        //        var quizScore = await _context.QuizSubmissions
        //            .Where(s => s.UserID == userId && s.QuizID == quizId)
        //            .SumAsync(s => s.Score ?? 0);

        //        foreach (var topicId in topics)
        //        {
        //            // Find or create LearningProgress
        //            var progress = await _context.LearningProgresses
        //                .FirstOrDefaultAsync(lp => lp.UserID == userId && lp.TopicID == topicId);

        //            if (progress == null)
        //            {
        //                progress = new LearningProgress
        //                {
        //                    ProgressID = Guid.NewGuid().ToString(),
        //                    UserID = userId,
        //                    TopicID = topicId,
        //                    CompletionPercentage = 0,
        //                    Score = 0,
        //                    LastUpdated = DateTime.UtcNow
        //                };
        //                _context.LearningProgresses.Add(progress);
        //            }

        //            // Update Points
        //            progress.Points = (progress.Points ?? 0) + quizScore;

        //            // Update CompletionPercentage (example: based on quiz completion)
        //            var totalQuizzes = await _context.QuizQuestions
        //                .Join(_context.StudentSkills,
        //                    q => q.SkillID,
        //                    s => s.SkillID,
        //                    (q, s) => new { q.QuizID, s.TopicID })
        //                .Where(q => q.TopicID == topicId)
        //                .Select(q => q.QuizID)
        //                .Distinct()
        //                .CountAsync();

        //            var completedQuizzes = await _context.QuizSubmissions
        //                .Join(_context.QuizQuestions,
        //                    s => s.QuestionID,
        //                    q => q.QuestionID,
        //                    (s, q) => new { s.UserID, s.QuizID, q.SkillID })
        //                .Join(_context.StudentSkills,
        //                    sq => sq.SkillID,
        //                    s => s.SkillID,
        //                    (sq, s) => new { sq.UserID, sq.QuizID, s.TopicID })
        //                .Where(sq => sq.UserID == userId && sq.TopicID == topicId)
        //                .Select(sq => sq.QuizID)
        //                .Distinct()
        //                .CountAsync();

        //            progress.CompletionPercentage = totalQuizzes > 0
        //                ? (double)(completedQuizzes + 1) / totalQuizzes * 100
        //                : 100;

        //            // Update LastUpdated
        //            progress.LastUpdated = DateTime.UtcNow;
        //        }

        //        await _context.SaveChangesAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"Failed to update learning progress: {ex.Message}");
        //    }
        //}
    }
}
