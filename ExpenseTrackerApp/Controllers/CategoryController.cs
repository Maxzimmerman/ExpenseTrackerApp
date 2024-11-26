using ExpenseTrackerApp.Data;
using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using ExpenseTrackerApp.Models;
using ExpenseTrackerApp.Models.ViewModels.CategoryViewModels;
using ExpenseTrackerApp.Services;
using ExpenseTrackerApp.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ExpenseTrackerApp.Controllers
{
    [Authorize]
    public class CategoryController : BaseController
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUserManageService _userManageService;

        public CategoryController(IFooterRepository footerRepository, 
            ICategoryRepository categoryRepository, 
            IUserManageService userManageService) : base(footerRepository)
        {
            _categoryRepository = categoryRepository;
            _userManageService = userManageService;
        }

        [HttpGet]
        public IActionResult SettingsCategory()
        {
            AddCategory addCategory = _categoryRepository.addCategoryData(_userManageService.GetCurrentUserId(User));

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
                _categoryRepository.createCategory(category, _userManageService.GetCurrentUserId(User));

                return RedirectToAction("Home", "Home");
            }
        }

        [HttpGet]
        public IActionResult LoadUpdateCategory(int id)
        {
            var category = _categoryRepository.findCategory(id);

            AddCategory addCategory = _categoryRepository.addCategoryData(_userManageService.GetCurrentUserId(User));

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