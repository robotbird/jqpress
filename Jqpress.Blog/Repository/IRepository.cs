using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jqpress.Blog.Repository
{
    /// <summary>
    /// Repository
    /// </summary>
    public partial interface IRepository<T>
    {
        T GetById(object id);
        void Insert(T entity);
        void Update(T entity);
        void Delete(T entity);
        IQueryable<T> Table { get; }
    }
}
