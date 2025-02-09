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
        using (var context = new ApplicationDbContext(options))
        {
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
    }
    // delete Category Test End
    
    // update Category Test Start
    [Fact]
    public void updateCategorySuccessTest()
    {
        // Arrange
        var options = CreateDbContextOptions(); // Ensure this creates a unique, isolated in-memory DB.
        using (var context = new ApplicationDbContext(options))
        {
            var (user, category, categoryType, categoryIcon, categoryColor) =
                AddDummyCategoryData.addOneCategoryWithAllRelations(context);

            var categoryRepo = new CategoryRepository(
                context,
                mockCategoryTypeRepository.Object,
                mockCategoryIconRepository.Object,
                mockCategoryColorRepository.Object,
                mockTransactionRepository.Object
            );

            var updatedCategory = new Category
            {
                Id = category.Id,
                Title = "Updated Category",
                ApplicationUser = user,
                ApplicationUserId = user.Id,
                CategoryType = categoryType,
                CategoryTypeId = categoryType.Id,
                CategoryIcon = categoryIcon,
                CategoryIconId = categoryIcon.Id,
                CategoryColor = categoryColor,
                CategoryColorId = categoryColor.Id
            };

            // Act
            categoryRepo.updateCategory(updatedCategory);

            // Assert
            var categoryDb = context.categories
                .Include(c => c.CategoryType)
                .Include(c => c.CategoryIcon)
                .Include(c => c.CategoryColor)
                .Include(c => c.ApplicationUser)
                .FirstOrDefault(c => c.Id == updatedCategory.Id);

            categoryDb.Should().NotBeNull();
            categoryDb.Should().BeEquivalentTo(updatedCategory, options => options
                .Excluding(c => c.CategoryType) // Exclude navigation properties to avoid lazy-loading issues
                .Excluding(c => c.CategoryIcon)
                .Excluding(c => c.CategoryColor)
                .Excluding(c => c.ApplicationUser));
        }
    }
    
    [Fact]
    public void updateCategoryCategoryIsNullFailTest()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using (var context = new ApplicationDbContext(options))
        {
            var CategoryRepo = new CategoryRepository(
                context,
                mockCategoryTypeRepository.Object,
                mockCategoryIconRepository.Object,
                mockCategoryColorRepository.Object,
                mockTransactionRepository.Object
            );

            // Act
            var exception = Assert.Throws<ArgumentNullException>(() => CategoryRepo.updateCategory(null));
        
            // Assert
            Assert.Equal(typeof(ArgumentNullException), exception.GetType());
        }
    }
    // update Category Test End
    
    // find category test start
    [Fact]
    public void findCategorySuccessTest()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using (var context = new ApplicationDbContext(options))
        {
            var (user, category, categoryType, categoryIcon, categoryColor) =
                AddDummyCategoryData.addOneCategoryWithAllRelations(context);

            var categoryRepo = new CategoryRepository
            (
                context,
                mockCategoryTypeRepository.Object,
                mockCategoryIconRepository.Object,
                mockCategoryColorRepository.Object,
                mockTransactionRepository.Object
            );
            
            // Act
            var foundCategory = categoryRepo.findCategory(category.Id);

            // Assert
            Assert.NotNull(foundCategory);
            foundCategory.Should().BeEquivalentTo(category);
        }
    }
    
    [Fact]
    public void findCategoryNoCategoryFailTest()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using (var context = new ApplicationDbContext(options))
        {
            var categoryRepo = new CategoryRepository
            (
                context,
                mockCategoryTypeRepository.Object,
                mockCategoryIconRepository.Object,
                mockCategoryColorRepository.Object,
                mockTransactionRepository.Object
            );
            
            // Act
            var exception = Assert.Throws<Exception>(() => categoryRepo.findCategory(1));
            
            // Assert
            Assert.Equal("Couldn't find any category", exception.Message);
        }
    }
    // find category test end
    
    // get all categories start
    [Fact]
    public void getAllCategoriesSuccessTest()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using (var context = new ApplicationDbContext(options))
        {
            var (user, categories, categoryTypes, categoryIcons, categoryColors) =
                AddDummyCategoryData.AddCategoriesWithAllRelations(context);

            var categoryRepo = new CategoryRepository(
                context,
                mockCategoryTypeRepository.Object,
                mockCategoryIconRepository.Object,
                mockCategoryColorRepository.Object,
                mockTransactionRepository.Object
                );
            
            // Act
            var resultCategories = categoryRepo.GetAllCategories(user.Id);

            // Assert 
            Assert.NotNull(resultCategories);
            Assert.NotEmpty(resultCategories);
            resultCategories.Should().BeEquivalentTo(categories);
        }
    }

    [Fact]
    public void getAllCategoriesNoCategoryTest()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using (var context = new ApplicationDbContext(options))
        {
            var categoryRepo = new CategoryRepository(
                context,
                mockCategoryTypeRepository.Object,
                mockCategoryIconRepository.Object,
                mockCategoryColorRepository.Object,
                mockTransactionRepository.Object
                );
            
            // Act
            var resultCategories = categoryRepo.GetAllCategories("testid");
            
            // Assert
            Assert.NotNull(resultCategories);
            Assert.Empty(resultCategories);
        }
    }
    // get all categories end
    
    // get all categories as selectListItems start
    [Fact]
    public void getAllCategoriesAsSelectListItemsSuccessTest()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using (var context = new ApplicationDbContext(options))
        {
            var categoryRepo = new CategoryRepository(
                context,
                mockCategoryTypeRepository.Object,
                mockCategoryIconRepository.Object,
                mockCategoryColorRepository.Object,
                mockTransactionRepository.Object
            );
            
            var (user, categories, categoryTypes, categoryIcons, categoryColors) =
                AddDummyCategoryData.AddCategoriesWithAllRelations(context);
            
            var expectedResult = categories.Select(e => new SelectListItem()
            {
                Text = e.Title,
                Value = e.Id.ToString(),
            });
            
            // Act
            var resultCategories = categoryRepo.GetAllCategoriesAsSelectListItems(user.Id);
            
            // Assert
            Assert.NotNull(resultCategories);
            Assert.NotEmpty(resultCategories);
            resultCategories.Should().BeEquivalentTo(expectedResult);
        }
    }

    [Fact]
    public void getAllCategoriesAsSelectListItemsNoCategoriesTest()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using (var context = new ApplicationDbContext(options))
        {
            var categoryRepo = new CategoryRepository(
                context,
                mockCategoryTypeRepository.Object,
                mockCategoryIconRepository.Object,
                mockCategoryColorRepository.Object,
                mockTransactionRepository.Object
            );
            
            // Act
            var resultCategories = categoryRepo.GetAllCategoriesAsSelectListItems("testid");

            // Assert
            Assert.NotNull(resultCategories);
            Assert.Empty(resultCategories);
        }
    }
    // get all categories as selectListItems end
    
    // get all expense categories as selectListItems start
    [Fact]
    public void getAllExpenseCategoriesAsSelectListItemsSuccess()
    {
        // Arrange
        var options = CreateDbContextOptions();
        
        using (var context = new ApplicationDbContext(options))
        {
            var user = new ApplicationUser()
            {
                ApplicationUserName = "User1", 
                Balance = 100, 
                registeredSince = DateTime.Now
            };

            var categoryTypes = new List<CategoryType>()
            {
                new CategoryType() { Name = "Expense" },
                new CategoryType() { Name = "Income" },
            };

            var categoryIcons = new List<CategoryIcon>()
            {
                new CategoryIcon() { Name = "CategoryIcon 1", Code = "1" },
                new CategoryIcon() { Name = "CategoryIcon 2", Code = "2" },
            };

            var categoryColors = new List<CategoryColor>()
            {
                new CategoryColor() { Name = "CategoryColor 1", code = "1" },
                new CategoryColor() { Name = "CategoryColor 2", code = "2" },
            };

            var categories = new List<Category>()
            {
                new Category() { Title = "Category 1", ApplicationUser = user, CategoryType = categoryTypes[0], CategoryIcon = categoryIcons[0], CategoryColor = categoryColors[0] },
                new Category() { Title = "Category 2", ApplicationUser = user, CategoryType = categoryTypes[1], CategoryIcon = categoryIcons[1], CategoryColor = categoryColors[1] },
            };
            
            context.Users.Add(user);
            context.categories.AddRange(categories);
            context.categoriesTypes.AddRange(categoryTypes);
            context.categoriesIcons.AddRange(categoryIcons);
            context.categoriesColors.AddRange(categoryColors);
            context.SaveChanges();

            var categoryRepo = new CategoryRepository(
                context,
                mockCategoryTypeRepository.Object,
                mockCategoryIconRepository.Object,
                mockCategoryColorRepository.Object,
                mockTransactionRepository.Object
            );

            var expectedResult = new List<SelectListItem>()
            {
                new SelectListItem() { Text = categories[0].Title, Value = categoryIcons[0].Id.ToString() },
            };
            
            // Act
            var resultCategories = categoryRepo.GetAllExpenseCategoriesAsSelectListItems(user.Id);

            // Assert 
            Assert.NotNull(resultCategories);
            Assert.NotEmpty(resultCategories);
            resultCategories.Should().BeEquivalentTo(categories);
        }
    }
    
    [Fact]
    public void getAllExpenseCategoriesAsSelectListItemsSuccessNoData()
    {
        // Arrange
        var options = CreateDbContextOptions();
        
        using (var context = new ApplicationDbContext(options))
        {
            var categoryRepo = new CategoryRepository(
                context,
                mockCategoryTypeRepository.Object,
                mockCategoryIconRepository.Object,
                mockCategoryColorRepository.Object,
                mockTransactionRepository.Object
            );
            
            // Act
            var resultCategories = categoryRepo.GetAllExpenseCategoriesAsSelectListItems("userid");

            // Assert 
            Assert.NotNull(resultCategories);
            Assert.Empty(resultCategories);
        }
    }
    
    // get all expense categories as selectListItems end
    
    // get all expense categories with transaction start
    [Fact]
    public void getAllExpenseCategoriesSuccessTest()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using (var context = new ApplicationDbContext(options))
        {
            var categoryRepo = new CategoryRepository(
                context,
                mockCategoryTypeRepository.Object,
                mockCategoryIconRepository.Object,
                mockCategoryColorRepository.Object,
                mockTransactionRepository.Object
            );
            
            var (user, categories, categoryTypes, categoryIcons, categoryColors) =
                AddDummyCategoryData.AddCategoriesWithAllRelations(context);
            
            // Act
            var resultCategories = categoryRepo.GetAllExpenseCategories(user.Id);
            
            Assert.NotNull(resultCategories);
            Assert.Equal(1, resultCategories.Count());
        }
    }
    
    [Fact]
    public void getAllExpenseCategoriesNoExpensesSuccessTest()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using (var context = new ApplicationDbContext(options))
        {
            var categoryRepo = new CategoryRepository(
                context,
                mockCategoryTypeRepository.Object,
                mockCategoryIconRepository.Object,
                mockCategoryColorRepository.Object,
                mockTransactionRepository.Object
            );
            
            var (user, categories, categoryTypes, categoryIcons, categoryColors) =
                AddDummyCategoryData.AddCategoriesWithAllRelationsNoExpensesNorIncomes(context);
            
            // Act
            var resultCategories = categoryRepo.GetAllExpenseCategories(user.Id);
            
            Assert.NotNull(resultCategories);
            Assert.Empty(resultCategories);
        }
    }
    
    [Fact]
    public void getAllExpenseCategoriesNoCategoriesSuccessTest()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using (var context = new ApplicationDbContext(options))
        {
            var categoryRepo = new CategoryRepository(
                context,
                mockCategoryTypeRepository.Object,
                mockCategoryIconRepository.Object,
                mockCategoryColorRepository.Object,
                mockTransactionRepository.Object
            );
            
            // Act
            var resultCategories = categoryRepo.GetAllExpenseCategories("userid");
            
            Assert.NotNull(resultCategories);
            Assert.Empty(resultCategories);
        }
    }
    // get all expense categories with transaction end
    
    // get all incomes categories with transaction start
    [Fact]
    public void getAllIncomeCategoriesSuccessTest()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using (var context = new ApplicationDbContext(options))
        {
            var categoryRepo = new CategoryRepository(
                context,
                mockCategoryTypeRepository.Object,
                mockCategoryIconRepository.Object,
                mockCategoryColorRepository.Object,
                mockTransactionRepository.Object
            );
            
            var (user, categories, categoryTypes, categoryIcons, categoryColors) =
                AddDummyCategoryData.AddCategoriesWithAllRelations(context);
            
            // Act
            var resultCategories = categoryRepo.GetAllIncomeCategories(user.Id);
            
            Assert.NotNull(resultCategories);
            Assert.Equal(1, resultCategories.Count());
        }
    }
    
    [Fact]
    public void getAllIncomeCategoriesNoExpensesSuccessTest()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using (var context = new ApplicationDbContext(options))
        {
            var categoryRepo = new CategoryRepository(
                context,
                mockCategoryTypeRepository.Object,
                mockCategoryIconRepository.Object,
                mockCategoryColorRepository.Object,
                mockTransactionRepository.Object
            );
            
            var (user, categories, categoryTypes, categoryIcons, categoryColors) =
                AddDummyCategoryData.AddCategoriesWithAllRelationsNoExpensesNorIncomes(context);
            
            // Act
            var resultCategories = categoryRepo.GetAllIncomeCategories(user.Id);
            
            Assert.NotNull(resultCategories);
            Assert.Empty(resultCategories);
        }
    }
    
    [Fact]
    public void getAllIncomeCategoriesNoCategoriesSuccessTest()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using (var context = new ApplicationDbContext(options))
        {
            var categoryRepo = new CategoryRepository(
                context,
                mockCategoryTypeRepository.Object,
                mockCategoryIconRepository.Object,
                mockCategoryColorRepository.Object,
                mockTransactionRepository.Object
            );
            
            // Act
            var resultCategories = categoryRepo.GetAllIncomeCategories("userid");
            
            Assert.NotNull(resultCategories);
            Assert.Empty(resultCategories);
        }
    }
    // get all incomes categories with transaction end
    
    // get total amount of all categories start
    [Fact]
    public void getTotalAmountOfCatecoriesNoCategoriesSuccessTest()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using (var context = new ApplicationDbContext(options))
        {
            string expenseOrIncome = "None";
            string userId = "userid";
            
            var mockTransactionRepository = new Mock<ITransactionRepository>();
            
            mockTransactionRepository
                .Setup(m => m.GetTotalAmountForAllCategories(userId, expenseOrIncome))
                .Returns(0);

            var lazyMockTransactionRepository = new Lazy<ITransactionRepository>(() => mockTransactionRepository.Object);

            var categoryRepo = new CategoryRepository(
                context,
                mockCategoryTypeRepository.Object,
                mockCategoryIconRepository.Object,
                mockCategoryColorRepository.Object,
                lazyMockTransactionRepository
            );
            
            // Act

            var resultAmount = categoryRepo.GetTotalAmountOfAllCategories(userId, expenseOrIncome);
            
            // Assert
            Assert.Equal(0, resultAmount);
        }
    }
    
    [Fact]
    public void getTotalAmountOfCatecoriesSuccessTest()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using (var context = new ApplicationDbContext(options))
        {
            string expenseOrIncome = "None";
            
            var (user, categories, categoryTypes, categoryIcons, categoryColors) =
                AddDummyCategoryData.AddCategoriesWithAllRelationsNoExpensesNorIncomes(context);
            
            var mockTransactionRepository = new Mock<ITransactionRepository>();
            
            mockTransactionRepository
                .Setup(m => m.GetTotalAmountForAllCategories(user.Id, expenseOrIncome))
                .Returns(100);

            var lazyMockTransactionRepository = new Lazy<ITransactionRepository>(() => mockTransactionRepository.Object);

            var categoryRepo = new CategoryRepository(
                context,
                mockCategoryTypeRepository.Object,
                mockCategoryIconRepository.Object,
                mockCategoryColorRepository.Object,
                lazyMockTransactionRepository
            );
            
            // Act

            var resultAmount = categoryRepo.GetTotalAmountOfAllCategories(user.Id, expenseOrIncome);
            
            // Assert
            Assert.Equal(100, resultAmount);
        }
    }
    // get total amount of all categories end
    
    // count all categories for user start
    [Fact]
    public void countAllCategoriesForUserSuccessTest()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using (var context = new ApplicationDbContext(options))
        {
            var categoryRepo = new CategoryRepository(
                context,
                mockCategoryTypeRepository.Object,
                mockCategoryIconRepository.Object,
                mockCategoryColorRepository.Object,
                mockTransactionRepository.Object
            );
            
            var (user, categories, categoryTypes, categoryIcons, categoryColors) =
                AddDummyCategoryData.AddCategoriesWithAllRelationsNoExpensesNorIncomes(context);
            
            // Act
            var resultCateroyCount = categoryRepo.CountAllCategoriesForUser(user.Id);
            
            // Assert
            Assert.Equal(categories.Count(), resultCateroyCount);
        }
    }
    
    [Fact]
    public void countAllCategoriesForUserNoCategoriesSuccessTest()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using (var context = new ApplicationDbContext(options))
        {
            var categoryRepo = new CategoryRepository(
                context,
                mockCategoryTypeRepository.Object,
                mockCategoryIconRepository.Object,
                mockCategoryColorRepository.Object,
                mockTransactionRepository.Object
            );
            
            // Act
            var resultCateroyCount = categoryRepo.CountAllCategoriesForUser("userid");
            
            // Assert
            Assert.Equal(0, resultCateroyCount);
        }
    }
    
    [Fact]
    public void countAllCategoriesForUserNoUserBelongingToACategorySuccessTest()
    {
        // Arrange
        var options = CreateDbContextOptions();
        using (var context = new ApplicationDbContext(options))
        {
            var categoryRepo = new CategoryRepository(
                context,
                mockCategoryTypeRepository.Object,
                mockCategoryIconRepository.Object,
                mockCategoryColorRepository.Object,
                mockTransactionRepository.Object
            );
            
            var (user, categories, categoryTypes, categoryIcons, categoryColors) =
                AddDummyCategoryData.AddCategoriesWithAllRelationsNoExpensesNorIncomes(context);
            
            // Act
            var resultCateroyCount = categoryRepo.CountAllCategoriesForUser("userid");
            
            // Assert
            Assert.Equal(0, resultCateroyCount);
        }
    }
    // count all categories for user end
}