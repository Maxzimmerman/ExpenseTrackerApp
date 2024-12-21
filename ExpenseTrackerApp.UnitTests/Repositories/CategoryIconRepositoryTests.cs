using System.Collections;
using Moq;
using Microsoft.EntityFrameworkCore;
using ExpenseTrackerApp.Data;
using ExpenseTrackerApp.Data.Repositories;
using ExpenseTrackerApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using ExpenseTrackerApp.UnitTests.Helpers.AddDummyData;
using FluentAssertions;

namespace ExpenseTrackerApp.UnitTests.Repositories;

public class CategoryIconRepositoryTests
{
    private DbContextOptions<ApplicationDbContext> CreateDbContextOptions()
    {
        return new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"TestDatabase_{Guid.NewGuid()}")
            .Options;
    }
    
    // BetCategoryIconsAs Select List Items Start
    [Fact]
    public void getCategoryIconsAsSelectListItemsSuccessTest()
    {
        // Arrange
        var options = CreateDbContextOptions();
        var context = new ApplicationDbContext(options);
        
        var categoryIcons = new List<CategoryIcon>()
        {
            new CategoryIcon() { Name = "Icon1", Code = "icon1"},
            new CategoryIcon() { Name = "Icon2", Code = "icon2" },
        };
        
        context.categoriesIcons.AddRange(categoryIcons);
        context.SaveChanges();

        var categoryIconRepo = new CategoryIconRepository(context);

        IEnumerable<SelectListItem> expectedResult = new List<SelectListItem>()
        {
            new SelectListItem() { Text = "Icon1", Value = "1" },
            new SelectListItem() { Text = "Icon2", Value = "2" },
        };
        
        // Act
        var result = categoryIconRepo.GetCategoryIconsAsSelectListItems();
        
        // Assert
        Assert.NotNull(result);
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public void getCategoryIconsAsSelectListItemsFailTest()
    {
        // Arrange
        var options = CreateDbContextOptions();
        var context = new ApplicationDbContext(options);
        
        var categoryIconRepo = new CategoryIconRepository(context);
        
        // Act
        var result = categoryIconRepo.GetCategoryIconsAsSelectListItems();
        
        // Assert
        Assert.NotNull(result);
        result.Should().BeEmpty();
    }
    // BetCategoryIconsAs Select List Items Start
    
    // GetCategoryIconFerCertainCategory Start
    [Fact]
    public void getGetCategoryIconFerCertainCategorySuccessTest()
    {
        // Arrange
        var options = CreateDbContextOptions();
        var context = new ApplicationDbContext(options);

        var categoryIcons = new List<CategoryIcon>()
        {
            new CategoryIcon() { Name = "Icon1", Code = "icon1"},
            new CategoryIcon() { Name = "Icon2", Code = "icon2" },
        };
        
        context.categoriesIcons.AddRange(categoryIcons);
        context.SaveChanges();
        
        var categoryIconRepo = new CategoryIconRepository(context);
        
        // Act
        var result = categoryIconRepo.GetCategoryIconById(1);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Icon1", result.Name);
        Assert.Equal("icon1", result.Code);
        result.Should().BeEquivalentTo(categoryIcons[0]);
    }

    [Fact]
    public void getGetCategoryColorFerCertainCategoryFailTest()
    {
        // Arrange
        var options = CreateDbContextOptions();
        var context = new ApplicationDbContext(options);
        
        var categoryIconRepo = new CategoryIconRepository(context);
        
        // Act
        var exceptions = Assert.Throws<Exception>(() => categoryIconRepo.GetCategoryIconById(1));
        
        // Assert
        Assert.Equal("Could not find CategoryIcon", exceptions.Message);
    }
    
    // GetCategoryIconFerCertainCategory End
}