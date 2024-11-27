using ExpenseTrackerApp.Models;

namespace ExpenseTrackerApp.Data.Repositories.IRepsitories
{
    public interface ISocialLinksRepository : IRepository<SocialLink>
    {
        List<SocialLink> socialLinks(int footerId);
    }
}
