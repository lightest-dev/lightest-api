using System;
using System.Collections.Generic;
using Lightest.Data;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;
using Microsoft.EntityFrameworkCore;

namespace Lightest.TransferTest
{
    public class MockDatabase
    {
        private readonly DbContextOptions<RelationalDbContext>
            _options = new DbContextOptionsBuilder<RelationalDbContext>()
            .UseInMemoryDatabase("Add_writes_to_database")
            .Options;

        public RelationalDbContext Context
        {
            get
            {
                var context = new RelationalDbContext(_options);
                return context;
            }
        }

        public static RelationalDbContext Fill(RelationalDbContext context)
        {
            var checker = new Checker
            {
                Id = new Guid()
            };
            var language = new Language
            {
                Id = new Guid(),
                Extension = "cpp",
                Name = "C++"
            };
            var task = new TaskDefinition
            {
                Id = new Guid(),
                Tests = new List<Test>
                {
                    new Test
                    {
                        Id = new Guid(),
                        Input = "4\n3",
                        Output = "7\n-1"
                    },
                    new Test
                    {
                        Id = new Guid(),
                        Input = "2\n6",
                        Output = "8\n4"
                    }
                },
                Languages = new List<TaskLanguage>
                {
                    new TaskLanguage
                    {
                        MemoryLimit = 512,
                        TimeLimit = 500,
                        Language = language,
                        LanguageId = language.Id
                    }
                },
                CheckerId = checker.Id,
                Checker = checker
            };
            var user = new ApplicationUser
            {
                Id = new Guid().ToString()
            };
            var userTask = new UserTask
            {
                UserId = user.Id,
                TaskId = task.Id
            };
            var upload = new Upload
            {
                Language = language,
                LanguageId = language.Id,
                Task = task,
                TaskId = task.Id,
                Id = new Guid(),
                UserId = user.Id
            };
            context.Checkers.Add(checker);
            context.Languages.Add(language);
            context.Tasks.Add(task);
            context.Users.Add(user);
            context.UserTasks.Add(userTask);
            context.Uploads.Add(upload);
            context.SaveChanges();
            return context;
        }
    }
}
