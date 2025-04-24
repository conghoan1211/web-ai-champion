using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace API.Models;

[Index(nameof(Email), nameof(Username), IsUnique = true)]
public partial class User
{
    [Key]
    [StringLength(36)]
    public string UserId { get; set; } = null!;
    [MaxLength(255)]  public string? Username { get; set; }
    [StringLength(10)] public string? Phone { get; set; }
    [MaxLength(255)] public string? Email { get; set; }
    [MaxLength(255)] public string? Password { get; set; }
    [MaxLength(255)] public string? Avatar { get; set; }
    public int? RoleId { get; set; }
    [MaxLength(255)]
    public string? GoogleId { get; set; }
    [MaxLength(255)]
    public string? FacebookId { get; set; }
    public int? Sex { get; set; }
    public DateTime? Dob { get; set; }
    [MaxLength(150)]
    public string? Bio { get; set; }
    public string? Address { get; set; }
    [StringLength(20)]
    public string? ProvinceId { get; set; }
    [StringLength(20)]
    public string? DistrictId { get; set; }
    [MaxLength(225)]
    public string? DistrictName { get; set; }
    [MaxLength(225)]
    public string? ProvinceName { get; set; }
    public bool? IsDisable { get; set; }        //     lock profile by user
    public bool? IsActive { get; set; }         //     active that set by admin
    public int? Status { get; set; }            //     0: inactive, 1: active   status of user
    public bool? IsVerified { get; set; }       //     status verify of email
    public DateTime? BlockUntil { get; set; }
    public DateTime? LastLogin { get; set; }
    public string? LastLoginIp { get; set; }
    public DateTime? CreateAt { get; set; }
    public DateTime? UpdateAt { get; set; }
    public string? CreateUser { get; set; }
    public string? UpdateUser { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? ExpiryDateToken { get; set; }

    public virtual ICollection<UserDailyUsage> UserDailyUsages { get; set; } = new List<UserDailyUsage>();
    // Navigation properties
    public virtual ICollection<LearningProgress> LearningProgresses { get; set; } = new List<LearningProgress>();
    public virtual ICollection<SubmissionCode> Submissions { get; set; } = new List<SubmissionCode>();
    public virtual ICollection<QuizSubmission> QuizSubmissions { get; set; } = new List<QuizSubmission>();
    public virtual ICollection<StudentSkillProgress> SkillProgresses { get; set; }  = new List<StudentSkillProgress>();
    public virtual ICollection<UserQuestionHistory> QuestionHistories { get; set; } = new List<UserQuestionHistory>();
    public virtual ICollection<SocialPost> SocialPosts { get; set; } = new List<SocialPost>();
    public virtual ICollection<SocialComment> SocialComments { get; set; } = new List<SocialComment>();
    public virtual ICollection<SocialLike> SocialLikes { get; set; }
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public virtual ICollection<Leaderboard> Leaderboards { get; set; } = new List<Leaderboard>();
    public virtual ICollection<StudentSkill> Classes { get; set; } = new List<StudentSkill>();
    public virtual ICollection<ClassMembership> ClassMemberships { get; set; } = new List<ClassMembership>();
 //   public virtual ICollection<LearningPath> LearningPaths { get; set; } = new List<LearningPath>();

}
