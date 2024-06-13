using ExpenseTrackerApp.Data;
using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using ExpenseTrackerApp.Models;
using ExpenseTrackerApp.Models.RepositoryModels;
using ExpenseTrackerApp.Models.ViewModels;
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
    }
}
