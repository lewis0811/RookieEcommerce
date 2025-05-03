using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Domain.Entities;
using System.Linq.Expressions;

namespace RookieEcommerce.Infrastructure.Persistence
{
    public class BaseRepository<T>(DbSet<T> dbSet) : IBaseRepository<T> where T : BaseEntity
    {
        public virtual async Task<T?> GetByIdAsync(Guid id, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null, CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = dbSet;

            //query = AddIncludesToQuery(includeProperties, query);
            if (include != null)
            {
                query = include(query);
            }

            return await query.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public virtual async Task<T?> GetByAttributeAsync(Expression<Func<T, bool>>? filter, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null, CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (include != null)
            {
                query = include(query);
            }

            return await query.FirstOrDefaultAsync<T>(cancellationToken);
        }

        public virtual async Task<List<T>> ListAllAsync(Expression<Func<T, bool>>? filter, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null, CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (include != null)
            {
                query = include(query);
            }

            return await query.ToListAsync(cancellationToken);
        }

        public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await dbSet.AddAsync(entity, cancellationToken);
            return entity;
        }

        public virtual Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            dbSet.Update(entity);
            return Task.CompletedTask;
        }

        public virtual Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            dbSet.Remove(entity);
            return Task.CompletedTask;
        }

        public virtual async Task DeleteByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            T? entityToDelete = await GetByIdAsync(id, null, cancellationToken);
            if (entityToDelete != null)
            {
                await DeleteAsync(entityToDelete, cancellationToken);
            }
        }

        public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default)
        {
            return await dbSet.AnyAsync(filter, cancellationToken);
        }

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? filter, CancellationToken cancellationToken = default)
        {
            if (filter != null)
            {
                return await dbSet.CountAsync(filter, cancellationToken);
            }
            return await dbSet.CountAsync(cancellationToken);
        }

        public static IQueryable<T> AddIncludesToQuery(string? includeProperties, IQueryable<T> query)
        {
            if (!string.IsNullOrEmpty(includeProperties))
            {
                var entityProperties = typeof(T).GetProperties().Select(c => c.Name);

                var validProperty = includeProperties.Trim()
                  .Split(",")
                  .Where(property => entityProperties
                    .Any(c => c.Equals(property.Trim()))
                  );

                foreach (var property in validProperty)
                {
                    var trimmedProperty = property.Trim();
                    query = query.Include(trimmedProperty);
                }
            }

            return query;
        }
    }
}