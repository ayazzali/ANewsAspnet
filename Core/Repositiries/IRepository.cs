using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.Repositories
{
    public interface IRepository<TEntity> where TEntity : BaseEntity
    {
        TEntity FindById(int id, bool includeDeleted = false);

        void Insert(TEntity entity);

        //void Update(TEntity entity);

        void Delete(int id);

        void Delete(TEntity entity);

        //IEnumerable<TEntity> All(bool includeDeleted = false);

        void SaveChanges();
    }
}
