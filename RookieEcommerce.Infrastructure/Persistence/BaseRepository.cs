﻿using Microsoft.EntityFrameworkCore;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Domain.Entities;
using System.Linq.Expressions;

namespace RookieEcommerce.Infrastructure.Persistence
{
    public class BaseRepository<T>(DbSet<T> dbSet) : IBaseRepository<T> where T : BaseEntity
    {
        public virtual async Task<T?> GetByIdAsync(Guid id, string? includeProperties, CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = dbSet;
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var property in includeProperties.Split(','))
                {
                    query = query.Include(property.Trim());
                }
            }

            return await query.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public virtual async Task<List<T>> ListAllAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default)
        {
            if (filter != null)
            {
                dbSet.Where(filter);
            }
            return await dbSet.ToListAsync(cancellationToken);
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
    }
}