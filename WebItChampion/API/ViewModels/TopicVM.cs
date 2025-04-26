using System.ComponentModel.DataAnnotations;

namespace API.ViewModels
{
    public class TopicVM
    {
        public int TopicID { get; set; }
        public string? TopicName { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
    }
    public class CreateUpdateTopicVM
    {
        public int TopicID { get; set; }
        [Required(ErrorMessage = "Tên chủ đề không được để trống")]
        [StringLength(100, ErrorMessage = "Tên chủ đề không quá 100 ký tự")]
        public string? TopicName { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
    }

}
