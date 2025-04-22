using BerAuto.Lib.Database;
using BerAuto_API.Lib.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BerAuto.Lib.Repositories
{
    public class GenericRepository<T> : IRepository<T> where T : class
    {
        protected readonly API_DbContext _context;
        private readonly DbSet<T> _entities;

        public GenericRepository(API_DbContext context)
        {
            _context = context;
            _entities = _context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync() => await _entities.ToListAsync();
        public async Task<T> GetByIdAsync(Guid id) => await _entities.FindAsync(id);
        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate) => await _entities.Where(predicate).ToListAsync();
        public async Task AddAsync(T entity) => await _entities.AddAsync(entity);
        public void Update(T entity) => _entities.Update(entity);
        public void Remove(T entity) => _entities.Remove(entity);
    }
}
