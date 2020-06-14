using System;
using System.Net;
using Lightest.CodeManagment.Models;
using Lightest.Data;
using Lightest.Data.CodeManagment.Services;
using Lightest.Data.Models.TaskModels;
using Lightest.TestingService.Interfaces;
using Lightest.TestingService.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace Lightest.Tests.TestingService.TestingRunner
{
    public abstract class BaseTest : TestingService.BaseTest
    {
        protected Lightest.TestingService.DefaultServices.TestingRunner _testingRunner =>
            new Lightest.TestingService.DefaultServices.TestingRunner(_scopeFactory.Object,
                _transferServiceFactory.Object, _uploadProcessorFactory.Object, _logger.Object);

        protected readonly Mock<IServiceScopeFactory> _scopeFactory;
        protected readonly Mock<IServiceScope> _scope;
        protected readonly Mock<IServiceProvider> _serviceProvider;

        protected readonly Mock<ITransferServiceFactory> _transferServiceFactory;
        protected readonly Mock<ITransferService> _transferService;

        protected readonly Mock<IUploadProcessorFactory> _uploadProcessorFactory;
        protected readonly Mock<IUploadProcessor> _uploadProcessor;

        protected readonly Mock<ILogger<Lightest.TestingService.DefaultServices.TestingRunner>> _logger;
        protected readonly Mock<ICodeManagmentService> _codeManagmentService;

        public BaseTest()
        {
            _codeManagmentService = new Mock<ICodeManagmentService>();
            _logger = new Mock<ILogger<Lightest.TestingService.DefaultServices.TestingRunner>>();

            _uploadProcessor = new Mock<IUploadProcessor>();
            _uploadProcessorFactory = new Mock<IUploadProcessorFactory>();
            _uploadProcessorFactory.Setup(f => f.Create(It.IsAny<Upload>(), It.IsAny<UploadData>(),
                It.IsAny<TestingServer>(), _context))
                .Returns(_uploadProcessor.Object);

            _transferService = new Mock<ITransferService>();
            _transferServiceFactory = new Mock<ITransferServiceFactory>();
            _transferServiceFactory.Setup(f => f.Create(It.IsAny<IPAddress>(), It.IsAny<int>()))
                .Returns(_transferService.Object);

            _serviceProvider = new Mock<IServiceProvider>();
            _serviceProvider.Setup(p => p.GetService(typeof(RelationalDbContext)))
                .Returns(_context);
            _serviceProvider.Setup(p => p.GetService(typeof(ICodeManagmentService)))
                .Returns(_codeManagmentService.Object);

            _scope = new Mock<IServiceScope>();
            _scope.Setup(s => s.ServiceProvider)
                .Returns(_serviceProvider.Object);

            _scopeFactory = new Mock<IServiceScopeFactory>();
            _scopeFactory.Setup(f => f.CreateScope())
                .Returns(_scope.Object);
        }

    }
}
