using ExpenseTrackerApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ExpenseTrackerApp.Controllers
{
    public class BaseController : Controller
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var footerModel = new Footer
            {
                CopryRightHolder = "Max Zimmermann",
                Links = null
            };
            ViewData["Footer"] = footerModel;
            base.OnActionExecuted(context);
        }
    }
}
