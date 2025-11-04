using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace UsersRoles.DAL.CodeFirst
{
    public class WebStructContext : IdentityDbContext<WebStructUser, WebStructRole, string>
    {
        public WebStructContext(DbContextOptions<WebStructContext> options) : base(options)
        {

        }

        public DbSet<WebStructPermission> Permissions { get; set; }
        public DbSet<WebStructRolePermission> RolePermissions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasDefaultSchema("users");

            builder.Entity<WebStructUser>().ToTable("AspNetUsers", "users");
            builder.Entity<WebStructRole>().ToTable("AspNetRoles", "users");
            builder.Entity<IdentityUserRole<string>>().ToTable("AspNetUserRoles", "users");
            builder.Entity<IdentityUserClaim<string>>().ToTable("AspNetUserClaims", "users");
            builder.Entity<IdentityUserLogin<string>>().ToTable("AspNetUserLogins", "users");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("AspNetRoleClaims", "users");
            builder.Entity<IdentityUserToken<string>>().ToTable("AspNetUserTokens", "users");

            base.OnModelCreating(builder);

            builder.Entity<WebStructPermission>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.HasIndex(p => p.Name).IsUnique();
                entity.Property(p => p.Name).IsRequired().HasMaxLength(100);
                entity.Property(p => p.Description).HasMaxLength(500);
            });

            builder.Entity<WebStructRolePermission>(entity =>
            {
                entity.HasKey(rp => rp.Id);

                entity.HasIndex(rp => new { rp.RoleId, rp.PermissionId }).IsUnique();

                entity.HasOne(rp => rp.Role)
                      .WithMany(r => r.RolePermissions)
                      .HasForeignKey(rp => rp.RoleId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(rp => rp.Permission)
                      .WithMany(p => p.RolePermissions)
                      .HasForeignKey(rp => rp.PermissionId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            SeedData(builder);
        }

        private void SeedData(ModelBuilder builder)
        {
            builder.Entity<WebStructRole>().HasData(
                new WebStructRole
                {
                    Id = "1",
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    Description = "Администратор системы"
                },
                new WebStructRole
                {
                    Id = "2",
                    Name = "User",
                    NormalizedName = "USER",
                    Description = "Обычный пользователь"
                },
                new WebStructRole
                {
                    Id = "3",
                    Name = "Manager",
                    NormalizedName = "MANAGER",
                    Description = "Менеджер"
                }
            );

            builder.Entity<WebStructPermission>().HasData(
                new WebStructPermission { Id = 1, Name = "Users.Read", Description = "Просмотр пользователей" },
                new WebStructPermission { Id = 2, Name = "Users.Create", Description = "Создание пользователей" },
                new WebStructPermission { Id = 3, Name = "Users.Update", Description = "Редактирование пользователей" },
                new WebStructPermission { Id = 4, Name = "Users.Delete", Description = "Удаление пользователей" },
                new WebStructPermission { Id = 5, Name = "Users.ChangePassword", Description = "Смена паролей пользователей" },

                new WebStructPermission { Id = 6, Name = "Roles.Read", Description = "Просмотр ролей" },
                new WebStructPermission { Id = 7, Name = "Roles.Create", Description = "Создание ролей" },
                new WebStructPermission { Id = 8, Name = "Roles.Update", Description = "Редактирование ролей", },
                new WebStructPermission { Id = 9, Name = "Roles.Delete", Description = "Удаление ролей" },
                new WebStructPermission { Id = 10, Name = "Roles.Assign", Description = "Назначение ролей пользователям" },

                new WebStructPermission { Id = 11, Name = "Permissions.Read", Description = "Просмотр полномочий" },
                new WebStructPermission { Id = 12, Name = "Permissions.Assign", Description = "Назначение полномочий ролям" },

                new WebStructPermission { Id = 13, Name = "Admin.Panel", Description = "Доступ к панели администратора" },

                new WebStructPermission { Id = 14, Name = "ModelsAlgorithms.Read", Description = "Просмотр списка алгоритмов моделей" },
                new WebStructPermission { Id = 15, Name = "ModelsAlgorithms.Create", Description = "Создание алгоритмов моделей" },
                new WebStructPermission { Id = 16, Name = "ModelsAlgorithms.Update", Description = "Редактирование алгоритмов моделей" },
                new WebStructPermission { Id = 17, Name = "ModelsAlgorithms.Delete", Description = "Удаление алгоритмов моделей" },

                new WebStructPermission { Id = 18, Name = "CompExperiments.Read", Description = "Просмотр всех вычислительных экспериментов" },
                new WebStructPermission { Id = 19, Name = "CompExperiments.Create", Description = "Запуск вычислительных экспериментов" }
            );

            // Seed связей ролей с полномочиями (Admin получает все полномочия)
            var rolePermissions = new List<WebStructRolePermission>();
            int rpId = 1;

            // Admin получает все полномочия
            for (int permissionId = 1; permissionId <= 19; permissionId++)
            {
                rolePermissions.Add(new WebStructRolePermission
                {
                    Id = rpId++,
                    RoleId = "1",
                    PermissionId = permissionId,
                    GrantedBy = Guid.Empty // System.
                });
            }

            builder.Entity<WebStructRolePermission>().HasData(rolePermissions);
        }
    }
}
