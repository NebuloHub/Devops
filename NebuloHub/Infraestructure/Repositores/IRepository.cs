namespace NebuloHub.Infraestructure.Repositores
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(long id);
        Task<T?> GetByIdAsync(string id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task SaveChangesAsync();
    }
}
