using API.Helper;
using API.Models;
using API.ViewModels;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace API.Services
{
    public interface IQuizService
    {
        public Task<(string, List<QuizVM>?)> GetListByTopicId(List<int> topicId, int? difficulty);
        public Task<(string, QuizVM?)> GetByQuizId(string quizId);
        public Task<(string, List<QuizVM>?)> GetAll();
        public Task<(string, List<QuizVM>?)> GetByUserId(string userId);
    }
    public class QuizService : IQuizService
    {
        private readonly Sep490Context _context;
        private readonly IMapper _mapper;

        public QuizService(Sep490Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<(string, List<QuizVM>?)> GetListByTopicId(List<int> topicIds, int? difficulty)
        {
            if (topicIds.Count <= 0) return ("Topic Id was not found!", null);

            //var list = await _context.QuizTopics
            //    .Where(qt => topicIds.Contains(qt.TopicID) && qt.IsDeleted == false && qt.IsActive == true)
            //    .Select(qt => qt.Quiz)
            //    .Where(q => q.Status != (int)QuizStatus.Deleted && (!difficulty.HasValue || q.DifficultyLevel == difficulty))
            //    .Distinct()
            //    .ToListAsync();
            var list = await _context.Quizzes
                 .Where(q => q.QuizTopics.Any(qt => topicIds.Contains(qt.TopicID))
                    && (!difficulty.HasValue || q.DifficultyLevel == difficulty)
                    && q.Status != (int)QuizStatus.Deleted)
                 .Select(quiz => new QuizVM
                 {
                     QuizID = quiz.QuizID,
                     Title = quiz.Title,
                     Description = quiz.Description,
                     DifficultyLevel = quiz.DifficultyLevel,
                     TimeLimit = quiz.TimeLimit,
                     Status = quiz.Status,
                     CreateAt = quiz.CreateAt,
                     UpdateAt = quiz.UpdateAt,
                     UserID = quiz.UserID,
                     TopicIDs = quiz.QuizTopics.Where(qt => topicIds.Contains(qt.TopicID))
                            .Select(qt => qt.TopicID).ToList(),
                     TopicNames = quiz.QuizTopics.Where(qt => topicIds.Contains(qt.TopicID))
                            .Select(qt => qt.Topic.TopicName).ToList()
                 })

                  .ToListAsync();
            if (list.Count == 0) return ("No quiz found!", null);

            var mapper = _mapper.Map<List<QuizVM>>(list);
            return ("", mapper);
        }

        public async Task<(string, QuizVM?)> GetByQuizId(string quizId)
        {
            if (string.IsNullOrEmpty(quizId)) return ("Quiz Id was not found!", null);

            var quiz = await _context.Quizzes.FirstOrDefaultAsync(x => x.QuizID == quizId 
                && x.Status != (int)QuizStatus.Deleted);
            if (quiz == null) return ("No quiz found!", null);

            var mapper = _mapper.Map<QuizVM>(quiz);
            return ("", mapper);
        }

        public async Task<(string, List<QuizVM>?)> GetAll()
        {
            var list = await _context.Quizzes.ToListAsync();
            if (list.Count == 0) return ("No quiz found!", null);

            var mapper = _mapper.Map<List<QuizVM>>(list);
            return ("", mapper);
        }

        public async Task<(string, List<QuizVM>?)> GetByUserId(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return ("User Id was not found!", null);

            var list = await _context.Quizzes.Where(x => x.UserID == userId).ToListAsync();
            if (list.Count == 0) return ("No quiz found!", null);

            var mapper = _mapper.Map<List<QuizVM>>(list);
            return ("", mapper);
        }
    }
}
