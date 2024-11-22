namespace ExpenseTrackerApp.Data.Repositories.IRepsitories
{
    public interface IRepository<T> where T : class
    {
        void Add(T entry);
        void Delete(T entry);
    }
}
