using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Zetester.Data.Models;

namespace Zetester.Data
{
    public class RelationalDbContext: IdentityDbContext<ApplicationUser>
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

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

            builder.Entity<ApplicationUser>()
                .HasOne(u => u.Group)
                .WithMany(g => g.Users)
                .HasForeignKey(u => u.GroupId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Category>()
                .HasOne(c => c.Parent)
                .WithMany(p => p.SubCategories)
                .HasForeignKey(c => c.ParentId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Group>()
                .HasOne(g => g.Parent)
                .WithMany(p => p.SubGroups)
                .HasForeignKey(g => g.ParentId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Task>()
                .HasOne(t => t.Category)
                .WithMany(c => c.Tasks)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Test>()
                .HasOne(t => t.Task)
                .WithMany(t => t.Tests)
                .HasForeignKey(t => t.TaskId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
