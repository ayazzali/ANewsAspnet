using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.Repositories.Impl
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        protected NewsContext Db;
        protected IDbSet<TEntity> DbSet;


        public Repository(NewsContext db)
        {
            Db = db;
            DbSet = db.Set<TEntity>();
        }

        public virtual TEntity FindById(int id, bool includeDeleted = false)
        {
            if (includeDeleted)
            {
                return DbSet.SingleOrDefault(e => e.Id == id);
            }
            else
            {
                return DbSet.SingleOrDefault(e => e.Id == id && !e.Deleted);
            }
        }

        public virtual void Insert(TEntity entity)
        {
            DbSet.Add(entity);
            //return entity.Id;
        }

        public virtual void Delete(int id)
        {
            var entity = DbSet.Single(e => e.Id == id);
            entity.Deleted = true;
        }

        public virtual void Delete(TEntity entity)
        {
            entity.Deleted = true;
        }

        public void SaveChanges()
        {
            Db.SaveChanges();
        }
    }
}
