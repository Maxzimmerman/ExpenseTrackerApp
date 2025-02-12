using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTrackerApp.Controllers;

[Authorize]
public class WalletsController : BaseController
{
    private readonly IFooterRepository _footerRepository;
    
    public WalletsController(IFooterRepository footerRepository) : base(footerRepository)
    {
        
    }

    [HttpGet]
    public IActionResult Wallets()
    {
        return View();
    }
}