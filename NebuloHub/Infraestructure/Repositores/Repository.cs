using Microsoft.EntityFrameworkCore;
using NebuloHub.Infraestructure.Context;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace NebuloHub.Infraestructure.Repositores
{
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
            var keyProperty = typeof(T).GetProperties()
                .FirstOrDefault(p =>
                    (p.PropertyType == typeof(long) || p.PropertyType == typeof(int)) &&
                    (p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase) ||
                     p.Name.StartsWith("Id", StringComparison.OrdinalIgnoreCase) ||
                     p.Name.EndsWith("Id", StringComparison.OrdinalIgnoreCase)));

            if (keyProperty == null)
                throw new InvalidOperationException($"A entidade {typeof(T).Name} não possui chave primária numérica.");

            return await _dbSet.FirstOrDefaultAsync(e =>
                EF.Property<long>(e, keyProperty.Name) == id);
        }

        // Busca por chave string (ex: CPF, CNPJ, ou Id string)
        public async Task<T?> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id));

            var keyProperty = typeof(T).GetProperties()
                .FirstOrDefault(p =>
                    p.PropertyType == typeof(string) &&
                    (p.Name.Equals("CPF", StringComparison.OrdinalIgnoreCase) ||
                     p.Name.Equals("CNPJ", StringComparison.OrdinalIgnoreCase) ||
                     p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase)));

            if (keyProperty == null)
                throw new InvalidOperationException($"A entidade {typeof(T).Name} não possui CPF, CNPJ ou Id (string).");

            return await _dbSet.FirstOrDefaultAsync(e =>
                EF.Property<string>(e, keyProperty.Name) == id);
        }

        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

        public async Task AddAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await _dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _dbSet.Remove(entity);
        }

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
