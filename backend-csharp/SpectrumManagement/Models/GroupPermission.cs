using System.ComponentModel.DataAnnotations;

namespace CharterAccess.Models
{
    public class GroupPermission
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int GroupId { get; set; }
        
        [Required]
        public int PermissionId { get; set; }
        
        public DateTime GrantedAt { get; set; } = DateTime.UtcNow;
        public string GrantedBy { get; set; } = string.Empty;
        
            public virtual Group Group { get; set; } = null!;
        public virtual Permission Permission { get; set; } = null!;
    }
} 