using ExpenseTrackerApp.Models;

namespace ExpenseTrackerApp.Data.Repositories.IRepsitories
{
    public interface IMessageRepository : IRepository<Message>
    {
        List<Message> GetAllMessages(string userId);
        List<Message> GetRecentMessages(string userId);
        Message? GetById(int id);
        void CreateMessageWithUserId(string userId, 
            string description, string backgroundColor, 
            string iconType, 
            string linkController, 
            string linkAction);
        void DeleteMessage(Message message);
        bool ContainsMessageThisMonth(string userId, string message, int year, int month);
        List<Message> GetMessageCreateInTheCurrentMinute(string userId);
    }
}
