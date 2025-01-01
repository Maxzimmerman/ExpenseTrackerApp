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

namespace ExpenseTrackerApp.UnitTests.Helpers.AddDummyData;

public static class AddDummyCategoryData
{
    public static (
        ApplicationUser, 
        List<Category>, 
        List<CategoryType>, 
        List<CategoryIcon>, 
        List<CategoryColor>) AddCategoriesWithAllRelations(ApplicationDbContext context)
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
        
        return (user, categories, categoryTypes, categoryIcons, categoryColors);
    }

    public static (
        ApplicationUser, 
        Category, 
        CategoryType, 
        CategoryIcon, 
        CategoryColor) addOneCategoryWithAllRelations(ApplicationDbContext context)
    {
        var user = new ApplicationUser()
        {
            ApplicationUserName = "User1", 
            Balance = 100, 
            registeredSince = DateTime.Now
        };
        
        var categoryType = new CategoryType() { Name = "Expense" };
        var categoryIcon = new CategoryIcon() { Name = "CategoryIcon 1", Code = "1" };
        var categoryColor = new CategoryColor() { Name = "CategoryColor 1", code = "1" };
        var category = new Category()
        {
            Title = "Category 1", 
            ApplicationUser = user, 
            CategoryType = categoryType,
            CategoryIcon = categoryIcon, 
            CategoryColor = categoryColor,
        };
            
        context.Users.Add(user);
        context.categories.AddRange(category);
        context.categoriesTypes.Add(categoryType);
        context.categoriesIcons.Add(categoryIcon);
        context.categoriesColors.Add(categoryColor);
        context.SaveChanges();
        
        return (user, category, categoryType, categoryIcon, categoryColor);
    }
}