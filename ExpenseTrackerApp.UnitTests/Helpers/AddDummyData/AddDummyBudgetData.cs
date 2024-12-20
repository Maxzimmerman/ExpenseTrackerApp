using ExpenseTrackerApp.Models;

using ExpenseTrackerApp.Data;

namespace ExpenseTrackerApp.UnitTests.Helpers.AddDummyData;

public static class AddDummyBudgetData
{
    public static ApplicationUser AddUser(ApplicationDbContext context)
    {
        var user = new ApplicationUser()
        {
            ApplicationUserName = "User1", 
            Balance = 100, 
            registeredSince = DateTime.Now
        };
            
        context.Users.Add(user);
        context.SaveChanges();

        return user;
    }

    public static Budget AddOneBudget(ApplicationDbContext context, decimal amount)
    {
        var budget = new Budget() { Id = 1, Amount = amount, CategoryId = 1 };
        context.budgets.Add(budget);
        context.SaveChanges();
        return budget;
    }

    public static Budget AddOneBudgetWithAllRelations(ApplicationDbContext context)
    {
        var budget = new Budget
        {
            Id = 1,
            Amount = 100,
            Category = new Category
            {
                Title = "Category1",
                ApplicationUserId = "user1",
                CategoryColor = new CategoryColor { Id = 1, code = "code1", Name = "name1" },
                CategoryIcon = new CategoryIcon { Id = 1, Code = "code1", Name = "name1" },
                CategoryType = new CategoryType { Id = 1, Name = "name1" },
                ApplicationUser = new ApplicationUser { Balance = 1, ApplicationUserName = "user1" }
            }
        };

        context.budgets.Add(budget);
        context.SaveChanges();
        return budget;
    }

    public static (ApplicationUser, List<Budget>) AddBudgetsWithAllRelations(ApplicationDbContext context, decimal amount)
    {
        var user = new ApplicationUser()
        {
            ApplicationUserName = "User1", 
            Balance = 100, 
            registeredSince = DateTime.Now
        };

        var category = new Category()
        {
            Id = 1,
            Title = "Category 1",
            CategoryIcon = new CategoryIcon() { Id = 1, Name = "Icon", Code = "icon"},
            CategoryColor = new CategoryColor() { Id = 1, Name = "Color", code = "color" },
            CategoryType = new CategoryType() { Id = 1, Name = "Type" },
            ApplicationUserId = user.Id,
        };

        var budgets = new List<Budget>()
        {
            new Budget() { Id = 1, Amount = amount, CategoryId = category.Id },
            new Budget() { Id = 2, Amount = amount, CategoryId = category.Id },
        };
            
        context.Users.Add(user);
        context.categories.Add(category);
        context.budgets.AddRange(budgets);
        context.SaveChanges();
        
        return (user, budgets);
    }
    
    public static (ApplicationUser, List<Budget>) AddBudgetWithAllRelations(ApplicationDbContext context, decimal amount)
    {
        var user = new ApplicationUser()
        {
            ApplicationUserName = "User1", 
            Balance = amount, 
            registeredSince = DateTime.Now
        };

        var category = new Category()
        {
            Id = 1,
            Title = "Category 1",
            CategoryIcon = new CategoryIcon() { Id = 1, Name = "Icon", Code = "icon"},
            CategoryColor = new CategoryColor() { Id = 1, Name = "Color", code = "color" },
            CategoryType = new CategoryType() { Id = 1, Name = "Type" },
            ApplicationUserId = user.Id,
        };

        var budgets = new List<Budget>()
        {
            new Budget() { Id = 1, Amount = amount, CategoryId = category.Id },
        };
            
        context.Users.Add(user);
        context.categories.Add(category);
        context.budgets.AddRange(budgets);
        context.SaveChanges();
        
        return (user, budgets);
    }
}