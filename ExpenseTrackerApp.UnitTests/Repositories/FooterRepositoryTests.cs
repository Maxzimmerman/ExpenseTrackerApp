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
    private Mock<ISocialLinksRepository> mockSocialLinksRepository;
    private DbContextOptions<ApplicationDbContext> CreateDbContextOptions()
    {
        return new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"TestDatabase_{Guid.NewGuid()}")
            .Options;
    }

    [Fact]
    public void getFooterSuccessTest()
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
            var result = footerRepo.GetFooter();
            
            // Assert
            Assert.Null(result);
        }
    }
}