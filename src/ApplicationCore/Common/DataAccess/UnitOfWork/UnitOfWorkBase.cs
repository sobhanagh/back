#pragma warning disable CA2100 // Review SQL queries for security vulnerabilities
namespace GamaEdtech.Backend.Common.DataAccess.UnitOfWork
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using GamaEdtech.Backend.Common.Core;
    using GamaEdtech.Backend.Common.DataAccess.Entities;
    using GamaEdtech.Backend.Common.DataAccess.Exceptions;
    using GamaEdtech.Backend.Common.DataAccess.Repositories;
    using GamaEdtech.Backend.Common.DataAnnotation.Schema;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public abstract class UnitOfWorkBase<TContext> : IUnitOfWorkBase
        where TContext : DbContext
    {
        protected internal UnitOfWorkBase(TContext context, IServiceProvider serviceProvider, ILogger<DataAccess> logger)
        {
            Context = context;
            ServiceProvider = serviceProvider;
            Logger = logger;
        }

        ~UnitOfWorkBase()
        {
            Dispose(false);
        }

        protected bool IsDisposed { get; set; }

        protected IServiceProvider ServiceProvider { get; }

        protected ILogger<DataAccess> Logger { get; }

        protected TContext Context { get; set; }

        public int SaveChanges()
        {
            CheckDisposed();
            Prepare();
            return Context.SaveChanges();
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            CheckDisposed();
            await PrepareAsync();

            return await Context.SaveChangesAsync(cancellationToken);
        }

        public IRepository<TEntity, long> GetRepository<TEntity>()
            where TEntity : class, IEntity<TEntity, long>
            => GetRepository<TEntity, long>();

        public IRepository<TEntity, TKey> GetRepository<TEntity, TKey>()
            where TEntity : class, IEntity<TEntity, TKey>
            where TKey : IEquatable<TKey>
        {
            CheckDisposed();
            var repository = ServiceProvider.GetService<IRepository<TEntity, TKey>>();
            if (repository is null)
            {
                var type = typeof(IRepository<TEntity, TKey>).Name;
                throw new RepositoryNotFoundException(type, $"Repository {type} not found in the IOC container. Check if it is registered during startup.");
            }

            _ = ((repository as IRepositoryInjection<TContext>)?.SetContext(Context));
            return repository;
        }

        public TRepository GetCustomRepository<TRepository>()
        {
            CheckDisposed();
            var repository = ServiceProvider.GetService<TRepository>();
            if (repository is null)
            {
                var type = typeof(TRepository).Name;
                throw new RepositoryNotFoundException(type, string.Format("Repository {0} not found in the IOC container. Check if it is registered during startup.", type));
            }

            _ = ((repository as IRepositoryInjection<TContext>)?.SetContext(Context));
            return repository;
        }

        public async Task<int> ExecuteSqlCommandAsync([NotNull] string sql, IEnumerable<(string ParameterName, object Value)>? param = null)
        {
            try
            {
                var connection = Context.Database.GetDbConnection();
                using var command = connection.CreateCommand();
                command!.CommandText = sql.Replace(Constants.SchemaIdentifier, Context.Model.GetDefaultSchema(), StringComparison.OrdinalIgnoreCase);
                if (connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }

                command.CommandType = CommandType.Text;
                command.CommandTimeout = 300;
                if (param is not null)
                {
                    foreach (var (parameterName, value) in param)
                    {
                        var parameter = command.CreateParameter();
                        parameter.ParameterName = parameterName;
                        parameter.Value = value;
                        _ = command.Parameters.Add(parameter);
                    }
                }

                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception exc)
            {
                Logger.LogException(exc);
                throw;
            }
        }

        public async Task<DataSet> SqlQueryAsync([NotNull] string sql, IList<(string ParameterName, object Value)>? param = null)
        {
            try
            {
                var connection = Context.Database.GetDbConnection();
                using var command = connection.CreateCommand();
                command.CommandText = sql.Replace(Constants.SchemaIdentifier, Context.Model.GetDefaultSchema(), StringComparison.OrdinalIgnoreCase);
                if (connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }

                command.CommandType = CommandType.Text;
                command.CommandTimeout = 300;

                if (param is not null)
                {
                    for (var i = 0; i < param.Count; i++)
                    {
                        var parameter = command.CreateParameter();
                        parameter.ParameterName = param[i].ParameterName;
                        parameter.Value = param[i].Value;
                        _ = command.Parameters.Add(parameter);
                    }
                }

                DataSet dataSet = new();
                using var reader = await command.ExecuteReaderAsync();
                do
                {
                    DataTable dt = new();
                    dt.Load(reader);
                    dataSet.Tables.Add(dt);
                }
                while (!reader.IsClosed && await reader.NextResultAsync());

                return dataSet;
            }
            catch (Exception exc)
            {
                Logger.LogException(exc);
                throw;
            }
        }

        public async Task<int> GetSequenceValueAsync(string sequence)
        {
            var connection = Context.Database.GetDbConnection();
            using var command = connection.CreateCommand();
            command.CommandText = GetSequenceCommand(sequence, Context.Model.GetDefaultSchema());
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            var value = (decimal)await command.ExecuteScalarAsync();
            return Convert.ToInt32(value);
        }

        public int GetSequenceValue(string sequence)
        {
            var connection = Context.Database.GetDbConnection();
            using var command = connection.CreateCommand();
            command.CommandText = GetSequenceCommand(sequence, Context.Model.GetDefaultSchema());
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            var value = (decimal)command.ExecuteScalar();
            return Convert.ToInt32(value);
        }

        #region IDisposable Implementation

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void CheckDisposed()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException("The UnitOfWork is already disposed and cannot be used anymore.");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed && disposing && Context is not null)
            {
                Context.Dispose();
                Context = null;
            }

            IsDisposed = true;
        }

        #endregion

        #region Private Methods

        private static string? GetSequenceCommand(string sequence, string? schema) => $"SELECT {(!string.IsNullOrEmpty(schema) ? $"{schema}." : string.Empty)}{sequence}.NEXTVAL FROM DUAL";

        private void Prepare()
        {
            var changedEntities = Context.ChangeTracker.Entries().Where(x => x.State is EntityState.Added or EntityState.Modified);
            foreach (var item in changedEntities)
            {
                if (item.Entity is null)
                {
                    continue;
                }

                var properties = item.Entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanRead && p.CanWrite);

                foreach (var property in properties)
                {
                    if (property.PropertyType == typeof(string))
                    {
                        var val = property.GetValue(item.Entity, null) as string;
                        if (!string.IsNullOrEmpty(val))
                        {
                            var newVal = val.NormalizePersian();
                            if (newVal != val)
                            {
                                property.SetValue(item.Entity, newVal, null);
                            }
                        }
                    }
                    else
                    {
                        if (item.State == EntityState.Added)
                        {
                            if (property.GetCustomAttributes(typeof(DatabaseGeneratedAttribute), false).FirstOrDefault() is DatabaseGeneratedAttribute attribute && attribute.DatabaseGeneratedOption == DatabaseGeneratedOption.Identity && !string.IsNullOrEmpty(attribute.SequenceName))
                            {
                                var val = GetSequenceValue(attribute.SequenceName);
                                property.SetValue(item.Entity, val, null);
                            }
                        }
                    }
                }
            }
        }

        private async Task PrepareAsync()
        {
            var changedEntities = Context.ChangeTracker.Entries().Where(x => x.State is EntityState.Added or EntityState.Modified);
            foreach (var item in changedEntities)
            {
                if (item.Entity is null)
                {
                    continue;
                }

                var properties = item.Entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanRead && p.CanWrite);

                foreach (var property in properties)
                {
                    if (property.Name == nameof(Concurrency.IRowVersion.RowVersion) && property.PropertyType == typeof(long))
                    {
                        if (property.GetValue(item.Entity, null) is long val)
                        {
                            property.SetValue(item.Entity, val++, null);
                        }
                    }
                    else if (property.PropertyType == typeof(string))
                    {
                        var val = property.GetValue(item.Entity, null) as string;
                        if (!string.IsNullOrEmpty(val))
                        {
                            var newVal = val.NormalizePersian();
                            if (newVal != val)
                            {
                                property.SetValue(item.Entity, newVal, null);
                            }
                        }
                    }
                    else
                    {
                        if (item.State == EntityState.Added)
                        {
                            if (property.GetCustomAttributes(typeof(DatabaseGeneratedAttribute), false).FirstOrDefault() is DatabaseGeneratedAttribute attribute && attribute.DatabaseGeneratedOption == DatabaseGeneratedOption.Identity && !string.IsNullOrEmpty(attribute.SequenceName))
                            {
                                var val = await GetSequenceValueAsync(attribute.SequenceName);
                                property.SetValue(item.Entity, val, null);
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}

#pragma warning restore CA2100 // Review SQL queries for security vulnerabilities
