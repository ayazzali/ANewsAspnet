using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data.Entity;

namespace Core.Entities
{
    public class NewsContext : DbContext
    {
        public NewsContext():base()//"NewsContext"
        {
        }
        
        public DbSet<TSource> TSources { get; set; }
        public DbSet<TLogin> TLogin { get; set; }//todo s
        public DbSet<TPreferableWords> TFavoriteWords { get; set; }
        public DbSet<TArticle> TParsedNews { get; set; }

        #region SaveChanges
        public override int SaveChanges()
        {
            UpdateCreatedAndModified();
            return base.SaveChanges();
        }
        public override Task<int> SaveChangesAsync()
        {
            UpdateCreatedAndModified();
            return base.SaveChangesAsync();
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
        private void UpdateCreatedAndModified()
        {
            var entities = from e in ChangeTracker.Entries<BaseEntity>()
                           where e.State == EntityState.Added || e.State == EntityState.Modified
                           select e;
            foreach (var e in entities)
            {
                e.Entity.Modified = DateTime.Now;
                if (e.State == EntityState.Added)
                {
                    //if (e.Entity.Id == 0)
                    //{
                    //    e.Entity.Id = GenerateId();
                    //    //throw new IndexOutOfRangeException(string.Format("You forget to set {0}.Id", e.Entity.GetType().ToString()));
                    //}
                    if (e.Entity.Created == DateTime.MinValue)
                    {
                        e.Entity.Created = DateTime.Now;
                    }
                }
            }
        }
        #endregion
    }

    public class TLogin : BaseEntity
    {
        public string Login { get; set; }

        public virtual ICollection<TSource> TSources { get; set; }
        public virtual ICollection<TPreferableWords> TPreferableWords { get; set; }

        public TLogin()
        {
            TSources = new List<TSource>();
            TPreferableWords = new List<TPreferableWords>();
        }
    }

    /// <summary>
    /// rss,статья,почтa,пуш
    /// </summary>
    public class TSource : BaseEntity
    {
        /// <summary>
        /// data
        /// </summary>
        public string Link { get; set; }

        public int Type { get; set; }//enums

        public virtual ICollection<TLogin> TLogins { get; set; }//а если у разных клиентов совподает один\много новостных сайтов
        public virtual ICollection<TArticle> TArticles { get; set; }

        /// <summary>
        /// нул всегда если query в гугл или наоборот
        /// </summary>
        //public virtual ICollection<TPreferableWords> TPreferableWords { get; set; }

        public TSource()
        {
            TArticles = new List<TArticle>();
            TLogins = new List<TLogin>();
            //TPreferableWords = new List<TPreferableWords>();
        }
    }
    
    public class TPreferableWords : BaseEntity
    {
        /// <summary>
        /// EXample:Путин
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// если false значит минус-стоп-слово 
        /// </summary>
        public bool IsMust { get; set; }

        public int TLoginId { get; set; }
        public TLogin TLogin { get; set; }
    }

    /// <summary>
    /// TParsedNews rawArticle
    /// </summary>
    public class TArticle: BaseEntity
    {
        public string Data { get; set; }
        public string OwnLink { get; set; }
        public string Title { get; set; }//description at google.com
        /// <summary>
        /// structure of data-news
        /// </summary>
        public int Version { get; set; }
        public DateTime Date { get; set; }

        public int TSourceId { get; set; }
        public TSource TSource { get; set; }
    }

    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public bool Deleted { get; set; }
    }
}
