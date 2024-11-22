using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerApp.Data.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _applicationDbContext;
        internal DbSet<T> dbSet { get; set; }

        public Repository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
            this.dbSet = _applicationDbContext.Set<T>();
        }

        public virtual async void Add(T entry)
        {
            _applicationDbContext.Add(entry);
            _applicationDbContext.SaveChanges();
        }

        public virtual async void Delete(T entry)
        {
            _applicationDbContext.Remove(entry);
            _applicationDbContext.SaveChanges();
        }
    }
}
