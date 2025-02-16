namespace GamaEdtech.Backend.Common.Service
{
    using System;

    using GamaEdtech.Backend.Common.DataAccess.UnitOfWork;

    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;

    public abstract class ServiceBase<T>(Lazy<IUnitOfWorkProvider> unitOfWorkProvider, Lazy<IHttpContextAccessor> httpContextAccessor, Lazy<ILogger<T>> logger)
        where T : class
    {
        protected Lazy<ILogger<T>> Logger { get; } = logger;

        protected Lazy<IHttpContextAccessor> HttpContextAccessor { get; } = httpContextAccessor;

        protected Lazy<IUnitOfWorkProvider> UnitOfWorkProvider { get; } = unitOfWorkProvider;
    }
}
