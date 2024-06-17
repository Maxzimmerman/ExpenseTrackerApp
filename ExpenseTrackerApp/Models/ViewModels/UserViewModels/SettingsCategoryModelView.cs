namespace ExpenseTrackerApp.Models.ViewModels.UserViewModels
{
    public class SettingsCategoryModelView
    {
        public List<Category> Expenses { get; set; }
        public List<Category> Incoms { get; set; }
        public Category SampleCategory { get; set; }
    }
}
