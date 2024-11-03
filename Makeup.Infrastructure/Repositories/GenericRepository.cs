using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Travel.Core.IRepositories;
using Travel.Infrastructure.Data;

namespace Travel.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext dbContext;

        public GenericRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task Create(T model)
        {
            await dbContext.Set<T>().AddAsync(model);
        }

        public void Delete(int id)
        {
            var entity = dbContext.Set<T>().Find(id);

            if (entity == null)
                throw new InvalidOperationException($"Entity with ID {id} not found.");
            dbContext.Set<T>().Remove(entity);
        }

        public async Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> Filter = null, int page_size = 2, int page_number = 1, string? includeProperty = null)
        {

            IQueryable<T> query = dbContext.Set<T>();
            if (Filter != null)
            {
                query = query.Where(Filter);
            }

            if (includeProperty != null)
            {
                foreach (var property in includeProperty.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property.Trim());
                }

            }

            if (page_size > 0)
            {
                if (page_size > 4)
                {
                    page_size = 4;
                }
                query = query.Skip(page_size * (page_number - 1)).Take(page_size);
            }
            // return await dbContext.Set<T>().ToListAsync();
            return await query.ToListAsync();

        }

        public async Task<T> GetById(int id)
        {
            return await dbContext.Set<T>().FindAsync(id);
        }

        public void Update(T model)
        {
            dbContext.Set<T>().Update(model);
        }
    }
}
