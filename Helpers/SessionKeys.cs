namespace BlogPlatform.Helpers
{
    // Tập trung tên khoá lưu trong Session để tránh gõ sai chuỗi ở nhiều nơi
    public static class SessionKeys
    {
        public const string UserId = "UserId";
        public const string UserName = "UserName";
        public const string DisplayName = "DisplayName";
        public const string RoleName = "RoleName";
    }
}
