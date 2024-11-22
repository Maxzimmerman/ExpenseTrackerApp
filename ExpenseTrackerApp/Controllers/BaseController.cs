using ExpenseTrackerApp.Data;
using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using ExpenseTrackerApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerApp.Controllers
{
    public class BaseController : Controller
    {
        private readonly IFooterRepository _footerRepository;

        public BaseController(IFooterRepository footerRepository)
        {
            _footerRepository = footerRepository;
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var footerModel = _footerRepository.GetFooter();
            ViewData["Footer"] = footerModel;
            base.OnActionExecuted(context);
        }
    }
}
