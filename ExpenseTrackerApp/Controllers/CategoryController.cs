﻿using ExpenseTrackerApp.Data;
using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using ExpenseTrackerApp.Models;
using ExpenseTrackerApp.Models.ViewModels.CategoryViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ExpenseTrackerApp.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICategoryRepository _categoryRepository;
        private readonly UserManager<IdentityUser> _userManager;

        public CategoryController(ApplicationDbContext context, ICategoryRepository categoryRepository, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _categoryRepository = categoryRepository;
            _userManager = userManager;
        }

        public async Task<IActionResult> SettingsCategory()
        {
            var currentUser = await _userManager.FindByIdAsync(User.Claims.First().Value);
            var currentUserId = currentUser.Id;

            AddCategory addCategory = _categoryRepository.addCategoryData(currentUserId);

            ViewBag.CategoryTypes = addCategory.CategoryTypes;
            ViewBag.CategoryIcons = addCategory.CategoryIcons;
            ViewBag.CategoryColors = addCategory.CategoryColors;
            ViewBag.Expenses = addCategory.Expenses.ToList();
            ViewBag.Incoms = addCategory.Incomes.ToList();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(Category category)
        {
            if(category == null)
            {
                return RedirectToAction("BadRequest");
            }
            else
            {
                var currentUser = await _userManager.FindByIdAsync(User.Claims.First().Value);
                var currentUserId = currentUser.Id;

                _categoryRepository.createCategory(category, currentUserId);

                return RedirectToAction("Home", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> LoadUpdateCategory(int id)
        {
            var currentUser = await _userManager.FindByIdAsync(User.Claims.First().Value);
            var currentUserId = currentUser.Id;

            var category = _categoryRepository.findCategory(id);

            AddCategory addCategory = _categoryRepository.addCategoryData(currentUserId);

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