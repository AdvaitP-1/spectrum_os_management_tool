using System.ComponentModel.DataAnnotations;

namespace CharterAccess.Models
{
    public class User
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [StringLength(10)]
        public string CurrentEnvironment { get; set; } = "QA";
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastLoginAt { get; set; } = DateTime.UtcNow;
        
            public virtual ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();
    }
} 