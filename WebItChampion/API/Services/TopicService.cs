using API.Models;
using API.ViewModels;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public interface ITopicService
    {
        public Task<(string, List<TopicVM>?)> GetAll();
        public Task<(string, TopicVM?)> GetById(int topicId);
        public Task<string> Delete(int topicId);
        public Task<string> ToggleActive(int topicId);
        public Task<string> CreateUpdate(CreateUpdateTopicVM input);
        public Task<(string, List<TopicVM>?)> Search(string searchString);
    }
    public class TopicService : ITopicService
    {
        private readonly IMapper _mapper;
        private readonly Sep490Context _context;

        public TopicService(IMapper mapper, Sep490Context context)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<(string, List<TopicVM>?)> GetAll()
        {
            var list = await _context.Topics.ToListAsync();
            if (list.IsNullOrEmpty()) return ("No Topic available", null);

            var listMapper = _mapper.Map<List<TopicVM>>(list);
            return ("", listMapper);
        }

        public async Task<(string, TopicVM?)> GetById(int topicId)
        {
            if (topicId <= 0) return ("Topic ID is not valid!", null);

            var topic = await _context.Topics.FirstOrDefaultAsync(x => x.TopicID == topicId);
            if (topic == null) return ("Topic not found", null);

            var topicMapper = _mapper.Map<TopicVM>(topic);
            return ("", topicMapper);
        }

        public async Task<(string, List<TopicVM>?)> Search(string searchString)
        {
            if (string.IsNullOrEmpty(searchString)) return ("Search string is not valid!", null);

            var list = await _context.Topics
                .Where(x => x.TopicName.Contains(searchString))
                .ToListAsync();

            if (list.IsNullOrEmpty()) return ("No Topic available", null);

            var listMapper = _mapper.Map<List<TopicVM>>(list);
            return ("", listMapper);
        }

        public async Task<string> Delete(int topicId)
        {
            if (topicId <= 0) return "Topic ID is not valid!";

            var topic = await _context.Topics.FirstOrDefaultAsync(x => x.TopicID == topicId);
            if (topic == null) return "Topic not found";

            topic.IsDeleted = true;
            _context.Topics.Update(topic);
            await _context.SaveChangesAsync();

            return "";
        }

        public async Task<string> ToggleActive(int topicId)
        {
            if (topicId <= 0) return "Topic ID is not valid!";

            var topic = await _context.Topics.FirstOrDefaultAsync(x => x.TopicID == topicId);
            if (topic == null) return "Topic not found";

            topic.IsActive = !topic.IsActive;
            _context.Topics.Update(topic);
            await _context.SaveChangesAsync();

            return "";
        }

        public async Task<string> CreateUpdate(CreateUpdateTopicVM input)
        {
            if (input == null) return ("Topic is not valid!");

            if (input.TopicID > 0)
            {
                var topic = await _context.Topics.FirstOrDefaultAsync(x => x.TopicID == input.TopicID);
                if (topic == null) return "Topic not found";

                topic.TopicName = input.TopicName;
                topic.Description = input.Description;
                topic.IsActive = input.IsActive;
                topic.IsDeleted = input.IsDeleted;

                _context.Topics.Update(topic);
            }
            else
            {
                var newTopic = new Topic
                {
                    TopicName = input.TopicName,
                    Description = input.Description,
                    IsActive = input.IsActive,
                    IsDeleted = input.IsDeleted
                };
                await _context.Topics.AddAsync(newTopic);
            }
            await _context.SaveChangesAsync();
            
            return "";
        }
    }
}
