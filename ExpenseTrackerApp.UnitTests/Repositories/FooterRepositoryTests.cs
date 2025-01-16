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

using ExpenseTrackerApp.Data.Repositories.IRepsitories;

namespace ExpenseTrackerApp.UnitTests.Repositories;

public class FooterRepositoryTests
{
    private Mock<ISocialLinksRepository> mockSocialLinksRepository = new Mock<ISocialLinksRepository>();
    private DbContextOptions<ApplicationDbContext> CreateDbContextOptions()
    {
        return new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"TestDatabase_{Guid.NewGuid()}")
            .Options;
    }
    
    [Fact]
    public void getFooterNoFooterSuccessTest()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using (var context = new ApplicationDbContext(options))
        {
            var footer = new Footer()
            {
                Id = 1,
                CopryRightHolder = "test",
            };
            
            List<SocialLink> socialLinks = new List<SocialLink>()
            {
                new SocialLink() {Id = 1, Footer = footer, Platform = "test1", Url = "url1", IconClass = "class1"},
                new SocialLink() { Id = 2, Footer = footer, Platform = "test2", Url = "url2", IconClass = "class2"}
            };
            
            context.Add(footer);
            context.SaveChanges();
            
            mockSocialLinksRepository.Setup(m => m.getLinksBelongingToCertainFooter(1))
                .Returns(socialLinks);

            var footerRepo = new FooterRepository(
                context,
                mockSocialLinksRepository.Object
            );
            
            // Act
            var result = footerRepo.GetFooter();
            
            // Assert
            Assert.NotNull(result);
            result.Should().BeEquivalentTo(footer);
            Assert.NotNull(result.SocialLinks);
            Assert.Equal(2, result.SocialLinks.Count());
        }
    }
    
    [Fact]
    public void getFooterNoFooterNoSocialLinksSuccessTest()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using (var context = new ApplicationDbContext(options))
        {
            var footer = new Footer()
            {
                Id = 1,
                CopryRightHolder = "test",
            };
            
            context.Add(footer);
            context.SaveChanges();
            
            mockSocialLinksRepository.Setup(m => m.getLinksBelongingToCertainFooter(1))
                .Returns(new List<SocialLink>());

            var footerRepo = new FooterRepository(
                context,
                mockSocialLinksRepository.Object
            );
            
            // Act
            var result = footerRepo.GetFooter();
            
            // Assert
            Assert.NotNull(result);
            result.Should().BeEquivalentTo(footer);
            Assert.NotNull(result.SocialLinks);
            Assert.Empty(result.SocialLinks);
        }
    }

    [Fact]
    public void getFooterNoFooterFailTest()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using (var context = new ApplicationDbContext(options))
        {
            mockSocialLinksRepository.Setup(m => m.getLinksBelongingToCertainFooter(1))
                .Returns(new List<SocialLink>());

            var footerRepo = new FooterRepository(
                context,
                mockSocialLinksRepository.Object
            );
            
            // Act
            var exception = Assert.Throws<Exception>(() => footerRepo.GetFooter());
            
            // Assert
            Assert.Equal("No footer found", exception.Message);
        }
    }
}