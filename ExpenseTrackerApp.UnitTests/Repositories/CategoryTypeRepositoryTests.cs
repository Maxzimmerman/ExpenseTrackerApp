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

public class CategoryTypeRepositoryTests
{
    private DbContextOptions<ApplicationDbContext> CreateDbContextOptions()
    {
        return new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"TestDatabase_{Guid.NewGuid()}")
            .Options;
    }
    
    // get category types as select list items start
    [Fact]
    public void getCategoryTypesAsSelectListItemsSuccessTest()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using (var context = new ApplicationDbContext(options))
        {
            var categoryTypes = new List<CategoryType>()
            {
                new CategoryType() {Name = "CategoryType 1"},
                new CategoryType() {Name = "CategoryType 2"},
            };  
            
            context.categoriesTypes.AddRange(categoryTypes);
            context.SaveChanges();
            
            var categoryTypeRepo = new CategoryTypeRepository(context);

            IEnumerable<SelectListItem> expectedResult = new List<SelectListItem>()
            {
                new SelectListItem() { Text = "CategoryType 1", Value = "1" },
                new SelectListItem() { Text = "CategoryType 2", Value = "2" },
            };
            
            // Act
            var result = categoryTypeRepo.GetCategoryTypesAsSelectListItems();
            
            Assert.NotNull(result);
            result.Should().BeEquivalentTo(expectedResult);
        }
    }
    
    [Fact]
    public void getCategoryTypesAsSelectListItemsFailTest()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using (var context = new ApplicationDbContext(options))
        {
            var categoryTypeRepo = new CategoryTypeRepository(context);
            
            // Act
            var result = categoryTypeRepo.GetCategoryTypesAsSelectListItems();
            
            Assert.NotNull(result);
            result.Should().BeEmpty();
        }
    }
    // get category types as select list items end
    
    // get category type for certain category start
    [Fact]
    public void getCategoryTypeForCertainCategorySuccessTest()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using (var context = new ApplicationDbContext(options))
        {
            var categoryTypes = new List<CategoryType>()
            {
                new CategoryType() {Name = "CategoryType 1"},
                new CategoryType() {Name = "CategoryType 2"},
            };  
            
            context.categoriesTypes.AddRange(categoryTypes);
            context.SaveChanges();
            
            var categoryTypeRepo = new CategoryTypeRepository(context);
            
            // Act
            var result = categoryTypeRepo.GetCategoryTypeForCertainCategory(1);
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("CategoryType 1", result.Name);
            result.Should().BeEquivalentTo(categoryTypes[0]);
        }
    }

    [Fact]
    public void getCategoryTypeForCertainCategoryFailTest()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using (var context = new ApplicationDbContext(options))
        {
            var categoryTypeRepo = new CategoryTypeRepository(context);
        
            // Act
            var exceptions = Assert.Throws<Exception>(() => categoryTypeRepo.GetCategoryTypeForCertainCategory(1));
        
            // Assert
            Assert.Equal("Could not find CategoryType", exceptions.Message);
        }
    }
    // get category type for certain category end
}