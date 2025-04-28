using Microsoft.EntityFrameworkCore.Query;
using RookieEcommerce.Domain.Entities;
using System.Linq.Expressions;

namespace RookieEcommerce.Application.Contacts.Persistence
{
    public interface IBaseRepository<T> where T : BaseEntity
    {
        Task<T?> GetByIdAsync(Guid id, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null, CancellationToken cancellationToken = default);

        Task<T?> GetByAttributeAsync(Expression<Func<T, bool>>? filter, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null, CancellationToken cancellationToken = default);

        Task<List<T>> ListAllAsync(Expression<Func<T, bool>>? filter, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null, CancellationToken cancellationToken = default);

        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

        Task DeleteAsync(T entity, CancellationToken cancellationToken = default);

        Task DeleteByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<bool> AnyAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default);

        Task<int> CountAsync(Expression<Func<T, bool>>? filter, CancellationToken cancellationToken = default);
    }
}