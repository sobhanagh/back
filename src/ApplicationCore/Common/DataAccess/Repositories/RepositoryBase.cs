namespace GamaEdtech.Backend.Common.DataAccess.Repositories
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    public abstract class RepositoryBase<TContext>(ILogger<DataAccess> logger, TContext context) : IRepositoryInjection<TContext>
        where TContext : DbContext
    {
        protected ILogger Logger { get; } = logger;

        protected TContext Context { get; private set; } = context;

        public IRepositoryInjection<TContext> SetContext(TContext context)
        {
            Context = context;
            return this;
        }
    }
}
