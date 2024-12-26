using System.Collections;
using System.Xml.XPath;
using Moq;
using Microsoft.EntityFrameworkCore;
using ExpenseTrackerApp.Data;
using ExpenseTrackerApp.Data.Repositories;
using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using ExpenseTrackerApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using ExpenseTrackerApp.UnitTests.Helpers.AddDummyData;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Formatters.Xml;

namespace ExpenseTrackerApp.UnitTests.Repositories;

public class CategoryRespositoryTests
{
    private Mock<ICategoryTypeRepsitory> mockCategoryTypeRepository = new Mock<ICategoryTypeRepsitory>();
    private Mock<ICategoryIconRepository> mockCategoryIconRepository = new Mock<ICategoryIconRepository>();
    private Mock<ICategoryColorRepository> mockCategoryColorRepository = new Mock<ICategoryColorRepository>();
    private Mock<Lazy<ITransactionRepository>> mockTransactionRepository = new Mock<Lazy<ITransactionRepository>>();
    
    private DbContextOptions<ApplicationDbContext> CreateDbContextOptions()
    {
        return new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"TestDatabase_{Guid.NewGuid()}")
            .Options;
    }
    
    // addCategoryData Tests Start
    [Fact]
    public void addCategoryDataNoDataSuccessTest()
    {
        // Arrange
        var options = CreateDbContextOptions();
        var context = new ApplicationDbContext(options);
        
        mockCategoryTypeRepository.Setup(mc => mc.GetCategoryTypesAsSelectListItems())
            .Returns(new List<SelectListItem>());
        mockCategoryIconRepository.Setup(mc => mc.GetCategoryIconsAsSelectListItems())
            .Returns(new List<SelectListItem>());
        mockCategoryColorRepository.Setup(mc => mc.GetCategoryColorsAsSelectListItems())
            .Returns(new List<SelectListItem>());

        var categoryRepo = new CategoryRepository
        (
            context,
            mockCategoryTypeRepository.Object,
            mockCategoryIconRepository.Object,
            mockCategoryColorRepository.Object,
            mockTransactionRepository.Object
        );
        
        // Act
        var result = categoryRepo.addCategoryData("userid");
        
        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.CategoryTypes);
        Assert.Empty(result.CategoryIcons);
        Assert.Empty(result.CategoryColors);
        Assert.Empty(result.Expenses);
        Assert.Empty(result.Incomes);
    }

    [Fact]
    public void addCategoryDataNoCategoriesSuccessTest()
    {
        // Arrange
        var options = CreateDbContextOptions();
        var context = new ApplicationDbContext(options);

        IEnumerable<SelectListItem> categoryTypes = new List<SelectListItem>()
        {
            new SelectListItem() { Text = "Expense", Value = "1"},
            new SelectListItem() { Text = "Income", Value = "2"},
        };

        IEnumerable<SelectListItem> categoryIcons = new List<SelectListItem>()
        {
            new SelectListItem() { Text = "Icon 1", Value = "1"},
            new SelectListItem() { Text = "Icon 2", Value = "2"},
        };

        IEnumerable<SelectListItem> categoryColors = new List<SelectListItem>()
        {
            new SelectListItem() { Text = "Color 1", Value = "1"},
            new SelectListItem() { Text = "Color 2", Value = "2"},
        };
        
        mockCategoryTypeRepository.Setup(mc => mc.GetCategoryTypesAsSelectListItems())
            .Returns(categoryTypes);
        mockCategoryIconRepository.Setup(mc => mc.GetCategoryIconsAsSelectListItems())
            .Returns(categoryIcons);
        mockCategoryColorRepository.Setup(mc => mc.GetCategoryColorsAsSelectListItems())
            .Returns(categoryColors);

        var categoryRepo = new CategoryRepository(
            context,
            mockCategoryTypeRepository.Object,
            mockCategoryIconRepository.Object,
            mockCategoryColorRepository.Object,
            mockTransactionRepository.Object
        );
        
        // Act
        var result = categoryRepo.addCategoryData("userid");
        
        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Expenses);
        Assert.Empty(result.Incomes);
        result.CategoryTypes.Should().BeEquivalentTo(categoryTypes);
        result.CategoryIcons.Should().BeEquivalentTo(categoryIcons);
        result.CategoryColors.Should().BeEquivalentTo(categoryColors);
    }
    
    
    [Fact]
    public void addCategoryDataSuccessTest()
    {
        // Arrange
        var options = CreateDbContextOptions();
        var context = new ApplicationDbContext(options);
        var (user, categories, categoryTypes, categoryIcons, categoryColors) = AddDummyCategoryData.AddCategoriesWithAllRelations(context);

        IEnumerable<SelectListItem> categoryTypesMock = new List<SelectListItem>()
        {
            new SelectListItem() { Text = "Income", Value = "1"},
            new SelectListItem() { Text = "Expense", Value = "2"},
        };

        IEnumerable<SelectListItem> categoryIconsMock = new List<SelectListItem>()
        {
            new SelectListItem() { Text = "Icon 1", Value = "1"},
            new SelectListItem() { Text = "Icon 2", Value = "2"},
        };

        IEnumerable<SelectListItem> categoryColorsMock = new List<SelectListItem>()
        {
            new SelectListItem() { Text = "Color 1", Value = "1"},
            new SelectListItem() { Text = "Color 2", Value = "2"},
        };
        
        mockCategoryTypeRepository.Setup(mc => mc.GetCategoryTypesAsSelectListItems())
            .Returns(categoryTypesMock);
        mockCategoryIconRepository.Setup(mc => mc.GetCategoryIconsAsSelectListItems())
            .Returns(categoryIconsMock);
        mockCategoryColorRepository.Setup(mc => mc.GetCategoryColorsAsSelectListItems())
            .Returns(categoryColorsMock);

        var categoryRepo = new CategoryRepository(
            context,
            mockCategoryTypeRepository.Object,
            mockCategoryIconRepository.Object,
            mockCategoryColorRepository.Object,
            mockTransactionRepository.Object
        );
        
        // Act
        var result = categoryRepo.addCategoryData(user.Id);
        
        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Expenses);
        Assert.NotEmpty(result.Incomes);
        result.CategoryTypes.Should().BeEquivalentTo(categoryTypesMock);
        result.CategoryIcons.Should().BeEquivalentTo(categoryIconsMock);
        result.CategoryColors.Should().BeEquivalentTo(categoryColorsMock);
    }
    // addCategoryData Test End
    
    // create Category Test Start
    [Fact]
    public void createCategorySuccessTest()
    {
        // Arrange
        var options = CreateDbContextOptions();
        var context = new ApplicationDbContext(options);
        
        mockCategoryTypeRepository.Setup(mc => mc.GetCategoryTypeForCertainCategory(1))
            .Returns(new CategoryType() { Id = 1, Name = "Category 1" });
        mockCategoryIconRepository.Setup(mc => mc.GetCategoryIconById(1))
            .Returns(new CategoryIcon() { Id = 1, Name = "Category 1", Code = "category1"});
        mockCategoryColorRepository.Setup(mc => mc.GetCategoryColorFerCertainCategory(1))
            .Returns(new CategoryColor() { Id = 1, Name = "Color 1", code = "color1" });

        var categoryRepo = new CategoryRepository(
            context,
            mockCategoryTypeRepository.Object,
            mockCategoryIconRepository.Object,
            mockCategoryColorRepository.Object,
            mockTransactionRepository.Object
        );

        var category = new Category()
        {
            Title = "Category 1",
            CategoryTypeId = 1,
            CategoryIconId = 1,
            CategoryColorId = 1,
            ApplicationUserId = "user1",
        };
        
        // Act
        categoryRepo.createCategory(category, "userid");
        
        // Assert
        var categoryDb = context.categories.FirstOrDefault();
        categoryDb.Should().BeEquivalentTo(category);
    }
    
    [Fact]
    public void createCategoryNoCategoryTypeFailTest()
    {
        // Arrange
        var options = CreateDbContextOptions();
        var context = new ApplicationDbContext(options);
        
        mockCategoryTypeRepository.Setup(mc => mc.GetCategoryTypeForCertainCategory(1))
            .Returns((CategoryType)null);
        mockCategoryIconRepository.Setup(mc => mc.GetCategoryIconById(1))
            .Returns(new CategoryIcon() { Id = 1, Name = "Category 1", Code = "category1"});
        mockCategoryColorRepository.Setup(mc => mc.GetCategoryColorFerCertainCategory(1))
            .Returns(new CategoryColor() { Id = 1, Name = "Color 1", code = "color1" });

        var categoryRepo = new CategoryRepository(
            context,
            mockCategoryTypeRepository.Object,
            mockCategoryIconRepository.Object,
            mockCategoryColorRepository.Object,
            mockTransactionRepository.Object
        );

        var category = new Category()
        {
            Title = "Category 1",
            CategoryTypeId = 1,
            CategoryIconId = 1,
            CategoryColorId = 1,
            ApplicationUserId = "user1",
        };
        
        // Act
        var exception = Assert.Throws<Exception>(() => categoryRepo.createCategory(category, "userid"));
        
        // Assert
        Assert.Equal("CategoryType not found.", exception.Message);
    }
    
    [Fact]
    public void createCategoryNoCategoryIconFailTest()
    {
        // Arrange
        var options = CreateDbContextOptions();
        var context = new ApplicationDbContext(options);
        
        mockCategoryTypeRepository.Setup(mc => mc.GetCategoryTypeForCertainCategory(1))
            .Returns(new CategoryType() { Id = 1, Name = "Category 1" });
        mockCategoryIconRepository.Setup(mc => mc.GetCategoryIconById(1))
            .Returns((CategoryIcon)null);
        mockCategoryColorRepository.Setup(mc => mc.GetCategoryColorFerCertainCategory(1))
            .Returns(new CategoryColor() { Id = 1, Name = "Color 1", code = "color1" });

        var categoryRepo = new CategoryRepository(
            context,
            mockCategoryTypeRepository.Object,
            mockCategoryIconRepository.Object,
            mockCategoryColorRepository.Object,
            mockTransactionRepository.Object
        );

        var category = new Category()
        {
            Title = "Category 1",
            CategoryTypeId = 1,
            CategoryIconId = 1,
            CategoryColorId = 1,
            ApplicationUserId = "user1",
        };
        
        // Act
        var exception = Assert.Throws<Exception>(() => categoryRepo.createCategory(category, "userid"));
        
        // Assert
        Assert.Equal("CategoryIcon not found.", exception.Message);
    }
    
    [Fact]
    public void createCategoryNoCategoryColorFailTest()
    {
        // Arrange
        var options = CreateDbContextOptions();
        var context = new ApplicationDbContext(options);
        
        mockCategoryTypeRepository.Setup(mc => mc.GetCategoryTypeForCertainCategory(1))
            .Returns(new CategoryType() { Id = 1, Name = "Category 1" });
        mockCategoryIconRepository.Setup(mc => mc.GetCategoryIconById(1))
            .Returns(new CategoryIcon() { Id = 1, Name = "Category 1", Code = "category1"});
        mockCategoryColorRepository.Setup(mc => mc.GetCategoryColorFerCertainCategory(1))
            .Returns((CategoryColor)null);

        var categoryRepo = new CategoryRepository(
            context,
            mockCategoryTypeRepository.Object,
            mockCategoryIconRepository.Object,
            mockCategoryColorRepository.Object,
            mockTransactionRepository.Object
        );

        var category = new Category()
        {
            Title = "Category 1",
            CategoryTypeId = 1,
            CategoryIconId = 1,
            CategoryColorId = 1,
            ApplicationUserId = "user1",
        };
        
        // Act
        var exception = Assert.Throws<Exception>(() => categoryRepo.createCategory(category, "userid"));
        
        // Assert
        Assert.Equal("CategoryColor not found.", exception.Message);
    }
    
    [Fact]
    public void createCategoryNoCategoryFailTest()
    {
        // Arrange
        var options = CreateDbContextOptions();
        var context = new ApplicationDbContext(options);
        
        mockCategoryTypeRepository.Setup(mc => mc.GetCategoryTypeForCertainCategory(1))
            .Returns(new CategoryType() { Id = 1, Name = "Category 1" });
        mockCategoryIconRepository.Setup(mc => mc.GetCategoryIconById(1))
            .Returns(new CategoryIcon() { Id = 1, Name = "Category 1", Code = "category1"});
        mockCategoryColorRepository.Setup(mc => mc.GetCategoryColorFerCertainCategory(1))
            .Returns(new CategoryColor() { Id = 1, Name = "Color 1", code = "color1" });

        var categoryRepo = new CategoryRepository(
            context,
            mockCategoryTypeRepository.Object,
            mockCategoryIconRepository.Object,
            mockCategoryColorRepository.Object,
            mockTransactionRepository.Object
        );
        
        // Act
        var exception = Assert.Throws<NullReferenceException>(() => categoryRepo.createCategory(null, "userid"));
        
        // Assert
        Assert.Equal("Object reference not set to an instance of an object.", exception.Message);
    }
    // create Category Test End
    
    // delete Category Test Start
    [Fact]
    public void deleteCategorySuccessTest()
    {
        // Arrange
        var options = CreateDbContextOptions();
        var context = new ApplicationDbContext(options);

        var (user, category, categoryType, categoryIcon, categoryColor) =
            AddDummyCategoryData.addOneCategoryWithAllRelations(context);

        var categoryRepo = new CategoryRepository(
            context, 
            mockCategoryTypeRepository.Object, 
            mockCategoryIconRepository.Object, 
            mockCategoryColorRepository.Object, 
            mockTransactionRepository.Object);

        // Act
        categoryRepo.deleteCategory(category.Id);
        
        // Assert
        var categoryDb = context.categories.FirstOrDefault();
        var categoryTypeDb = context.categoriesTypes.FirstOrDefault();
        var categoryIconDb = context.categoriesIcons.FirstOrDefault();
        var categoryColorDb = context.categoriesColors.FirstOrDefault();
        var userDb = context.applicationUsers.FirstOrDefault();
        Assert.Null(categoryDb);
        Assert.NotNull(categoryTypeDb);
        Assert.NotNull(categoryIconDb);
        Assert.NotNull(categoryColorDb);
        Assert.NotNull(userDb);
    }

    [Fact]
    public void deleteCategoryCategoryNotFoundFailTest()
    {
        // Arrange
        var options = CreateDbContextOptions();
        var context = new ApplicationDbContext(options);

        var categoryRepo = new CategoryRepository(
            context,
            mockCategoryTypeRepository.Object,
            mockCategoryIconRepository.Object,
            mockCategoryColorRepository.Object,
            mockTransactionRepository.Object
        );
        
        // Act
        // The CategoryRepositor.findCategory method will throw this exception
        var exception = Assert.Throws<Exception>(() => categoryRepo.deleteCategory(1));
        
        Assert.Equal("Couldn't find any category", exception.Message);
    }
    // delete Category Test End
    
    // update Category Test Start
    [Fact]
    public void updateCategorySuccessTest()
    {
        // Arrange
        var options = CreateDbContextOptions();
        var context = new ApplicationDbContext(options);
        
        // Act
        
        // Assert
    }
    // update Category Test End
}