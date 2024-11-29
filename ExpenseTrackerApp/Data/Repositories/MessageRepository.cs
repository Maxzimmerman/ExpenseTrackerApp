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
        
        public void CreateMessageWithUserId(string userId, string description)
        {
            Message message = new Message();
            message.ApplicationUserId = userId;
            message.Description = description;
            message.Date = DateTime.Now;

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
    }
}
