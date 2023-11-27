using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Proj.DataAccess.Repository.IRepository
{
    public interface IRepository <T> where T : class
    {
        IEnumerable<T> GetAll(string? includeProperties = null);
        T Get(Expression<Func<T,bool>> filter, string? includeProperties = null);
        void Add(T entity);

        //___ Update Senerio is some time different so we handle update in different Repo ____
        //void Update(T entity);

        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entity);
    }
}
