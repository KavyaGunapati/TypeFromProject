using System.Linq.Expressions;
namespace TypeFormProject.Interfaces.IRepository
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<List<T>> GetAllAsync();
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task DeleteAsync(T entity);
    }
}
