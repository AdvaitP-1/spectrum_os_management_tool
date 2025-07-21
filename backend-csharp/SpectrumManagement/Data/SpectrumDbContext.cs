using Microsoft.EntityFrameworkCore;
using SpectrumManagement.Models;

namespace SpectrumManagement.Data
{
    public class SpectrumDbContext : DbContext
    {
        public SpectrumDbContext(DbContextOptions<SpectrumDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<GroupPermission> GroupPermissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            modelBuilder.Entity<Group>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.HasIndex(e => new { e.Name, e.Environment }).IsUnique();
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.HasIndex(e => e.Name).IsUnique();
            });

            modelBuilder.Entity<UserGroup>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                
                entity.HasOne(e => e.User)
                    .WithMany(e => e.UserGroups)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(e => e.Group)
                    .WithMany(e => e.UserGroups)
                    .HasForeignKey(e => e.GroupId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasIndex(e => new { e.UserId, e.GroupId }).IsUnique();
            });

            modelBuilder.Entity<GroupPermission>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                
                entity.HasOne(e => e.Group)
                    .WithMany(e => e.GroupPermissions)
                    .HasForeignKey(e => e.GroupId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(e => e.Permission)
                    .WithMany(e => e.GroupPermissions)
                    .HasForeignKey(e => e.PermissionId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasIndex(e => new { e.GroupId, e.PermissionId }).IsUnique();
            });

            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            var permissions = new List<Permission>
            {
                new Permission { Id = 1, Name = "Read Database", Description = "View database records", Category = "Database" },
                new Permission { Id = 2, Name = "Write Database", Description = "Create and update database records", Category = "Database" },
                new Permission { Id = 3, Name = "Delete Database", Description = "Delete database records", Category = "Database" },
                new Permission { Id = 4, Name = "Database Admin", Description = "Full database administration", Category = "Database" },
                
                new Permission { Id = 5, Name = "API Access", Description = "Access REST API endpoints", Category = "API" },
                new Permission { Id = 6, Name = "Admin API", Description = "Access administrative API functions", Category = "API" },
                new Permission { Id = 7, Name = "External API", Description = "Access external integrations", Category = "API" },
                
                new Permission { Id = 8, Name = "UI Access", Description = "Access user interface", Category = "UI" },
                new Permission { Id = 9, Name = "Dashboard Access", Description = "View system dashboard", Category = "UI" },
                new Permission { Id = 10, Name = "Analytics Access", Description = "Access analytics and insights", Category = "UI" },
                
                new Permission { Id = 11, Name = "View Reports", Description = "View system reports", Category = "Reports" },
                new Permission { Id = 12, Name = "Create Reports", Description = "Create custom reports", Category = "Reports" },
                new Permission { Id = 13, Name = "Export Reports", Description = "Export reports to various formats", Category = "Reports" },
                
                new Permission { Id = 14, Name = "User Management", Description = "Manage user accounts", Category = "Admin" },
                new Permission { Id = 15, Name = "Group Management", Description = "Manage groups and permissions", Category = "Admin" },
                new Permission { Id = 16, Name = "System Config", Description = "Configure system settings", Category = "Admin" },
                new Permission { Id = 17, Name = "Security Admin", Description = "Manage security settings", Category = "Admin" },
                
                new Permission { Id = 18, Name = "System Monitoring", Description = "Monitor system health", Category = "Monitoring" },
                new Permission { Id = 19, Name = "Log Access", Description = "Access system logs", Category = "Monitoring" },
                new Permission { Id = 20, Name = "Alert Management", Description = "Manage system alerts", Category = "Monitoring" }
            };

            modelBuilder.Entity<Permission>().HasData(permissions);

            var users = new List<User>
            {
                new User { Id = "P1234567", Name = "Patrick Dugan", Email = "patrick.dugan@spectrum.com" },
                new User { Id = "P2345678", Name = "Kent Herbst", Email = "kent.herbst@spectrum.com" },
                new User { Id = "P3456789", Name = "Sion Pixley", Email = "sion.pixley@spectrum.com" },
                new User { Id = "P4567890", Name = "Boyuan Bruce Sun", Email = "boyuan.sun@spectrum.com" },
                new User { Id = "P5678901", Name = "Maria Alvarez Anticona", Email = "maria.alvarez@spectrum.com" },
                new User { Id = "P6789012", Name = "Joel Black", Email = "joel.black@spectrum.com" },
                new User { Id = "P7890123", Name = "Swetha Priya Yarlagadda", Email = "swetha.yarlagadda@spectrum.com" },
                new User { Id = "P8901234", Name = "Sheldon Skaggs", Email = "sheldon.skaggs@spectrum.com" },
                new User { Id = "P9012345", Name = "Jason Routh", Email = "jason.routh@spectrum.com" },
                new User { Id = "P1011121", Name = "Advait Pandey", Email = "advait.pandey@spectrum.com" }
            };

            modelBuilder.Entity<User>().HasData(users);

            var groups = new List<Group>();
            var environments = new[] { "QA", "UAT" };
            var groupId = 1;

            foreach (var env in environments)
            {
                groups.AddRange(new[]
                {
                    new Group { Id = groupId++, Name = "Developers", Description = "Development team members with coding and deployment access", Environment = env, CreatedBy = "P1234567" },
                    new Group { Id = groupId++, Name = "Testers", Description = "Quality assurance team with testing and validation permissions", Environment = env, CreatedBy = "P1234567" },
                    new Group { Id = groupId++, Name = "Admins", Description = "System administrators with full access privileges", Environment = env, CreatedBy = "P1234567" },
                    new Group { Id = groupId++, Name = "Business Users", Description = "End users and business stakeholders with operational access", Environment = env, CreatedBy = "P1234567" },
                    new Group { Id = groupId++, Name = "Read Only", Description = "Users with read-only access to view system information", Environment = env, CreatedBy = "P1234567" },
                    new Group { Id = groupId++, Name = "Data Analysts", Description = "Users focused on data analysis and reporting", Environment = env, CreatedBy = "P1234567" }
                });
            }

            modelBuilder.Entity<Group>().HasData(groups);

            var userGroups = new List<UserGroup>
            {
                new UserGroup { Id = 1, UserId = "P1234567", GroupId = 3, AddedBy = "P1234567" },
                new UserGroup { Id = 2, UserId = "P4567890", GroupId = 1, AddedBy = "P1234567" },
                new UserGroup { Id = 3, UserId = "P7890123", GroupId = 2, AddedBy = "P1234567" },
                new UserGroup { Id = 4, UserId = "P1011121", GroupId = 1, AddedBy = "P1234567" },
                new UserGroup { Id = 5, UserId = "P1011121", GroupId = 6, AddedBy = "P1234567" },
                
                new UserGroup { Id = 6, UserId = "P2345678", GroupId = 9, AddedBy = "P1234567" },
                new UserGroup { Id = 7, UserId = "P5678901", GroupId = 10, AddedBy = "P1234567" },
                new UserGroup { Id = 8, UserId = "P8901234", GroupId = 8, AddedBy = "P1234567" },
                

            };

            modelBuilder.Entity<UserGroup>().HasData(userGroups);

            var groupPermissions = new List<GroupPermission>();
            var permissionId = 1;

            var adminPermissions = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
            var developerPermissions = new[] { 1, 2, 5, 8, 9, 11, 18, 19 };
            var testerPermissions = new[] { 1, 5, 8, 9, 11, 12, 18 };
            var businessUserPermissions = new[] { 1, 8, 9, 10, 11 };
            var readOnlyPermissions = new[] { 1, 8, 11 };
            var dataAnalystPermissions = new[] { 1, 8, 9, 10, 11, 12, 13 };
            foreach (var env in environments)
            {
                var envIndex = Array.IndexOf(environments, env);
                var baseGroupId = envIndex * 6 + 1;

                // Developers
                foreach (var perm in developerPermissions)
                {
                    groupPermissions.Add(new GroupPermission { Id = permissionId++, GroupId = baseGroupId, PermissionId = perm, GrantedBy = "P1234567" });
                }

                // Testers
                foreach (var perm in testerPermissions)
                {
                    groupPermissions.Add(new GroupPermission { Id = permissionId++, GroupId = baseGroupId + 1, PermissionId = perm, GrantedBy = "P1234567" });
                }

                // Admins
                foreach (var perm in adminPermissions)
                {
                    groupPermissions.Add(new GroupPermission { Id = permissionId++, GroupId = baseGroupId + 2, PermissionId = perm, GrantedBy = "P1234567" });
                }

                // Business Users
                foreach (var perm in businessUserPermissions)
                {
                    groupPermissions.Add(new GroupPermission { Id = permissionId++, GroupId = baseGroupId + 3, PermissionId = perm, GrantedBy = "P1234567" });
                }

                // Read Only
                foreach (var perm in readOnlyPermissions)
                {
                    groupPermissions.Add(new GroupPermission { Id = permissionId++, GroupId = baseGroupId + 4, PermissionId = perm, GrantedBy = "P1234567" });
                }

                // Data Analysts
                foreach (var perm in dataAnalystPermissions)
                {
                    groupPermissions.Add(new GroupPermission { Id = permissionId++, GroupId = baseGroupId + 5, PermissionId = perm, GrantedBy = "P1234567" });
                }
            }

            modelBuilder.Entity<GroupPermission>().HasData(groupPermissions);
        }
    }
} 