using System.ComponentModel.DataAnnotations;

namespace CharterAccess.Models
{
    public class UserGroup
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        [Required]
        public int GroupId { get; set; }
        
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        public string AddedBy { get; set; } = string.Empty;
        
            public virtual User User { get; set; } = null!;
        public virtual Group Group { get; set; } = null!;
    }
} 