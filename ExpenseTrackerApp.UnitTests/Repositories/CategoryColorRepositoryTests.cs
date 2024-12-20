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

public class CategoryColorRepositoryTests
{
    private DbContextOptions<ApplicationDbContext> CreateDbContextOptions()
    {
        return new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"TestDatabase_{Guid.NewGuid()}")
            .Options;
    }
    
    // BetCategoryColorsAs Select List Items Start
    
    [Fact]
    public void getCategoryColorsAsSelectListItemsSuccess()
    {
        // Arrange
        var options = CreateDbContextOptions();
        var context = new ApplicationDbContext(options);
        
        var categoryColors = new List<CategoryColor>()
        {
            new CategoryColor() { Name = "Color1", code = "color1"},
            new CategoryColor() { Name = "Color2", code = "color2" },
        };
        
        context.categoriesColors.AddRange(categoryColors);
        context.SaveChanges();

        var categoryColorRepo = new CategoryColorRepository(context);

        IEnumerable<SelectListItem> expectedResult = new List<SelectListItem>()
        {
            new SelectListItem() { Text = "Color1", Value = "1" },
            new SelectListItem() { Text = "Color2", Value = "2" },
        };
        
        // Act
        var result = categoryColorRepo.GetCategoryColorsAsSelectListItems();
        
        // Assert
        Assert.NotNull(result);
        result.Should().BeEquivalentTo(expectedResult);
    }
    // BetCategoryColorsAs Select List Items Start
    
    // GetCategoryColorFerCertainCategory Start
    [Fact]
    public void getGetCategoryColorFerCertainCategorySuccess()
    {
        // Arrange
        var options = CreateDbContextOptions();
        var context = new ApplicationDbContext(options);

        var categoryColors = new List<CategoryColor>()
        {
            new CategoryColor() { Name = "Color1", code = "color1" },
            new CategoryColor() { Name = "Color2", code = "color2" },
        };
        
        context.categoriesColors.AddRange(categoryColors);
        context.SaveChanges();
        
        var categoryColorRepo = new CategoryColorRepository(context);
        
        // Act
        var result = categoryColorRepo.GetCategoryColorFerCertainCategory(1);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Color1", result.Name);
        Assert.Equal("color1", result.code);
        result.Should().BeEquivalentTo(categoryColors[0]);
    }
    
    // GetCategoryColorFerCertainCategory End
}