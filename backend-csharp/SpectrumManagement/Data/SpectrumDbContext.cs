using Microsoft.EntityFrameworkCore;
using CharterAccess.Models;

namespace CharterAccess.Data
{
    public class CharterAccessDbContext : DbContext
    {
        public CharterAccessDbContext(DbContextOptions<CharterAccessDbContext> options) : base(options)
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
                            new User { Id = "P1234567", Name = "Patrick Dugan", Email = "patrick.dugan@charter.com" },
            new User { Id = "P2345678", Name = "Kent Herbst", Email = "kent.herbst@charter.com" },
            new User { Id = "P3456789", Name = "Sion Pixley", Email = "sion.pixley@charter.com" },
            new User { Id = "P4567890", Name = "Boyuan Bruce Sun", Email = "boyuan.sun@charter.com" },
            new User { Id = "P5678901", Name = "Maria Alvarez Anticona", Email = "maria.alvarez@charter.com" },
            new User { Id = "P6789012", Name = "Joel Black", Email = "joel.black@charter.com" },
            new User { Id = "P7890123", Name = "Swetha Priya Yarlagadda", Email = "swetha.yarlagadda@charter.com" },
            new User { Id = "P8901234", Name = "Sheldon Skaggs", Email = "sheldon.skaggs@charter.com" },
            new User { Id = "P9012345", Name = "Jason Routh", Email = "jason.routh@charter.com" },
            new User { Id = "P1011121", Name = "Advait Pandey", Email = "advait.pandey@charter.com" }
            };

            modelBuilder.Entity<User>().HasData(users);

