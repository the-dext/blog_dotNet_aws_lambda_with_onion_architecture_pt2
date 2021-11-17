using System;
using LambdaLogger;
using LambdaLogger.Setup;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ProductCatalogue.Application.Setup;

namespace MultipleLambdas
{
    /// <summary>
    /// Base class for functions to centralise the ioc and mediatr setup
    /// </summary>
    public class FunctionBase
    {
        private readonly IServiceCollection _serviceCollection;
        private readonly ServiceProvider _serviceProvider;
        private readonly Lazy<IMediator> _mediatr;
        private readonly Lazy<ILogger> _logger;

        public FunctionBase()
        {
            this._serviceCollection = new ServiceCollection()
                .AddApplicationServices()
                .AddLoggingService();
            this._serviceProvider = this._serviceCollection.BuildServiceProvider();

            this._mediatr = new Lazy<IMediator>(() => this._serviceProvider.GetRequiredService<IMediator>());
            this._logger = new Lazy<ILogger>(() => this._serviceProvider.GetRequiredService<ILogger>());
        }

        protected ILogger Logger => this._logger.Value;
        protected IMediator Mediator => this._mediatr.Value;
    }
}
