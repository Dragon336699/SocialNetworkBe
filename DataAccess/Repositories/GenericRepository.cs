using DataAccess.DbContext;
using Domain.Interfaces.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected SocialNetworkDbContext _context;
        public GenericRepository(SocialNetworkDbContext context)
        {
            _context = context;
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            var entity = await _context.Set<T>().FindAsync(id);
            if (entity == null)
            {
                return null;
            }
            return entity;
        }

        public async Task<IEnumerable<T>?> GetAllAsync()
        {
            var entities = await _context.Set<T>().ToListAsync();
            if (entities == null) return null;
            return entities;
        }

        public async Task<IEnumerable<T>?> FindAsync(Expression<Func<T, bool>> expression)
        {
            var entities = await _context.Set<T>().Where(expression).ToListAsync();
            if (entities == null) return null;
            return entities;
        }

        public async Task<T?> FindFirstAsync(Expression<Func<T, bool>> expression)
        {
            var entity = await _context.Set<T>().Where(expression).FirstOrDefaultAsync();
            if (entity == null) return null;
            return entity;
        }

        public void Add(T entity)
        {
            _context.Set<T>().Add(entity);
        }
        public void AddRange(IEnumerable<T> entities)
        {
            _context.Set<T>().AddRange(entities);
        }
        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
        }

        public void Remove(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _context.Set<T>().RemoveRange(entities);
        }
    }
}
