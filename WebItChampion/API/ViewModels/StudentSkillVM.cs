using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace API.ViewModels
{
    public class StudentSkillVM
    {
        public string SkillID { get; set; } = null!;
        public string? SkillName { get; set; }
        public string? Description { get; set; }
        public int? TopicID { get; set; }
        public bool IsCoreSkill { get; set; } = false;
        public DateTime CreateAt { get; set; } = DateTime.Now;
    }

    public class CreateUpdateSkillVM
    {
        public string? SkillID { get; set; }
        [Required(ErrorMessage = "Tên kĩ năng không được để trống")]
        [StringLength(100, ErrorMessage = "Tên kĩ năng không quá 100 ký tự")]
        public string? SkillName { get; set; }
        public string? Description { get; set; }
        public int? TopicID { get; set; }
    }

    public class GeneratedSkill
    {
        public string? SkillName { get; set; }
        public string? Description { get; set; }
        public string? TopicName { get; set; }
    }

}
