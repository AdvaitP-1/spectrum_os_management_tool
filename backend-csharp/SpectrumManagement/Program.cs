using Microsoft.EntityFrameworkCore;
using SpectrumManagement.Data;
using SpectrumManagement.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var useInMemoryDb = builder.Configuration.GetValue<bool>("UseInMemoryDatabase", false);

if (useInMemoryDb || string.IsNullOrEmpty(connectionString))
{
    builder.Services.AddDbContext<SpectrumDbContext>(options =>
        options.UseInMemoryDatabase("SpectrumManagementInMemory"));
    Console.WriteLine("Using in-memory database for development");
}
else
{
    builder.Services.AddDbContext<SpectrumDbContext>(options =>
        options.UseSqlServer(connectionString));
    Console.WriteLine($"Using SQL Server database: {connectionString}");
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "http://localhost:4201", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
        
        var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();
        if (allowedOrigins?.Length > 0)
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SpectrumDbContext>();
    
    if (useInMemoryDb || string.IsNullOrEmpty(connectionString))
    {
        context.Database.EnsureCreated();
    }
    else
    {
        context.Database.EnsureCreated();
    }
    
    if (!context.Permissions.Any())
    {
        var permissions = new List<Permission>
        {
            new Permission { Name = "Read Database", Description = "View database records and schemas", Category = "Database" },
            new Permission { Name = "Write Database", Description = "Insert and update database records", Category = "Database" },
            new Permission { Name = "Delete Database", Description = "Delete database records", Category = "Database" },
            new Permission { Name = "Admin Database", Description = "Full database administration access", Category = "Database" },
            
            new Permission { Name = "Read API", Description = "Make GET requests to API endpoints", Category = "API" },
            new Permission { Name = "Write API", Description = "Make POST/PUT requests to API endpoints", Category = "API" },
            new Permission { Name = "Delete API", Description = "Make DELETE requests to API endpoints", Category = "API" },
            new Permission { Name = "Admin API", Description = "Full API access including system endpoints", Category = "API" },
            
            new Permission { Name = "Read System", Description = "View system configurations and logs", Category = "System" },
            new Permission { Name = "Write System", Description = "Modify system configurations", Category = "System" },
            new Permission { Name = "Admin System", Description = "Full system administration access", Category = "System" },
            
            new Permission { Name = "Read Users", Description = "View user accounts and profiles", Category = "User Management" },
            new Permission { Name = "Write Users", Description = "Create and modify user accounts", Category = "User Management" },
            new Permission { Name = "Delete Users", Description = "Delete user accounts", Category = "User Management" }
        };
        
        context.Permissions.AddRange(permissions);
        context.SaveChanges();
        
        var groups = new List<Group>
        {
            new Group 
            { 
                Name = "Database Admins", 
                Description = "Full database administration access", 
                Environment = "QA",
                CreatedBy = "System"
            },
            new Group 
            { 
                Name = "API Developers", 
                Description = "API development and testing access", 
                Environment = "QA",
                CreatedBy = "System"
            },
            new Group 
            { 
                Name = "System Operators", 
                Description = "System monitoring and basic operations", 
                Environment = "QA",
                CreatedBy = "System"
            },

        };
        
        context.Groups.AddRange(groups);
        context.SaveChanges();
        
        var dbAdminPerms = permissions.Where(p => p.Category == "Database" || p.Name.Contains("Admin")).ToList();
        var apiDevPerms = permissions.Where(p => p.Category == "API" || p.Category == "Database" && !p.Name.Contains("Delete")).ToList();
        var sysOpPerms = permissions.Where(p => p.Category == "System" && p.Name.Contains("Read")).ToList();
        var readOnlyPerms = permissions.Where(p => p.Name.Contains("Read")).ToList();
        
        var groupPermissions = new List<GroupPermission>();
        
        foreach (var group in groups.Where(g => g.Name == "Database Admins"))
        {
            foreach (var perm in dbAdminPerms)
            {
                groupPermissions.Add(new GroupPermission 
                { 
                    GroupId = group.Id, 
                    PermissionId = perm.Id,
                    GrantedBy = "System"
                });
            }
        }
        
        foreach (var group in groups.Where(g => g.Name == "API Developers"))
        {
            foreach (var perm in apiDevPerms)
            {
                groupPermissions.Add(new GroupPermission 
                { 
                    GroupId = group.Id, 
                    PermissionId = perm.Id,
                    GrantedBy = "System"
                });
            }
        }
        
        foreach (var group in groups.Where(g => g.Name == "System Operators"))
        {
            foreach (var perm in sysOpPerms)
            {
                groupPermissions.Add(new GroupPermission 
                { 
                    GroupId = group.Id, 
                    PermissionId = perm.Id,
                    GrantedBy = "System"
                });
            }
        }
        
        foreach (var group in groups.Where(g => g.Name == "Read Only Users"))
        {
            foreach (var perm in readOnlyPerms)
            {
                groupPermissions.Add(new GroupPermission 
                { 
                    GroupId = group.Id, 
                    PermissionId = perm.Id,
                    GrantedBy = "System"
                });
            }
        }
        
        context.GroupPermissions.AddRange(groupPermissions);
        context.SaveChanges();
    }
}

app.Run();
