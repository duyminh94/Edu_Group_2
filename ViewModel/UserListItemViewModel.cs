namespace BlogPlatform.ViewModel
{
    // Một dòng người dùng trong bảng quản lý user của Admin (Issue #13, UC25)
    //
    // ⚠️ Không có property PasswordHash — dữ liệu nhạy cảm không được rời tầng Service
    //
    // 👥 Khu A sở hữu
    public class UserListItemViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;

        // ===== Vai trò — Admin dùng để đổi role (quy tắc 3.10, 3.12) =====
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;

        // Quy tắc 3.7 — true thì chặn đăng nhập, nhưng bài viết vẫn hiển thị (quy tắc 3.13)
        public bool IsLocked { get; set; }

        public DateTime CreatedAt { get; set; }

        // Quy tắc 3.14 — còn bài viết thì không cho xoá user
        public int PostCount { get; set; }
    }
}
