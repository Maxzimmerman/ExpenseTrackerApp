using ExpenseTrackerApp.Data;
using ExpenseTrackerApp.Models;
using ExpenseTrackerApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExpenseTrackerApp.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult SettingsCategory()
        {
            IEnumerable<SelectListItem> categoryTypes =
                _context.categoriesTypes.Select(ct => new SelectListItem
                {
                    Text = ct.Name,
                    Value = ct.Id.ToString()
                }).ToList();

            IEnumerable<SelectListItem> categoryIcons =
                _context.categoriesIcons.Select(ct => new SelectListItem
                {
                    Text = ct.Name,
                    Value = ct.Id.ToString()
                }).ToList();

            IEnumerable<SelectListItem> categoryColors =
                _context.categoriesColors.Select(ct => new SelectListItem
                {
                    Text = ct.Name,
                    Value = ct.Id.ToString()
                }).ToList();

            var expenses = _context.categories.Where(ex => ex.CategoryType.Name == "Expense").ToList();
            var incoms = _context.categories.Where(ex => ex.CategoryType.Name == "Incom").ToList();

            ViewBag.CategoryTypes = categoryTypes;
            ViewBag.CategoryIcons = categoryIcons;
            ViewBag.CategoryColors = categoryColors;
            ViewBag.Expenses = expenses.ToList();
            ViewBag.Incoms = incoms.ToList();

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
                _context.categories.Add(category);
                _context.SaveChanges();
                return View("SettingsCategory");
            }
        }
    }
}
