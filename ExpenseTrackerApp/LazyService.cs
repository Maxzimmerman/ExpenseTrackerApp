namespace ExpenseTrackerApp
{
    public class LazyService<T> : Lazy<T>
    {
        public LazyService(IServiceProvider serviceProvider) : base(() => serviceProvider.GetRequiredService<T>()) { }
    }
}
