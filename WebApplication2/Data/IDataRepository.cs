using WebApplication2.Models;

namespace WebApplication1.Data
{
    public interface IDataRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByEmailAsync(String email);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task DeleteAsyncByEmail(String email);
        Task<bool> Save();
    }
}
