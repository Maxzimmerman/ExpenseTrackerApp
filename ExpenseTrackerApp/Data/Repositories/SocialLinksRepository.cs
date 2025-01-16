using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using ExpenseTrackerApp.Models;

namespace ExpenseTrackerApp.Data.Repositories
{
    public class SocialLinksRepository : Repository<SocialLink>, ISocialLinksRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public SocialLinksRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public List<SocialLink> getLinksBelongingToCertainFooter(int footerId)
        {
            return _applicationDbContext.socialLinks
                .Where(link => link.FooterId == footerId)
                .ToList();
        }
    }
}
