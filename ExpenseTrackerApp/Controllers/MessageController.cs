using ExpenseTrackerApp.Data;
using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using ExpenseTrackerApp.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTrackerApp.Controllers
{
    [Authorize]
    public class MessageController : BaseController
    {
        private readonly IFooterRepository _footerRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IUserManageService _userManageService;

        public MessageController(IFooterRepository footerRepository,
            IMessageRepository messageRepository,
            IUserManageService userManageService) : base(footerRepository)
        {
            _footerRepository = footerRepository;
            _messageRepository = messageRepository;
            _userManageService = userManageService;
        }

        [HttpGet]
        public IActionResult Messages()
        {
            var messages = _messageRepository.GetAllMessages(_userManageService.GetCurrentUserId(User));
            return View(messages);
        }

        [HttpGet]
        public IActionResult RecentMessages()
        {
            var recentMessages = _messageRepository.GetRecentMessages(_userManageService.GetCurrentUserId(User));
            return PartialView("_RecentMessages", recentMessages);
        }
    }
}
