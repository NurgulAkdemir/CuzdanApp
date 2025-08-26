using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DAL.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
        Task<int> SaveChangesAsync(); // opsiyonel: transaction'ları kontrol etmek için
        IQueryable<T> Query(); // advanced sorgular için (dikkat: dışarıya IQueryable veriyorsun)
    }
}