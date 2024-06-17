using ExpenseTrackerApp.Data;
using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using ExpenseTrackerApp.Models;
using ExpenseTrackerApp.Models.ViewModels.CategoryViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ExpenseTrackerApp.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(ApplicationDbContext context, ICategoryRepository categoryRepository)
        {
            _context = context;
            _categoryRepository = categoryRepository;
        }

        public IActionResult SettingsCategory()
        {
            AddCategory addCategory = _categoryRepository.addCategoryData();

            ViewBag.CategoryTypes = addCategory.CategoryTypes;
            ViewBag.CategoryIcons = addCategory.CategoryIcons;
            ViewBag.CategoryColors = addCategory.CategoryColors;
            ViewBag.Expenses = addCategory.Expenses.ToList();
            ViewBag.Incoms = addCategory.Incomes.ToList();

            return View();
        }

        [HttpPost]
        public IActionResult AddCategory(Category category)
        {
            if(category == null)
            {
                return RedirectToAction("BadRequest");
            }
            else
            {
                var currentUser = (ClaimsIdentity)User.Identity;
                var currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;

                _categoryRepository.createCategory(category, currentUserId);

                return RedirectToAction("Home", "Home");
            }
        }

        [HttpGet]
        public IActionResult LoadUpdateCategory(int id)
        {
            var category = _categoryRepository.findCategory(id);

            AddCategory addCategory = _categoryRepository.addCategoryData();

            ViewBag.CategoryTypes = addCategory.CategoryTypes;
            ViewBag.CategoryIcons = addCategory.CategoryIcons;
            ViewBag.CategoryColors = addCategory.CategoryColors;
            ViewBag.Expenses = addCategory.Expenses.ToList();
            ViewBag.Incoms = addCategory.Incomes.ToList();
            ViewBag.LoadedCategory = category;

            return View("SettingsCategory", category);
        }

        [HttpPost]
        public IActionResult UpdateCategory(Category category)
        {
            _categoryRepository.updateCategory(category);
            return RedirectToAction("SettingsCategory");
        }

        [HttpGet]
        public IActionResult DeleteCategory(int id)
        {
            _categoryRepository.deleteCategory(id);
            return RedirectToAction("SettingsCategory");
        }
    }
}