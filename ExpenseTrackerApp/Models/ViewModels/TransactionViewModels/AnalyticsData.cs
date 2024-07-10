namespace ExpenseTrackerApp.Models.ViewModels.TransactionViewModels
{
    public class AnalyticsData
    {
        public decimal? DailyAverageValue;
        public decimal? TotalAmount;
        public int? TotalTransactions;
        public int? NumberOfCategories;
        public List<List<decimal>> weeklyEspenses;

        public AnalyticsData(int? totalTransactions, int? numberOfCategories, decimal? dailyAverage, decimal? totalAmount)
        {
            TotalTransactions = totalTransactions;
            NumberOfCategories = numberOfCategories;
            DailyAverageValue = dailyAverage;
            TotalAmount = totalAmount;
        }

        public AnalyticsData() { }
    }
}
