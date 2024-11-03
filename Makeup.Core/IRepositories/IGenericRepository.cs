using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Core.IRepositories
{
    public interface IGenericRepository<T> where T : class
    {
        public Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> Filter, int page_size = 2, int page_number = 1, string? includeProperty = null); //includePropert for models with related data
        public Task<T> GetById(int id);
        public Task Create(T model);
        public void Update(T model);
        public void Delete(int id);
    }
}
