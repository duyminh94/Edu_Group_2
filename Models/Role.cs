using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace BlogPlatform.Models
{
    // Vai trò người dùng — seed sẵn 3 dòng: Admin, Author, Reader
    [Index(nameof(Name), IsUnique = true)]
    public class Role
    {
        [Key]
        public int Id { get; set; }

        // Tên vai trò, dùng để so sánh trong SessionAuthorizeAttribute
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = null!;

        [StringLength(200)]
        public string? Description { get; set; }

        // Navigation: 1 vai trò có nhiều người dùng
        public List<User> Users { get; set; } = new();
    }
}
