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
                Id = new Guid(),
                Code = @"#include ""testlib.h""

int main(int argc, char *argv[])
{
    registerTestlibCmd(argc, argv);
    int pans1 = ouf.readInt(-2000, 2000, ""sum of numbers"");
    int jans1 = ans.readInt();

    int pans2 = ouf.readInt(-2000, 2000, ""sum of numbers"");
    int jans2 = ans.readInt();

    if (pans1 == jans1 && pans2 == jans2)
        quitf(_ok, ""The sum is correct."");
    else
        quitf(_wa, ""The sum is wrong"");
}"
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
            var upload = new CodeUpload
            {
                Language = language,
                LanguageId = language.Id,
                Task = task,
                TaskId = task.Id,
                UploadId = new Guid(),
                UserId = user.Id,
                Code = @"#include <iostream>

using namespace std;

int main()
{
    long long a, b;
    cin >> a >> b;
    cout << a + b << endl;
    cout << b - a;

    return 0;
}"
            };
            context.Checkers.Add(checker);
            context.Languages.Add(language);
            context.Tasks.Add(task);
            context.Users.Add(user);
            context.UserTasks.Add(userTask);
            context.CodeUploads.Add(upload);
            context.SaveChanges();
            return context;
        }
    }
}
