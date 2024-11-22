using ExpenseTrackerApp.Models;

namespace ExpenseTrackerApp.Data.Repositories.IRepsitories
{
    public interface IFooterRepository : IRepository<Footer>
    {
        Footer GetFooter(); 
    }
}
