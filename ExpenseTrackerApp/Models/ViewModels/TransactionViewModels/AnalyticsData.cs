namespace ExpenseTrackerApp.Models.ViewModels.TransactionViewModels
{
    public class AnalyticsData
    {
        public double? DailyAverageValue;
        public double? ChangeValue;
        public int? TotalTransactions;
        public int? NumberOfCategories;

        public AnalyticsData(int? totalTransactions, int? numberOfCategories)
        {
            TotalTransactions = totalTransactions;
            NumberOfCategories = numberOfCategories;
        }

        public AnalyticsData() { }
    }
}
