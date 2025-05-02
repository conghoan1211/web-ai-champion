using API.Configurations;

namespace API.Helper
{
    public static class Constant
    {
        public static readonly string UrlImagePath = "wwwroot/img";
        public static readonly IList<string> IMAGE_EXTENDS = new List<string> { ".png", ".jpg", ".jpeg" }.AsReadOnly();

        public const long AVATAR_FILE_SIZE = 1 * 1024 * 1024;
    }

    public static class ConstMessage
    {
        public static readonly string ACCOUNT_UNVERIFIED = "Tài khoản chưa được xác minh.";
        public static readonly string EMAIL_EXISTED = "Email này đã tồn tại.";

        public static readonly string NOTIFY_LIKE_POST = "liked your post.";
        public static readonly string NOTIFY_COMMENT_POST = "commented: ";
        public static readonly string NOTIFY_LIKE_COMMENT = "liked your comment: ";
        public static readonly string NOTIFY_NEW_FOLLOW = "started following you.";
        public static readonly string NOTIFY_ACCEPTED_FRIENDS = "accepted your friends request.";

        public static readonly string CHATAI_DEFAULT_MODEL = "gpt-3.5-turbo";
    }

    public static class UrlS3
    {
        public static readonly string UrlMain = ConfigManager.gI().UrlS3Key;
        public static readonly string Profile = "profile/";

    }

    public enum NotifyType
    {
        FriendRequest = 0,
        FriendAccept,
        PostLike,
        PostComment,
        CommentLike,
        Message
    }

    public enum UserStatus
    {
        Inactive = 0, // Người dùng không hoạt động
        Active,   // Người dùng đang hoạt động
    }

    public enum PostPrivacy
    {
        Public = 0,
        Private,
    }

    public enum Gender
    {
        Male = 0,
        Female,
        Other,
    }

    public enum Role
    {
        User = 0,
        Teacher = 1,
        Admin,
    }

    public enum ChatbotRole
    {
        User = 0,
        Assistant,
        System,
    }

    public enum DifficultyLevel
    {
        Easy = 1,
        Medium,
        Hard,
        VeryHard,
    }

    public enum QuestionType
    {
        Essay = 0,
        MultipleChoice,
    }

    public enum QuizStatus
    {
        Private = 0,
        Public = 1, 
        Hide = 2,
        Deleted = 3,
    }

    public enum SkillLevel
    {
        Beginner = 0,
        Elementary,
        Intermediate,
        Advanced,
        Expert,
    }
}
