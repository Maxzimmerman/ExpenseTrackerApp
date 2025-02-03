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

public class MessageRepositoryTests
{
    private DbContextOptions<ApplicationDbContext> CreateDbContextOptions()
    {
        return new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"TestDatabase_{Guid.NewGuid()}")
            .Options;
    }

    [Fact]
    public void containsMessageThisMonthShouldReturnNothing()
    {
        // Arrane
        var options = CreateDbContextOptions();

        using (var context = new ApplicationDbContext(options))
        {
            var repository = new MessageRepository(context);
            
            // Act
            var result = repository.ContainsMessageThisMonth(
                    "userid", 
                    "testmessage", 
                    DateTime.Now.Year, 
                    DateTime.Now.Month
                    );
            
            Assert.False(result);
        }
    }
}