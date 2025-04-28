using API.Models;
using API.ViewModels;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public interface ISkillCoreService
    {
        public Task<(string, List<StudentSkillVM>?)> GetAll();
        public Task<(string, StudentSkillVM?)> GetById(string id);
        public Task<string> Delete(string id);
        public Task<(string, CreateUpdateSkillVM?)> CreateUpdate(CreateUpdateSkillVM skillVM);
        public Task<(string, List<StudentSkillVM>?)> Search(string searchString);
    }
    public class SkillCoreService : ISkillCoreService
    {
        private readonly IMapper _mapper;
        private readonly Sep490Context _context;

        public SkillCoreService(IMapper mapper, Sep490Context context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<(string, List<StudentSkillVM>?)> GetAll()
        {
            var list = await _context.StudentSkills.ToListAsync();
            if (list.IsNullOrEmpty()) return ("No skill available", null);

            var listMapper = _mapper.Map<List<StudentSkillVM>>(list);
            return ("", listMapper);
        }

        public async Task<(string, StudentSkillVM?)> GetById(string id)
        {
            var skill = await _context.StudentSkills.FirstOrDefaultAsync(x => x.SkillID == id);
            if (skill == null) return ("Skill not found", null);

            var skillMapper = _mapper.Map<StudentSkillVM>(skill);
            return ("", skillMapper);
        }

        public async Task<string> Delete(string id)
        {
            var skill = await _context.StudentSkills.FirstOrDefaultAsync(x => x.SkillID == id);
            if (skill == null) return ("Skill not found");

            _context.StudentSkills.Remove(skill);
            await _context.SaveChangesAsync();
            return "";
        }

        public async Task<(string, List<StudentSkillVM>?)> Search(string searchString)
        {
            if (string.IsNullOrEmpty(searchString)) return ("Search string is not valid!", null);

            var list = await _context.StudentSkills
                .Where(x => x.SkillName.Contains(searchString))
                .ToListAsync();

            if (list.IsNullOrEmpty()) return ("No skill available", null);

            var listMapper = _mapper.Map<List<StudentSkillVM>>(list);
            return ("", listMapper);
        }

        public async Task<(string, CreateUpdateSkillVM?)> CreateUpdate(CreateUpdateSkillVM skillVM)
        {
            if (skillVM == null) return ("Skill is null", null);

            if (skillVM.SkillID.IsNullOrEmpty())
            {
                var existingSkill = await _context.StudentSkills.FirstOrDefaultAsync(x => x.SkillName == skillVM.SkillName && x.TopicID == skillVM.TopicID);
                if (existingSkill == null) return ("Skill already exists", null);

                var newSkill = new StudentSkill
                {
                    SkillID = Guid.NewGuid().ToString(),
                    SkillName = skillVM.SkillName,
                    Description = skillVM.Description,
                    CreateAt = DateTime.UtcNow,
                    IsCoreSkill = true,
                    TopicID = skillVM.TopicID,
                };
                await _context.StudentSkills.AddAsync(newSkill);
            }
            else
            {
                var skill = await _context.StudentSkills.FirstOrDefaultAsync(x => x.SkillID == skillVM.SkillID);
                if (skill == null) return ("Skill not found", null);
                
                skill.SkillName = skillVM.SkillName;
                skill.Description = skillVM.Description;
                skill.TopicID = skillVM.TopicID;
                skill.UpdateAt = DateTime.UtcNow;

                _context.StudentSkills.Update(skill);
            }
            await _context.SaveChangesAsync();
            return ("", skillVM);
        }
    }
}
