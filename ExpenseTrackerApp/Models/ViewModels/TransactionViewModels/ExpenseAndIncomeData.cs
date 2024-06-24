namespace ExpenseTrackerApp.Models.ViewModels.TransactionViewModels
{
    public class ExpenseAndIncomeData
    {
        public List<ExpenseTrackerApp.Models.Transaction> transactions { get; set; }
        public List<ExpenseAndIncomeCategoryData> categorieDataList { get; set; }
    }
}
