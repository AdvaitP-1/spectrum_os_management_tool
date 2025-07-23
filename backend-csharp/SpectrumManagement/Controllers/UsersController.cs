using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CharterAccess.Data;
using CharterAccess.DTOs;
using CharterAccess.Models;

namespace CharterAccess.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly CharterAccessDbContext _context;

        public UsersController(CharterAccessDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers([FromQuery] string? environment = null)
        {
            var query = _context.Users
                .Include(u => u.UserGroups)
                    .ThenInclude(ug => ug.Group)
                        .ThenInclude(g => g.GroupPermissions)
                            .ThenInclude(gp => gp.Permission)
                .AsQueryable();

            if (!string.IsNullOrEmpty(environment))
            {
                query = query.Where(u => u.CurrentEnvironment == environment);
            }

            var users = await query.ToListAsync();

            var userDtos = users.Select(u => new UserDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                CurrentEnvironment = u.CurrentEnvironment,
                CreatedAt = u.CreatedAt,
                LastLoginAt = u.LastLoginAt,
                Groups = u.UserGroups.Where(ug => ug.Group.Environment == u.CurrentEnvironment).Select(ug => new SimpleGroupDto
                {
                    Id = ug.Group.Id,
                    Name = ug.Group.Name,
                    Description = ug.Group.Description,
                    Environment = ug.Group.Environment,
                    CreatedAt = ug.Group.CreatedAt,
                    UserCount = ug.Group.UserGroups.Count
                }).ToList()
            }).ToList();

            return Ok(userDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(string id)
        {
            var user = await _context.Users
                .Include(u => u.UserGroups)
                    .ThenInclude(ug => ug.Group)
                        .ThenInclude(g => g.GroupPermissions)
                            .ThenInclude(gp => gp.Permission)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                CurrentEnvironment = user.CurrentEnvironment,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                Groups = user.UserGroups.Where(ug => ug.Group.Environment == user.CurrentEnvironment).Select(ug => new SimpleGroupDto
                {
                    Id = ug.Group.Id,
                    Name = ug.Group.Name,
                    Description = ug.Group.Description,
                    Environment = ug.Group.Environment,
                    CreatedAt = ug.Group.CreatedAt,
                    UserCount = ug.Group.UserGroups.Count
                }).ToList()
            };

            return Ok(userDto);
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto createUserDto)
        {
            var userId = GenerateUserId();

            var user = new User
            {
                Id = userId,
                Name = createUserDto.Name,
                Email = createUserDto.Email,
                CurrentEnvironment = createUserDto.CurrentEnvironment
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var userDto = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                CurrentEnvironment = user.CurrentEnvironment,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                Groups = new List<SimpleGroupDto>()
            };

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, userDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, UpdateUserDto updateUserDto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.Name = updateUserDto.Name;
            user.Email = updateUserDto.Email;
            user.CurrentEnvironment = updateUserDto.CurrentEnvironment;
            user.LastLoginAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{userId}/groups/{groupId}")]
        public async Task<IActionResult> JoinGroup(string userId, int groupId)
        {
            var user = await _context.Users.FindAsync(userId);
            var group = await _context.Groups.FindAsync(groupId);

            if (user == null || group == null)
            {
                return NotFound();
            }

            var existingMembership = await _context.UserGroups
                .FirstOrDefaultAsync(ug => ug.UserId == userId && ug.GroupId == groupId);

            if (existingMembership != null)
            {
                return Conflict("User is already a member of this group");
            }

            var userGroup = new UserGroup
            {
                UserId = userId,
                GroupId = groupId,
                AddedBy = userId
            };

            _context.UserGroups.Add(userGroup);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{userId}/groups/{groupId}")]
        public async Task<IActionResult> LeaveGroup(string userId, int groupId)
        {
            var userGroup = await _context.UserGroups
                .FirstOrDefaultAsync(ug => ug.UserId == userId && ug.GroupId == groupId);

            if (userGroup == null)
            {
                return NotFound();
            }

            _context.UserGroups.Remove(userGroup);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login([FromBody] string userId)
        {
            var user = await _context.Users
                .Include(u => u.UserGroups)
                    .ThenInclude(ug => ug.Group)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            user.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var userDto = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                CurrentEnvironment = user.CurrentEnvironment,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                Groups = user.UserGroups.Select(ug => new SimpleGroupDto
                {
                    Id = ug.Group.Id,
                    Name = ug.Group.Name,
                    Description = ug.Group.Description,
                    Environment = ug.Group.Environment,
                    CreatedAt = ug.Group.CreatedAt,
                    UserCount = ug.Group.UserGroups.Count
                }).ToList()
            };

            return Ok(userDto);
        }

        private string GenerateUserId()
        {
            var random = new Random();
            string userId;
            
            do
            {
                var numbers = string.Join("", Enumerable.Range(0, 7).Select(_ => random.Next(0, 10)));
                userId = $"P{numbers}";
            }
            while (_context.Users.Any(u => u.Id == userId));

            return userId;
        }
    }
} 