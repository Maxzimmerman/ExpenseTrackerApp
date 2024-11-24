using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using ExpenseTrackerApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerApp.Data.Repositories
{
    public class FooterRepository : Repository<Footer>, IFooterRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public FooterRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public Footer GetFooter()
        {
            var footerModel = _applicationDbContext.footers.FirstOrDefault();
            footerModel.SocialLinks = _applicationDbContext.socialLinks
                .Where(link => link.FooterId == footerModel.Id)
                .ToList();
            return footerModel;
        }
    }
}
