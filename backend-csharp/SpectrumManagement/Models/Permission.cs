using System.ComponentModel.DataAnnotations;

namespace SpectrumManagement.Models
{
    public class Permission
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string Category { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
            public virtual ICollection<GroupPermission> GroupPermissions { get; set; } = new List<GroupPermission>();
    }
} 