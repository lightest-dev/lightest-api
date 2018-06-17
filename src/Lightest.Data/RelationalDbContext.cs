using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Lightest.Data
{
    public class RelationalDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Category> Categories { get; set; }

        public DbSet<Group> Groups { get; set; }

        public DbSet<Language> Languages { get; set; }

        public DbSet<TaskDefinition> Tasks { get; set; }

        public DbSet<Test> Tests { get; set; }

        public DbSet<Checker> Checkers { get; set; }

        public RelationalDbContext(DbContextOptions<RelationalDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //todo: Configure ignore fields for user
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>()
                .Ignore(u => u.PhoneNumber)
                .Ignore(u => u.PhoneNumberConfirmed)
                .Ignore(u => u.LockoutEnabled)
                .Ignore(u => u.LockoutEnd)
                .Ignore(u => u.AccessFailedCount);

            builder.Entity<CategoryUser>()
                .HasKey(cu => new { cu.CategoryId, cu.UserId });
            builder.Entity<CategoryUser>()
                .HasOne(cu => cu.User)
                .WithMany(u => u.AvailableCategories)
                .HasForeignKey(cu => cu.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<CategoryUser>()
                .HasOne(cu => cu.Category)
                .WithMany(c => c.Users)
                .HasForeignKey(cu => cu.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserGroup>()
                .HasKey(ug => new { ug.GroupId, ug.UserId });
            builder.Entity<UserGroup>()
                .HasOne(ug => ug.User)
                .WithMany(u => u.Groups)
                .HasForeignKey(ug => ug.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<UserGroup>()
                .HasOne(ug => ug.Group)
                .WithMany(g => g.Users)
                .HasForeignKey(ug => ug.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<TaskLanguage>()
                .HasKey(tl => new { tl.LanguageId, tl.TaskId });
            builder.Entity<TaskLanguage>()
                .HasOne(tl => tl.Task)
                .WithMany(t => t.Languages)
                .HasForeignKey(tl => tl.TaskId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<TaskLanguage>()
                .HasOne(tl => tl.Language)
                .WithMany(l => l.Tasks)
                .HasForeignKey(tl => tl.LanguageId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserTask>()
                .HasKey(ut => new { ut.TaskId, ut.UserId });
            builder.Entity<UserTask>()
                .HasOne(ut => ut.User)
                .WithMany(u => u.Tasks)
                .HasForeignKey(ut => ut.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<UserTask>()
                .HasOne(ut => ut.Task)
                .WithMany(t => t.Users)
                .HasForeignKey(ut => ut.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Category>()
                .HasOne(c => c.Parent)
                .WithMany(p => p.SubCategories)
                .HasForeignKey(c => c.ParentId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Group>()
                .HasOne(g => g.Parent)
                .WithMany(p => p.SubGroups)
                .HasForeignKey(g => g.ParentId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            builder.Entity<TaskDefinition>()
                .HasOne(t => t.Category)
                .WithMany(c => c.Tasks)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TaskDefinition>()
                .HasOne(t => t.Checker)
                .WithMany(c => c.Tasks)
                .HasForeignKey(t => t.CheckerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Test>()
                .HasOne(t => t.Task)
                .WithMany(t => t.Tests)
                .HasForeignKey(t => t.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ArchiveUpload>()
                .HasOne(up => up.Task)
                .WithMany(t => t.ArchiveUploads)
                .HasForeignKey(up => up.TaskId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<ArchiveUpload>()
                .HasOne(up => up.User)
                .WithMany(u => u.ArchiveUploads)
                .HasForeignKey(up => up.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ArchiveUpload>()
                .HasOne(up => up.Language)
                .WithMany(l => l.ArchiveUploads)
                .HasForeignKey(up => up.LanguageId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<CodeUpload>()
                .HasOne(up => up.Task)
                .WithMany(t => t.CodeUploads)
                .HasForeignKey(up => up.TaskId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<CodeUpload>()
                .HasOne(up => up.User)
                .WithMany(u => u.CodeUploads)
                .HasForeignKey(up => up.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CodeUpload>()
                .HasOne(up => up.Language)
                .WithMany(l => l.CodeUploads)
                .HasForeignKey(up => up.LanguageId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}