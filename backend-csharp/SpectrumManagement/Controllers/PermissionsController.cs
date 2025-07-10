using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpectrumManagement.Data;
using SpectrumManagement.DTOs;
using SpectrumManagement.Models;

namespace SpectrumManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PermissionsController : ControllerBase
    {
        private readonly SpectrumDbContext _context;

        public PermissionsController(SpectrumDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PermissionDto>>> GetPermissions([FromQuery] string? category = null)
        {
            var query = _context.Permissions.AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category == category);
            }

            var permissions = await query.ToListAsync();

            var permissionDtos = permissions.Select(p => new PermissionDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Category = p.Category,
                CreatedAt = p.CreatedAt
            }).ToList();

            return Ok(permissionDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PermissionDto>> GetPermission(int id)
        {
            var permission = await _context.Permissions.FindAsync(id);

            if (permission == null)
            {
                return NotFound();
            }

            var permissionDto = new PermissionDto
            {
                Id = permission.Id,
                Name = permission.Name,
                Description = permission.Description,
                Category = permission.Category,
                CreatedAt = permission.CreatedAt
            };

            return Ok(permissionDto);
        }

        [HttpPost]
        public async Task<ActionResult<PermissionDto>> CreatePermission(CreatePermissionDto createPermissionDto)
        {
            var permission = new Permission
            {
                Name = createPermissionDto.Name,
                Description = createPermissionDto.Description,
                Category = createPermissionDto.Category
            };

            _context.Permissions.Add(permission);
            await _context.SaveChangesAsync();

            var permissionDto = new PermissionDto
            {
                Id = permission.Id,
                Name = permission.Name,
                Description = permission.Description,
                Category = permission.Category,
                CreatedAt = permission.CreatedAt
            };

            return CreatedAtAction(nameof(GetPermission), new { id = permission.Id }, permissionDto);
        }

        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<string>>> GetCategories()
        {
            var categories = await _context.Permissions
                .Select(p => p.Category)
                .Distinct()
                .ToListAsync();

            return Ok(categories);
        }
    }
} 