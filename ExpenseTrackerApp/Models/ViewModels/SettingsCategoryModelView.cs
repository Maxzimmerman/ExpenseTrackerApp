namespace ExpenseTrackerApp.Models.ViewModels
{
    public class SettingsCategoryModelView
    {
        public List<Category> Expenses { get; set; }
        public List<Category> Incoms { get; set; }
        public Category SampleCategory { get; set; }
    }
}
