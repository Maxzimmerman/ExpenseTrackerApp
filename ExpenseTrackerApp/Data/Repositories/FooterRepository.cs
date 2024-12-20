﻿using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using ExpenseTrackerApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerApp.Data.Repositories
{
    public class FooterRepository : Repository<Footer>, IFooterRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly ISocialLinksRepository _socialLinksRepository;

        public FooterRepository(ApplicationDbContext applicationDbContext,
            ISocialLinksRepository socialLinksRepository) : base(applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
            _socialLinksRepository = socialLinksRepository;
        }

        public Footer GetFooter()
        {
            Footer footerModel = new Footer();
            footerModel = _applicationDbContext.footers.FirstOrDefault();
            if (footerModel == null)
                return null;
            footerModel.SocialLinks = _socialLinksRepository.socialLinks(footerModel.Id);
            return footerModel;
        }
    }
}
