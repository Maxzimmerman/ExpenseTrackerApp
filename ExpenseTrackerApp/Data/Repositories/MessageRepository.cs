using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using ExpenseTrackerApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerApp.Data.Repositories
{
    public class MessageRepository : Repository<Message>, IMessageRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public MessageRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public bool ContainsMessageThisMonth(string userId, string message, int year, int month)
        {
            var currentDate = DateTime.Now;
            return _applicationDbContext.messages
                .Include(m => m.ApplicationUser)
                .Any(m => m.ApplicationUserId == userId
                && m.Description == message
                && m.Date.Year == year
                && m.Date.Month == month);
        }


        public List<Message> GetAllMessages(string userId)
        {
            return _applicationDbContext.messages
                .Include(m => m.ApplicationUser)
                .Where(m => m.ApplicationUser.Id == userId)
                .OrderByDescending(m => m.Date)
                .ToList();
        }

        public List<Message> GetRecentMessages(string userId)
        {
            int currentMonth = DateTime.Now.Month;
            int currentYear = DateTime.Now.Year;
            return _applicationDbContext.messages
                .Include(m => m.ApplicationUser)
                .Where(m => m.ApplicationUser.Id == userId
                && m.Date.Year == currentYear
                && m.Date.Month == currentMonth)
                .OrderByDescending(m => m.Date)
                .ToList();
        }

        public Message? GetById(int id)
        {
            return _applicationDbContext.messages
                .Include(m => m.ApplicationUser)
                .FirstOrDefault(m => m.Id == id);
        }
        
        public void CreateMessageWithUserId(string userId, 
            string description, 
            string backgroundColor, 
            string iconType, 
            string linkController, 
            string linkAction)
        {
            Message message = new Message();
            message.ApplicationUserId = userId;
            message.Description = description;
            message.Date = DateTime.Now;
            message.IconBackground = backgroundColor;
            message.IconType = iconType;
            message.ControllerLink = linkController;
            message.ActionLink = linkAction;

            _applicationDbContext.messages.Add(message);
            _applicationDbContext.SaveChanges();
        }

        public void DeleteMessage(Message message)
        {
            if(message != null)
            {
                _applicationDbContext.messages.Remove(message);
                _applicationDbContext.SaveChanges();
            }
            throw new Exception("message was null");
        }

        public List<Message> GetMessageCreateInTheCurrentMinute(string userId)
        {
            DateTime now = DateTime.Now;
            return _applicationDbContext.messages
                .Where(m => m.ApplicationUserId == userId
                && m.Date.Year == now.Year
                && m.Date.Month == now.Month
                && m.Date.Hour == now.Hour
                && m.Date.Minute == now.Minute)
                .ToList();
        }
    }
}
