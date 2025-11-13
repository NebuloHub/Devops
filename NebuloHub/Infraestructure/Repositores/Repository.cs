using Microsoft.EntityFrameworkCore;
using NebuloHub.Infraestructure.Context;
using NebuloHub.Infraestructure.Repositores;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(AppDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(long id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<T?> GetByIdAsync(string id)
    {
        // Aqui tentamos encontrar a entidade que tenha uma propriedade "CPF" ou "CNPJ"
        var keyProperty = typeof(T).GetProperties()
            .FirstOrDefault(p => string.Equals(p.Name, "CPF", StringComparison.OrdinalIgnoreCase)
                              || string.Equals(p.Name, "CNPJ", StringComparison.OrdinalIgnoreCase)
                              || string.Equals(p.Name, "Id", StringComparison.OrdinalIgnoreCase));

        if (keyProperty == null)
            throw new InvalidOperationException($"A entidade {typeof(T).Name} não possui CPF, CNPJ ou Id.");

        return await _dbSet.FirstOrDefaultAsync(e =>
            EF.Property<string>(e, keyProperty.Name) == id);
    }

    public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

    public async Task AddAsync(T entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        await _dbSet.AddAsync(entity);
    }

    public void Update(T entity) => _dbSet.Update(entity);

    public void Delete(T entity) => _dbSet.Remove(entity);

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}
