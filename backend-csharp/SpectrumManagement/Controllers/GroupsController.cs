using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpectrumManagement.Data;
using SpectrumManagement.DTOs;
using SpectrumManagement.Models;

namespace SpectrumManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroupsController : ControllerBase
    {
        private readonly SpectrumDbContext _context;

        public GroupsController(SpectrumDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GroupDto>>> GetGroups([FromQuery] string? environment = null)
        {
            var query = _context.Groups
                .Include(g => g.UserGroups)
                    .ThenInclude(ug => ug.User)
                .Include(g => g.GroupPermissions)
                    .ThenInclude(gp => gp.Permission)
                .AsQueryable();

            if (!string.IsNullOrEmpty(environment))
            {
                query = query.Where(g => g.Environment == environment);
            }

            var groups = await query.ToListAsync();

            var groupDtos = groups.Select(g => new GroupDto
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                Environment = g.Environment,
                CreatedAt = g.CreatedAt,
                CreatedBy = g.CreatedBy,
                UserCount = g.UserGroups.Count,
                Users = g.UserGroups.Select(ug => new SimpleUserDto
                {
                    Id = ug.User.Id,
                    Name = ug.User.Name,
                    Email = ug.User.Email,
                    CurrentEnvironment = ug.User.CurrentEnvironment
                }).ToList(),
                Permissions = g.GroupPermissions.Select(gp => new PermissionDto
                {
                    Id = gp.Permission.Id,
                    Name = gp.Permission.Name,
                    Description = gp.Permission.Description,
                    Category = gp.Permission.Category,
                    CreatedAt = gp.Permission.CreatedAt
                }).ToList()
            }).ToList();

            return Ok(groupDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GroupDto>> GetGroup(int id)
        {
            var group = await _context.Groups
                .Include(g => g.UserGroups)
                    .ThenInclude(ug => ug.User)
                .Include(g => g.GroupPermissions)
                    .ThenInclude(gp => gp.Permission)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (group == null)
            {
                return NotFound();
            }

            var groupDto = new GroupDto
            {
                Id = group.Id,
                Name = group.Name,
                Description = group.Description,
                Environment = group.Environment,
                CreatedAt = group.CreatedAt,
                CreatedBy = group.CreatedBy,
                UserCount = group.UserGroups.Count,
                Users = group.UserGroups.Select(ug => new SimpleUserDto
                {
                    Id = ug.User.Id,
                    Name = ug.User.Name,
                    Email = ug.User.Email,
                    CurrentEnvironment = ug.User.CurrentEnvironment
                }).ToList(),
                Permissions = group.GroupPermissions.Select(gp => new PermissionDto
                {
                    Id = gp.Permission.Id,
                    Name = gp.Permission.Name,
                    Description = gp.Permission.Description,
                    Category = gp.Permission.Category,
                    CreatedAt = gp.Permission.CreatedAt
                }).ToList()
            };

            return Ok(groupDto);
        }

        [HttpPost]
        public async Task<ActionResult<GroupDto>> CreateGroup(CreateGroupDto createGroupDto)
        {
            var group = new Group
            {
                Name = createGroupDto.Name,
                Description = createGroupDto.Description,
                Environment = createGroupDto.Environment,
                CreatedBy = "P1234567" // In a real app, this would come from authentication
            };

            _context.Groups.Add(group);
            await _context.SaveChangesAsync();

            // Add permissions to the group
            if (createGroupDto.PermissionIds.Any())
            {
                var groupPermissions = createGroupDto.PermissionIds.Select(permissionId => new GroupPermission
                {
                    GroupId = group.Id,
                    PermissionId = permissionId,
                    GrantedBy = "P1234567"
                }).ToList();

                _context.GroupPermissions.AddRange(groupPermissions);
                await _context.SaveChangesAsync();
            }

            // Reload the group with its permissions
            var createdGroup = await _context.Groups
                .Include(g => g.GroupPermissions)
                    .ThenInclude(gp => gp.Permission)
                .FirstOrDefaultAsync(g => g.Id == group.Id);

            var groupDto = new GroupDto
            {
                Id = createdGroup!.Id,
                Name = createdGroup.Name,
                Description = createdGroup.Description,
                Environment = createdGroup.Environment,
                CreatedAt = createdGroup.CreatedAt,
                CreatedBy = createdGroup.CreatedBy,
                UserCount = 0,
                Users = new List<SimpleUserDto>(),
                Permissions = createdGroup.GroupPermissions.Select(gp => new PermissionDto
                {
                    Id = gp.Permission.Id,
                    Name = gp.Permission.Name,
                    Description = gp.Permission.Description,
                    Category = gp.Permission.Category,
                    CreatedAt = gp.Permission.CreatedAt
                }).ToList()
            };

            return CreatedAtAction(nameof(GetGroup), new { id = group.Id }, groupDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGroup(int id, UpdateGroupDto updateGroupDto)
        {
            var group = await _context.Groups
                .Include(g => g.GroupPermissions)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (group == null)
            {
                return NotFound();
            }

            group.Name = updateGroupDto.Name;
            group.Description = updateGroupDto.Description;

            // Update permissions
            _context.GroupPermissions.RemoveRange(group.GroupPermissions);
            
            if (updateGroupDto.PermissionIds.Any())
            {
                var groupPermissions = updateGroupDto.PermissionIds.Select(permissionId => new GroupPermission
                {
                    GroupId = group.Id,
                    PermissionId = permissionId,
                    GrantedBy = "P1234567"
                }).ToList();

                _context.GroupPermissions.AddRange(groupPermissions);
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            var group = await _context.Groups.FindAsync(id);
            if (group == null)
            {
                return NotFound();
            }

            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("environments")]
        public ActionResult<IEnumerable<string>> GetEnvironments()
        {
            var environments = new[] { "QA", "UAT", "PROD" };
            return Ok(environments);
        }

        [HttpGet("by-user/{userId}")]
        public async Task<ActionResult<IEnumerable<GroupDto>>> GetGroupsByUser(string userId, [FromQuery] string? environment = null)
        {
            var query = _context.UserGroups
                .Include(ug => ug.Group)
                    .ThenInclude(g => g.GroupPermissions)
                        .ThenInclude(gp => gp.Permission)
                .Include(ug => ug.Group)
                    .ThenInclude(g => g.UserGroups)
                        .ThenInclude(ug2 => ug2.User)
                .Where(ug => ug.UserId == userId)
                .AsQueryable();

            if (!string.IsNullOrEmpty(environment))
            {
                query = query.Where(ug => ug.Group.Environment == environment);
            }

            var userGroups = await query.ToListAsync();

            var groupDtos = userGroups.Select(ug => new GroupDto
            {
                Id = ug.Group.Id,
                Name = ug.Group.Name,
                Description = ug.Group.Description,
                Environment = ug.Group.Environment,
                CreatedAt = ug.Group.CreatedAt,
                CreatedBy = ug.Group.CreatedBy,
                UserCount = ug.Group.UserGroups.Count,
                Users = ug.Group.UserGroups.Select(ug2 => new SimpleUserDto
                {
                    Id = ug2.User.Id,
                    Name = ug2.User.Name,
                    Email = ug2.User.Email,
                    CurrentEnvironment = ug2.User.CurrentEnvironment
                }).ToList(),
                Permissions = ug.Group.GroupPermissions.Select(gp => new PermissionDto
                {
                    Id = gp.Permission.Id,
                    Name = gp.Permission.Name,
                    Description = gp.Permission.Description,
                    Category = gp.Permission.Category,
                    CreatedAt = gp.Permission.CreatedAt
                }).ToList()
            }).ToList();

            return Ok(groupDtos);
        }
    }
} 