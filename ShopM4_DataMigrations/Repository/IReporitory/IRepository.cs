using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ShopM4_DataMigrations.Repository.IReporitory
{
    public interface IRepository<T> where T : class
    {
        T Find(int id);
        IEnumerable<T> GetAll(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, bool isTracking = true,
            string includeProperties = null);
        T FirstOrDefault(Expression<Func<T, bool>> filter = null, string includeProperties = null
            , bool isTracking = true);
        void Add(T item);
        void Remove(T item);
        void Save();
    }
}