            var groups = new List<Group>
            {
                // QA Environment Groups
                new Group { Id = 1, Name = "Developers", Description = "Development team members with coding and deployment access", Environment = "QA", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), CreatedBy = "P1234567" },
                new Group { Id = 2, Name = "Testers", Description = "Quality assurance team with testing and validation permissions", Environment = "QA", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), CreatedBy = "P1234567" },
                new Group { Id = 3, Name = "Admins", Description = "System administrators with full access privileges", Environment = "QA", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), CreatedBy = "P1234567" },
                new Group { Id = 4, Name = "Business Users", Description = "End users and business stakeholders with operational access", Environment = "QA", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), CreatedBy = "P1234567" },
                new Group { Id = 5, Name = "Read Only", Description = "Users with read-only access to view system information", Environment = "QA", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), CreatedBy = "P1234567" },
                new Group { Id = 6, Name = "Data Analysts", Description = "Users focused on data analysis and reporting", Environment = "QA", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), CreatedBy = "P1234567" },
                
                // UAT Environment Groups
                new Group { Id = 7, Name = "Developers", Description = "Development team members with coding and deployment access", Environment = "UAT", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), CreatedBy = "P1234567" },
                new Group { Id = 8, Name = "Testers", Description = "Quality assurance team with testing and validation permissions", Environment = "UAT", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), CreatedBy = "P1234567" },
                new Group { Id = 9, Name = "Admins", Description = "System administrators with full access privileges", Environment = "UAT", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), CreatedBy = "P1234567" },
                new Group { Id = 10, Name = "Business Users", Description = "End users and business stakeholders with operational access", Environment = "UAT", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), CreatedBy = "P1234567" },
                new Group { Id = 11, Name = "Read Only", Description = "Users with read-only access to view system information", Environment = "UAT", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), CreatedBy = "P1234567" },
                new Group { Id = 12, Name = "Data Analysts", Description = "Users focused on data analysis and reporting", Environment = "UAT", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), CreatedBy = "P1234567" }
            };

            modelBuilder.Entity<Group>().HasData(groups);

            var userGroups = new List<UserGroup>
            {
                new UserGroup { Id = 1, UserId = "P1234567", GroupId = 3, JoinedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), AddedBy = "P1234567" },
                new UserGroup { Id = 2, UserId = "P4567890", GroupId = 1, JoinedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), AddedBy = "P1234567" },
                new UserGroup { Id = 3, UserId = "P7890123", GroupId = 2, JoinedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), AddedBy = "P1234567" },
                new UserGroup { Id = 4, UserId = "P1011121", GroupId = 1, JoinedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), AddedBy = "P1234567" },
                new UserGroup { Id = 5, UserId = "P1011121", GroupId = 6, JoinedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), AddedBy = "P1234567" },
                
                new UserGroup { Id = 6, UserId = "P2345678", GroupId = 9, JoinedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), AddedBy = "P1234567" },
                new UserGroup { Id = 7, UserId = "P5678901", GroupId = 10, JoinedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), AddedBy = "P1234567" },
                new UserGroup { Id = 8, UserId = "P8901234", GroupId = 8, JoinedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), AddedBy = "P1234567" }
            };

            modelBuilder.Entity<UserGroup>().HasData(userGroups);

            var groupPermissions = new List<GroupPermission>
            {
                // QA Environment - Developers (Group 1)
                new GroupPermission { Id = 1, GroupId = 1, PermissionId = 1, GrantedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), GrantedBy = "P1234567" },
                new GroupPermission { Id = 2, GroupId = 1, PermissionId = 2, GrantedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), GrantedBy = "P1234567" },
                new GroupPermission { Id = 3, GroupId = 1, PermissionId = 5, GrantedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), GrantedBy = "P1234567" },
                new GroupPermission { Id = 4, GroupId = 1, PermissionId = 8, GrantedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), GrantedBy = "P1234567" },
                new GroupPermission { Id = 5, GroupId = 1, PermissionId = 9, GrantedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), GrantedBy = "P1234567" },
                new GroupPermission { Id = 6, GroupId = 1, PermissionId = 11, GrantedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), GrantedBy = "P1234567" },
                new GroupPermission { Id = 7, GroupId = 1, PermissionId = 18, GrantedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), GrantedBy = "P1234567" },
                new GroupPermission { Id = 8, GroupId = 1, PermissionId = 19, GrantedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), GrantedBy = "P1234567" },

                // QA Environment - Testers (Group 2)
                new GroupPermission { Id = 9, GroupId = 2, PermissionId = 1, GrantedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), GrantedBy = "P1234567" },
                new GroupPermission { Id = 10, GroupId = 2, PermissionId = 5, GrantedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), GrantedBy = "P1234567" },
                new GroupPermission { Id = 11, GroupId = 2, PermissionId = 8, GrantedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), GrantedBy = "P1234567" },
                new GroupPermission { Id = 12, GroupId = 2, PermissionId = 9, GrantedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), GrantedBy = "P1234567" },
                new GroupPermission { Id = 13, GroupId = 2, PermissionId = 11, GrantedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), GrantedBy = "P1234567" },
                new GroupPermission { Id = 14, GroupId = 2, PermissionId = 12, GrantedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), GrantedBy = "P1234567" },
                new GroupPermission { Id = 15, GroupId = 2, PermissionId = 18, GrantedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), GrantedBy = "P1234567" },

                // QA Environment - Admins (Group 3) - All permissions
                new GroupPermission { Id = 16, GroupId = 3, PermissionId = 1, GrantedBy = "P1234567" },
                new GroupPermission { Id = 17, GroupId = 3, PermissionId = 2, GrantedBy = "P1234567" },
                new GroupPermission { Id = 18, GroupId = 3, PermissionId = 3, GrantedBy = "P1234567" },
                new GroupPermission { Id = 19, GroupId = 3, PermissionId = 4, GrantedBy = "P1234567" },
                new GroupPermission { Id = 20, GroupId = 3, PermissionId = 5, GrantedBy = "P1234567" },
                new GroupPermission { Id = 21, GroupId = 3, PermissionId = 6, GrantedBy = "P1234567" },
                new GroupPermission { Id = 22, GroupId = 3, PermissionId = 7, GrantedBy = "P1234567" },
                new GroupPermission { Id = 23, GroupId = 3, PermissionId = 8, GrantedBy = "P1234567" },
                new GroupPermission { Id = 24, GroupId = 3, PermissionId = 9, GrantedBy = "P1234567" },
                new GroupPermission { Id = 25, GroupId = 3, PermissionId = 10, GrantedBy = "P1234567" },
                new GroupPermission { Id = 26, GroupId = 3, PermissionId = 11, GrantedBy = "P1234567" },
                new GroupPermission { Id = 27, GroupId = 3, PermissionId = 12, GrantedBy = "P1234567" },
                new GroupPermission { Id = 28, GroupId = 3, PermissionId = 13, GrantedBy = "P1234567" },
                new GroupPermission { Id = 29, GroupId = 3, PermissionId = 14, GrantedBy = "P1234567" },
                new GroupPermission { Id = 30, GroupId = 3, PermissionId = 15, GrantedBy = "P1234567" },
                new GroupPermission { Id = 31, GroupId = 3, PermissionId = 16, GrantedBy = "P1234567" },
                new GroupPermission { Id = 32, GroupId = 3, PermissionId = 17, GrantedBy = "P1234567" },
                new GroupPermission { Id = 33, GroupId = 3, PermissionId = 18, GrantedBy = "P1234567" },
                new GroupPermission { Id = 34, GroupId = 3, PermissionId = 19, GrantedBy = "P1234567" },
                new GroupPermission { Id = 35, GroupId = 3, PermissionId = 20, GrantedBy = "P1234567" },

                // QA Environment - Business Users (Group 4)
                new GroupPermission { Id = 36, GroupId = 4, PermissionId = 1, GrantedBy = "P1234567" },
                new GroupPermission { Id = 37, GroupId = 4, PermissionId = 8, GrantedBy = "P1234567" },
                new GroupPermission { Id = 38, GroupId = 4, PermissionId = 9, GrantedBy = "P1234567" },
                new GroupPermission { Id = 39, GroupId = 4, PermissionId = 10, GrantedBy = "P1234567" },
                new GroupPermission { Id = 40, GroupId = 4, PermissionId = 11, GrantedBy = "P1234567" },

                // QA Environment - Read Only (Group 5)
                new GroupPermission { Id = 41, GroupId = 5, PermissionId = 1, GrantedBy = "P1234567" },
                new GroupPermission { Id = 42, GroupId = 5, PermissionId = 8, GrantedBy = "P1234567" },
                new GroupPermission { Id = 43, GroupId = 5, PermissionId = 11, GrantedBy = "P1234567" },

                // QA Environment - Data Analysts (Group 6)
                new GroupPermission { Id = 44, GroupId = 6, PermissionId = 1, GrantedBy = "P1234567" },
                new GroupPermission { Id = 45, GroupId = 6, PermissionId = 8, GrantedBy = "P1234567" },
                new GroupPermission { Id = 46, GroupId = 6, PermissionId = 9, GrantedBy = "P1234567" },
                new GroupPermission { Id = 47, GroupId = 6, PermissionId = 10, GrantedBy = "P1234567" },
                new GroupPermission { Id = 48, GroupId = 6, PermissionId = 11, GrantedBy = "P1234567" },
                new GroupPermission { Id = 49, GroupId = 6, PermissionId = 12, GrantedBy = "P1234567" },
                new GroupPermission { Id = 50, GroupId = 6, PermissionId = 13, GrantedBy = "P1234567" },

                // UAT Environment - Developers (Group 7)
                new GroupPermission { Id = 51, GroupId = 7, PermissionId = 1, GrantedBy = "P1234567" },
                new GroupPermission { Id = 52, GroupId = 7, PermissionId = 2, GrantedBy = "P1234567" },
                new GroupPermission { Id = 53, GroupId = 7, PermissionId = 5, GrantedBy = "P1234567" },
                new GroupPermission { Id = 54, GroupId = 7, PermissionId = 8, GrantedBy = "P1234567" },
                new GroupPermission { Id = 55, GroupId = 7, PermissionId = 9, GrantedBy = "P1234567" },
                new GroupPermission { Id = 56, GroupId = 7, PermissionId = 11, GrantedBy = "P1234567" },
                new GroupPermission { Id = 57, GroupId = 7, PermissionId = 18, GrantedBy = "P1234567" },
                new GroupPermission { Id = 58, GroupId = 7, PermissionId = 19, GrantedBy = "P1234567" },

                // UAT Environment - Testers (Group 8)
                new GroupPermission { Id = 59, GroupId = 8, PermissionId = 1, GrantedBy = "P1234567" },
                new GroupPermission { Id = 60, GroupId = 8, PermissionId = 5, GrantedBy = "P1234567" },
                new GroupPermission { Id = 61, GroupId = 8, PermissionId = 8, GrantedBy = "P1234567" },
                new GroupPermission { Id = 62, GroupId = 8, PermissionId = 9, GrantedBy = "P1234567" },
                new GroupPermission { Id = 63, GroupId = 8, PermissionId = 11, GrantedBy = "P1234567" },
                new GroupPermission { Id = 64, GroupId = 8, PermissionId = 12, GrantedBy = "P1234567" },
                new GroupPermission { Id = 65, GroupId = 8, PermissionId = 18, GrantedBy = "P1234567" },

                // UAT Environment - Admins (Group 9) - All permissions
                new GroupPermission { Id = 66, GroupId = 9, PermissionId = 1, GrantedBy = "P1234567" },
                new GroupPermission { Id = 67, GroupId = 9, PermissionId = 2, GrantedBy = "P1234567" },
                new GroupPermission { Id = 68, GroupId = 9, PermissionId = 3, GrantedBy = "P1234567" },
                new GroupPermission { Id = 69, GroupId = 9, PermissionId = 4, GrantedBy = "P1234567" },
                new GroupPermission { Id = 70, GroupId = 9, PermissionId = 5, GrantedBy = "P1234567" },
                new GroupPermission { Id = 71, GroupId = 9, PermissionId = 6, GrantedBy = "P1234567" },
                new GroupPermission { Id = 72, GroupId = 9, PermissionId = 7, GrantedBy = "P1234567" },
                new GroupPermission { Id = 73, GroupId = 9, PermissionId = 8, GrantedBy = "P1234567" },
                new GroupPermission { Id = 74, GroupId = 9, PermissionId = 9, GrantedBy = "P1234567" },
                new GroupPermission { Id = 75, GroupId = 9, PermissionId = 10, GrantedBy = "P1234567" },
                new GroupPermission { Id = 76, GroupId = 9, PermissionId = 11, GrantedBy = "P1234567" },
                new GroupPermission { Id = 77, GroupId = 9, PermissionId = 12, GrantedBy = "P1234567" },
                new GroupPermission { Id = 78, GroupId = 9, PermissionId = 13, GrantedBy = "P1234567" },
                new GroupPermission { Id = 79, GroupId = 9, PermissionId = 14, GrantedBy = "P1234567" },
                new GroupPermission { Id = 80, GroupId = 9, PermissionId = 15, GrantedBy = "P1234567" },
                new GroupPermission { Id = 81, GroupId = 9, PermissionId = 16, GrantedBy = "P1234567" },
                new GroupPermission { Id = 82, GroupId = 9, PermissionId = 17, GrantedBy = "P1234567" },
                new GroupPermission { Id = 83, GroupId = 9, PermissionId = 18, GrantedBy = "P1234567" },
                new GroupPermission { Id = 84, GroupId = 9, PermissionId = 19, GrantedBy = "P1234567" },
                new GroupPermission { Id = 85, GroupId = 9, PermissionId = 20, GrantedBy = "P1234567" },

                // UAT Environment - Business Users (Group 10)
                new GroupPermission { Id = 86, GroupId = 10, PermissionId = 1, GrantedBy = "P1234567" },
                new GroupPermission { Id = 87, GroupId = 10, PermissionId = 8, GrantedBy = "P1234567" },
                new GroupPermission { Id = 88, GroupId = 10, PermissionId = 9, GrantedBy = "P1234567" },
                new GroupPermission { Id = 89, GroupId = 10, PermissionId = 10, GrantedBy = "P1234567" },
                new GroupPermission { Id = 90, GroupId = 10, PermissionId = 11, GrantedBy = "P1234567" },

                // UAT Environment - Read Only (Group 11)
                new GroupPermission { Id = 91, GroupId = 11, PermissionId = 1, GrantedBy = "P1234567" },
                new GroupPermission { Id = 92, GroupId = 11, PermissionId = 8, GrantedBy = "P1234567" },
                new GroupPermission { Id = 93, GroupId = 11, PermissionId = 11, GrantedBy = "P1234567" },

                // UAT Environment - Data Analysts (Group 12)
                new GroupPermission { Id = 94, GroupId = 12, PermissionId = 1, GrantedBy = "P1234567" },
                new GroupPermission { Id = 95, GroupId = 12, PermissionId = 8, GrantedBy = "P1234567" },
                new GroupPermission { Id = 96, GroupId = 12, PermissionId = 9, GrantedBy = "P1234567" },
                new GroupPermission { Id = 97, GroupId = 12, PermissionId = 10, GrantedBy = "P1234567" },
                new GroupPermission { Id = 98, GroupId = 12, PermissionId = 11, GrantedBy = "P1234567" },
                new GroupPermission { Id = 99, GroupId = 12, PermissionId = 12, GrantedBy = "P1234567" },
                new GroupPermission { Id = 100, GroupId = 12, PermissionId = 13, GrantedBy = "P1234567" }
            };

            modelBuilder.Entity<GroupPermission>().HasData(groupPermissions);
        }
    }
} 