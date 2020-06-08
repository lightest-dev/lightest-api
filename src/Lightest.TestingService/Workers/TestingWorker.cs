using System;
using System.Threading;
using System.Threading.Tasks;
using Lightest.TestingService.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Lightest.TestingService.Workers
{
    public class TestingWorker : IHostedService, IDisposable
    {
        private readonly ILogger<TestingWorker> _logger;
        private readonly ITestingRunner _testingRunner;
        private Timer _statusUpdateTimer;
        private Timer _queueRunnerTimer;
        private bool _disposedValue;

        public TestingWorker(ILogger<TestingWorker> logger, ITestingRunner testingRunner)
        {
            _logger = logger;
            _testingRunner = testingRunner;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _statusUpdateTimer = new Timer(async (_) => await UpdateServerStatuses(), null, TimeSpan.Zero, TimeSpan.FromMinutes(3));
            _queueRunnerTimer = new Timer(async (_) => await TryStartNewRunner(), null, TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(30));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping backgroung testing jobs");
            _statusUpdateTimer?.Change(Timeout.Infinite, 0);
            _queueRunnerTimer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public Task TryStartNewRunner() => _testingRunner.TryStartNewWorker();

        public Task UpdateServerStatuses() => _testingRunner.UpdateServerStatuses();

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _statusUpdateTimer?.Dispose();
                    _queueRunnerTimer?.Dispose();
                }
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
