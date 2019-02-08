using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Lightest.TestingService.DefaultServices;
using Lightest.TestingService.Requests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Lightest.TransferTest
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            var db = new MockDatabase();
            MockDatabase.Fill(db.Context);
#pragma warning disable CS0618 // Type or member is obsolete
            var logFactory = new LoggerFactory()
                .AddConsole(LogLevel.Debug);
#pragma warning restore CS0618 // Type or member is obsolete
            await CompleteTest(db, logFactory);
            // await MessageTest(db);
            // await FileTest(db);
        }

        private static async Task CompleteTest(MockDatabase db, ILoggerFactory logFactory)
        {
            var factory = new TransferServiceFactory(logFactory);
            var repo = new ServerRepository(db.Context);
            // todo: fix creation
            // repo.AddFreeServer(IPAddress.Loopback);
            var testingService = new TestingService.DefaultServices.TestingService(repo, db.Context, factory);
            var upload = db.Context.CodeUploads
                .Include(u => u.Task)
                .ThenInclude(t => t.Languages)
                .Include(u => u.Task)
                .ThenInclude(t => t.Tests)
                .Include(u => u.Task)
                .ThenInclude(t => t.Checker)
                .Include(u => u.Language)
                .FirstOrDefault();
            await testingService.BeginTesting(upload);
        }

        private static async Task MessageTest(MockDatabase db, ILoggerFactory logFactory)
        {
            var upload = db.Context.CodeUploads
                .Include(u => u.Task)
                .ThenInclude(t => t.Languages)
                .Include(u => u.Task)
                .ThenInclude(t => t.Tests)
                .FirstOrDefault();

            var transferService = new TransferService(logFactory.CreateLogger(typeof(TransferService)),
                IPAddress.Loopback, 10000);
            var language = upload.Task.Languages.FirstOrDefault(l => l.LanguageId == upload.LanguageId);
            var request = new TestingRequest
            {
                UploadId = upload.UploadId,
                MemoryLimit = language.MemoryLimit,
                TimeLimit = language.TimeLimit,
                CheckerId = upload.Task.CheckerId,
                TestsCount = upload.Task.Tests.Count
            };
            var result = await transferService.SendMessage(request.ToString());
        }

        private static async Task FileTest(MockDatabase db, ILoggerFactory logFactory)
        {
            var upload = db.Context.CodeUploads.FirstOrDefault();
            var request = new SingleFileCodeRequest(upload.UploadId + ".cpp");
            var transferService = new TransferService(logFactory.CreateLogger(typeof(TransferService)),
                IPAddress.Loopback, 10000);
            await transferService.SendFile(request, Encoding.UTF8.GetBytes(upload.Code));
        }
    }
}
