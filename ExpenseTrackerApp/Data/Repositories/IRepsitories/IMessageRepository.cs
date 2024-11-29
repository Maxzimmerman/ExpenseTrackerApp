using ExpenseTrackerApp.Models;

namespace ExpenseTrackerApp.Data.Repositories.IRepsitories
{
    public interface IMessageRepository : IRepository<Message>
    {
        List<Message> GetAllMessages(string userId);
        List<Message> GetRecentMessages(string userId);
        Message? GetById(int id);
        void CreateMessageWithUserId(string userId, string description);
        void DeleteMessage(Message message);
    }
}
