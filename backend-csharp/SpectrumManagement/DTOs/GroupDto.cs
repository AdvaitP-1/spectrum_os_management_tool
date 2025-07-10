namespace SpectrumManagement.DTOs
{
    public class GroupDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Environment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public List<PermissionDto> Permissions { get; set; } = new List<PermissionDto>();
        public List<SimpleUserDto> Users { get; set; } = new List<SimpleUserDto>();
        public int UserCount { get; set; }
    }

    public class SimpleGroupDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Environment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int UserCount { get; set; }
    }

    public class CreateGroupDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Environment { get; set; } = string.Empty;
        public List<int> PermissionIds { get; set; } = new List<int>();
    }

    public class UpdateGroupDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<int> PermissionIds { get; set; } = new List<int>();
    }

    public class GroupMembershipDto
    {
        public string UserId { get; set; } = string.Empty;
        public int GroupId { get; set; }
    }
} 