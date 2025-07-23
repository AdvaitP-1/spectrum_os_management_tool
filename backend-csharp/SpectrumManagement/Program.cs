using Microsoft.EntityFrameworkCore;
using CharterAccess.Data;
using CharterAccess.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var useInMemoryDb = builder.Configuration.GetValue<bool>("UseInMemoryDatabase", false);

if (useInMemoryDb || string.IsNullOrEmpty(connectionString))
{
            builder.Services.AddDbContext<CharterAccessDbContext>(options =>
            options.UseInMemoryDatabase("CharterAccessInMemory"));
    Console.WriteLine("Using in-memory database for development");
}
else
{
            builder.Services.AddDbContext<CharterAccessDbContext>(options =>
            options.UseSqlite(connectionString));
    Console.WriteLine($"Using SQLite database: {connectionString}");
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            "http://localhost:4200", 
            "http://localhost:4201", 
            "http://localhost:3000",
            "http://127.0.0.1:4200",
            "http://127.0.0.1:4201",
            "http://127.0.0.1:3000"
        )
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
            var context = scope.ServiceProvider.GetRequiredService<CharterAccessDbContext>();
    
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
        
        // Add some demo users
        if (!context.Users.Any())
        {
            var users = new List<User>
            {
                new User { Id = "P1234567", Name = "Patrick Dugan", Email = "patrick.dugan@charter.com", CurrentEnvironment = "QA" },
                new User { Id = "P2345678", Name = "Kent Herbst", Email = "kent.herbst@charter.com", CurrentEnvironment = "QA" },
                new User { Id = "P3456789", Name = "Sion Pixley", Email = "sion.pixley@charter.com", CurrentEnvironment = "QA" },
                new User { Id = "P4567890", Name = "Boyuan Bruce Sun", Email = "boyuan.sun@charter.com", CurrentEnvironment = "QA" },
                new User { Id = "P5678901", Name = "Maria Alvarez Anticona", Email = "maria.alvarez@charter.com", CurrentEnvironment = "QA" },
                new User { Id = "P6789012", Name = "Joel Black", Email = "joel.black@charter.com", CurrentEnvironment = "QA" },
                new User { Id = "P7890123", Name = "Swetha Priya Yarlagadda", Email = "swetha.yarlagadda@charter.com", CurrentEnvironment = "QA" },
                new User { Id = "P8901234", Name = "Sheldon Skaggs", Email = "sheldon.skaggs@charter.com", CurrentEnvironment = "QA" },
                new User { Id = "P9012345", Name = "Jason Routh", Email = "jason.routh@charter.com", CurrentEnvironment = "QA" },
                new User { Id = "P1011121", Name = "Advait Pandey", Email = "advait.pandey@charter.com", CurrentEnvironment = "QA" }
            };
            
            context.Users.AddRange(users);
            context.SaveChanges();
            
            // Add some user-group relationships
            var userGroups = new List<UserGroup>
            {
                new UserGroup { UserId = "P1234567", GroupId = 1, AddedBy = "System" }, // Patrick in Database Admins
                new UserGroup { UserId = "P4567890", GroupId = 2, AddedBy = "System" }, // Boyuan in API Developers
                new UserGroup { UserId = "P7890123", GroupId = 2, AddedBy = "System" }, // Swetha in API Developers
                new UserGroup { UserId = "P1011121", GroupId = 2, AddedBy = "System" }, // Advait in API Developers
                new UserGroup { UserId = "P1011121", GroupId = 3, AddedBy = "System" }  // Advait also in System Operators
            };
            
            context.UserGroups.AddRange(userGroups);
            context.SaveChanges();
        }
    }
}

app.Run();
