using ExpenseTrackerApp.Data;
using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using ExpenseTrackerApp.Models;
using ExpenseTrackerApp.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTrackerApp.SeedDataBase.Data;

public class SeedCategory
{
    private readonly IUserManageService _userManageService;
    private readonly ApplicationDbContext _context;
    private List<CategoryColor> _categoryColors;
    private List<CategoryType> _categoryTypes;
    private List<CategoryIcon> _categoryIcons;

    public SeedCategory(ApplicationDbContext context, IUserManageService userManageService)
    {
        _context = context;
        _userManageService = userManageService;
    }

    public void SeedFirstDefaultUser()
    {
        var user = new ApplicationUser()
        {
            ApplicationUserName = "admin",
            Email = "admin@admin.com",
            Balance = 0,
            registeredSince = DateTime.UtcNow,
        };
        _context.Users.Add(user);
        _context.SaveChanges();
    }
    
    public void SeedCategoryTypes()
    {
        List<CategoryType> categoryTypes = new List<CategoryType>();
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ExpenseTrackerApp", "SeedDataBase", "Data", "CategoryType.csv");
        try
        {
            // Open the CSV file
            using (StreamReader reader = new StreamReader(filePath))
            {
                // Read file line by line
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();

                    // Split the line into values
                    string[] values = line.Split(',');

                    // Ensure values have the correct format
                    if (values.Length >= 2)
                    {
                        var type = new CategoryType
                        {
                            Name = values[1].Trim().Trim('"'),
                            // Remove extra spaces and quotes
                        };
                        categoryTypes.Add(type);

                        // Add to context
                        _context.categoriesTypes.Add(type);
                    }
                }
                _categoryTypes = categoryTypes;
                // Save changes after adding all items
                Console.WriteLine("!!!!!!!!!!!!!!!!!!!!! ADDED CATEGORY TYPE ENTRIES !!!!!!!!!!!!!");
                _context.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading Category Type CSV file: {ex.Message}");
        }
    }
    
    public void SeedCategoryIcons()
    {
        List<CategoryIcon> categoryIcons = new List<CategoryIcon>();
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ExpenseTrackerApp", "SeedDataBase", "Data", "CategoryIcon.csv");
        try
        {
            // Open the CSV file
            using (StreamReader reader = new StreamReader(filePath))
            {
                // Read file line by line
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();

                    // Split the line into values
                    string[] values = line.Split(',');

                    // Ensure values have the correct format
                    if (values.Length >= 3)
                    {
                        var icon = new CategoryIcon()
                        {
                            Name = values[1].Trim().Trim('"'), 
                            Code = values[2].Trim().Trim('"')// Remove extra spaces and quotes
                        };
                        
                        categoryIcons.Add(icon);

                        // Add to context
                        _context.categoriesIcons.Add(icon);
                    }
                }
                _categoryIcons = categoryIcons;
                // Save changes after adding all items
                Console.WriteLine("!!!!!!!!!!!!!!!!!!!!! ADDED CATEGORY ICON ENTRIES !!!!!!!!!!!!!");
                _context.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading Category Icon CSV file: {ex.Message}");
        }
    }
    
    public void SeedCategoryColors()
    {
        List<CategoryColor> categoryColors = new List<CategoryColor>();
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ExpenseTrackerApp", "SeedDataBase", "Data", "CategoryColor.csv");
        try
        {
            // Open the CSV file
            using (StreamReader reader = new StreamReader(filePath))
            {
                // Read file line by line
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();

                    // Split the line into values
                    string[] values = line.Split(',');

                    // Ensure values have the correct format
                    if (values.Length >= 3)
                    {
                        var color = new CategoryColor
                        {
                            Name = values[1].Trim().Trim('"'), // Remove extra spaces and quotes
                            code = values[2].Trim().Trim('"')
                        };
                        
                        categoryColors.Add(color);

                        // Add to context
                        _context.categoriesColors.Add(color);
                    }
                }
                _categoryColors = categoryColors;
                // Save changes after adding all items
                Console.WriteLine("!!!!!!!!!!!!!!!!!!!!! ADDED CATEGORY COLOR ENTRIES !!!!!!!!!!!!!");
                _context.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading Category Color CSV file: {ex.Message}");
        }
    }

    public void Seed()
    {
        this.SeedFirstDefaultUser();
        this.SeedCategoryColors();
        this.SeedCategoryIcons();
        this.SeedCategoryTypes();
        
        Console.WriteLine($"---------------------START---------------------");
        foreach (CategoryIcon categoryIcon in _categoryIcons)
        {
            Console.WriteLine($"Category Icon: {categoryIcon.Name}");
        }

        foreach (CategoryColor categoryColor in _categoryColors)
        {
            Console.WriteLine($"Category Color: {categoryColor.Name}");
        }
        Console.WriteLine($"---------------------END------------------------");
        
        var expenseTypeId = _categoryTypes.FirstOrDefault(e => e.Name == "Expense").Id; 
        var incomeTypeId = _categoryTypes.FirstOrDefault(e => e.Name == "Income").Id;
        var firstUser = _context.applicationUsers.FirstOrDefault(user => user.ApplicationUserName == "admin");
        
        string firstUserId = firstUser.Id;

        if (firstUser == null)
        {
            throw new Exception("First user is null");
        }
        
        var expenseCategory = new Category()
        {
            Title = "Expense Default Category",
            CategoryTypeId = expenseTypeId,
            CategoryIconId = _categoryIcons[0].Id,
            CategoryColorId = _categoryColors[0].Id,
            ApplicationUserId = firstUserId,
        };
        
        var incomeCategory = new Category()
        {
            Title = "Income Default Category",
            CategoryTypeId = incomeTypeId,
            CategoryIconId = _categoryIcons[1].Id,
            CategoryColorId = _categoryColors[1].Id,
            ApplicationUserId = firstUserId,
        };
        
        _context.categories.Add(expenseCategory);
        _context.categories.Add(incomeCategory);
        _context.SaveChanges();
        
        Console.WriteLine("!!!!!!!!!!!!!!!!!!!!! ADDED CATEGORY COLOR ENTRIES !!!!!!!!!!!!!");
    }
}