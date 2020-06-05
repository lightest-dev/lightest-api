using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;
using Lightest.Data.Seeding.Interfaces;
using Lightest.Data.Seeding.Properties;
using Microsoft.AspNetCore.Identity;

namespace Lightest.Data.Seeding
{
    public class DefaultSeeder : ISeeder
    {
        public DefaultSeeder(RelationalDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            UserManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            RoleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        }

        protected RelationalDbContext Context { get; set; }

        protected UserManager<ApplicationUser> UserManager { get; set; }

        protected RoleManager<IdentityRole> RoleManager { get; set; }

        public async Task Seed()
        {
            await AddLanguages();

            await AddRequiredRoles();
            await AddUsers();

            await Context.SaveChangesAsync();
        }

        protected async Task AddLanguages()
        {
            Context.Languages.Add(new Language
            {
                Id = Guid.NewGuid(),
                Extension = "cpp",
                Name = "C++"
            });
            Context.Languages.Add(new Language
            {
                Id = Guid.NewGuid(),
                Extension = "c",
                Name = "C"
            });
            Context.Languages.Add(new Language
            {
                Id = Guid.NewGuid(),
                Extension = "py",
                Name = "Python3"
            });
            Context.Languages.Add(new Language
            {
                Id = Guid.NewGuid(),
                Extension = "Pascal",
                Name = "pas"
            });
            await Context.SaveChangesAsync();
        }

        protected async Task AddUsers()
        {
            var student = new ApplicationUser
            {
                UserName = "student"
            };
            await UserManager.CreateAsync(student, "Password12$");
            var teacher = new ApplicationUser
            {
                UserName = "teacher"
            };
            await UserManager.CreateAsync(teacher, "Password12$");
            var admin = new ApplicationUser
            {
                UserName = "admin"
            };
            await UserManager.CreateAsync(admin, "Password12$");
            await UserManager.AddToRoleAsync(admin, "Admin");
            await UserManager.AddToRoleAsync(teacher, "Teacher");
        }

        protected async Task AddRequiredRoles()
        {
            await RoleManager.CreateAsync(new IdentityRole("Admin"));
            await RoleManager.CreateAsync(new IdentityRole("Teacher"));
        }

        public async Task AddTestData()
        {
            await AddSampleCheckers();
            await AddSampleCategories();
            await AddSampleTasks();
            await AssignTasks();
        }

        protected async Task AddSampleCategories()
        {
            var publicCategory = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Sample public category",
                Public = true
            };
            var privateCategory = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Sample public category"
            };
            Context.Categories.Add(publicCategory);
            Context.Categories.Add(privateCategory);
            await Context.SaveChangesAsync();
        }

        protected async Task AddSampleCheckers()
        {
            Context.Checkers.Add(new Checker
            {
                Id = Guid.NewGuid(),
                Name = "Integer sequence",
                Code = Resources.IntSequenceChecker
            });

            Context.Checkers.Add(new Checker
            {
                Id = Guid.NewGuid(),
                Name = "Double sequence",
                Code = Resources.DoubleSequenceChecker
            });

            Context.Checkers.Add(new Checker
            {
                Id = Guid.NewGuid(),
                Name = "Text (line by line comparer)",
                Code = Resources.LineSequenceChecker
            });

            await Context.SaveChangesAsync();
        }

        protected async Task AddSampleTasks()
        {
            var checker = Context.Checkers.First(c => c.Name == "Integer sequence");
            var privateCategory = Context.Categories.First(c => !c.Public);
            var publicCategory = Context.Categories.First(c => c.Public);
            var privateTask = new TaskDefinition
            {
                Id = Guid.NewGuid(),
                Description = "You are given 2 numbers separated by new line. Print the sum and the difference of numbers.",
                Name = "Sum and difference",
                Points = 100,
                Category = privateCategory,
                CategoryId = privateCategory.Id,
                Tests = new List<Test>
                {
                    new Test
                    {
                        Id = Guid.NewGuid(),
                        Input = "4\n3",
                        Output = "7\n-1"
                    },
                    new Test
                    {
                        Id = Guid.NewGuid(),
                        Input = "2\n6",
                        Output = "8\n4"
                    }
                },
                Languages = new List<TaskLanguage>(),
                CheckerId = checker.Id,
                Checker = checker
            };
            var publicTask = new TaskDefinition
            {
                Id = Guid.NewGuid(),
                Description = @"You are given 2 numbers separated by new line. Print the sum and the difference of numbers.",
                Name = "Sum and difference",
                Points = 100,
                Public = true,
                Category = publicCategory,
                CategoryId = publicCategory.Id,
                Tests = new List<Test>
                {
                    new Test
                    {
                        Id = Guid.NewGuid(),
                        Input = "4\n3",
                        Output = "7\n-1"
                    },
                    new Test
                    {
                        Id = Guid.NewGuid(),
                        Input = "2\n6",
                        Output = "8\n4"
                    }
                },
                Languages = new List<TaskLanguage>(),
                CheckerId = checker.Id,
                Checker = checker
            };

            foreach (var language in Context.Languages)
            {
                privateTask.Languages.Add(new TaskLanguage
                {
                    Language = language,
                    LanguageId = language.Id,
                    Task = privateTask,
                    TaskId = privateTask.Id,
                    MemoryLimit = 512,
                    TimeLimit = 500
                });
                publicTask.Languages.Add(new TaskLanguage
                {
                    Language = language,
                    LanguageId = language.Id,
                    Task = privateTask,
                    TaskId = privateTask.Id,
                    MemoryLimit = 512,
                    TimeLimit = 500
                });
            }

            Context.Tasks.Add(publicTask);
            Context.Tasks.Add(privateTask);

            await Context.SaveChangesAsync();
        }

        protected async Task AssignTasks()
        {
            var student = await UserManager.FindByNameAsync("student");
            var privateTask = Context.Tasks.First(t => t.Public);
            privateTask.Users = new List<Assignment>
            {
                new Assignment
                {
                    Task = privateTask,
                    TaskId = privateTask.Id,
                    User = student,
                    UserId = student.Id,
                    CanRead = true
                }
            };

            await Context.SaveChangesAsync();
        }
    }
}
