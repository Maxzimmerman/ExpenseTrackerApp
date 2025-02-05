using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpenseTrackerApp.Data;
using ExpenseTrackerApp.Data.Repositories;
using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using ExpenseTrackerApp.Models;
using ExpenseTrackerApp.UnitTests.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using ExpenseTrackerApp.Models.ViewModels.BudgetViewModels;
using ExpenseTrackerApp.UnitTests.Helpers.AddDummyData;
using FluentAssertions;

namespace ExpenseTrackerApp.UnitTests.Repositories;

public class SocialLinksRepositoryTests
{
    private DbContextOptions<ApplicationDbContext> CreateDbContextOptions()
    {
        return new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"TestDatabase_{Guid.NewGuid()}")
            .Options;
    }

    [Fact]
    public void getLinksBelongingToCertainFooterShouldReturnLinks()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using (var context = new ApplicationDbContext(options))
        {
            var socialLinkRepository = new SocialLinksRepository(context);
            var footerDb = new Footer();
            
            var socialLinksDb = new List<SocialLink>()
            {
                new SocialLink() { Platform = "", Url = "", IconClass = "", Footer = footerDb },
                new SocialLink() { Platform = "", Url = "", IconClass = "", Footer = footerDb },
            };
            
            context.AddRange(socialLinksDb);
            context.SaveChanges();
            
            // Act
            var result = socialLinkRepository.getLinksBelongingToCertainFooter(footerDb.Id);
            
            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            result.Count().Should().Be(socialLinksDb.Count);
        }
    }

    [Fact]
    public void getLinksBelongingToCertainFooterNoMatchingEntriesShouldReturnLinks()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using (var context = new ApplicationDbContext(options))
        {
            var socialLinkRepository = new SocialLinksRepository(context);
            var footerDb = new Footer();

            var socialLinksDb = new List<SocialLink>()
            {
                new SocialLink() { Platform = "", Url = "", IconClass = "", Footer = footerDb },
                new SocialLink() { Platform = "", Url = "", IconClass = "", Footer = footerDb },
            };

            context.AddRange(socialLinksDb);
            context.SaveChanges();

            // Act
            var result = socialLinkRepository.getLinksBelongingToCertainFooter(footerDb.Id + 1);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }

    [Fact]
    public void getLinksBelongingToCertainFooterNoEntryInDbShouldReturnEmptyList()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using (var context = new ApplicationDbContext(options))
        {
            var socialLinkRepository = new SocialLinksRepository(context);

            // Act
            var result = socialLinkRepository.getLinksBelongingToCertainFooter(1);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}