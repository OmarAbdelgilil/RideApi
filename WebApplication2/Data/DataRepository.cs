using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;

namespace WebApplication1.Data
{
    public class DataRepository<T> : IDataRepository<T> where T : class
    {
        private readonly DataContext _db;
        private readonly DbSet<T> table;

        public DataRepository(DataContext db)
        {
            _db = db;
            table = _db.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await table.ToListAsync();
        }

        public async Task<T> GetByEmailAsync(String email)
        {
            return await table.FindAsync(email);
        }

        public async Task AddAsync(T entity)
        {
            await table.AddAsync(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            _db.Entry(entity).State = EntityState.Modified;
        }

        public async Task DeleteAsync(T entity)
        {
            table.Remove(entity);
        }
        public async Task DeleteAsyncByEmail(String email)
        {
            T? entity = await table.FindAsync(email);
            if (entity != null)
            {
                table.Remove(entity);
            }
        }

            public async Task<bool> Save()
        {
            return await _db.SaveChangesAsync() > 0;
        }
    }
}
