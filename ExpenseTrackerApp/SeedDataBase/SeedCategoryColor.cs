using ExpenseTrackerApp.Data;
using ExpenseTrackerApp.Models;

namespace ExpenseTrackerApp.SeedDataBase;

public class SeedCategoryColor
{
    private readonly ApplicationDbContext _context;

    public SeedCategoryColor(ApplicationDbContext context)
    {
        _context = context;
    }

    public void ReadCSV()
    {
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ExpenseTrackerApp", "SeedDataBase", "Data", "CategoryColor.csv");
        try
        {
            // Open the CSV file
            using (StreamReader reader = new StreamReader(filePath))
            {
                // Read file line by line
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();

                    // Split the line into values
                    string[] values = line.Split(',');

                    // Ensure values have the correct format
                    if (values.Length >= 3)
                    {
                        var color = new CategoryColor
                        {
                            Name = values[1].Trim().Trim('"'), // Remove extra spaces and quotes
                            code = values[2].Trim().Trim('"')
                        };

                        // Add to context
                        _context.categoriesColors.Add(color);
                    }
                }

                // Save changes after adding all items
                Console.WriteLine("!!!!!!!!!!!!!!!!!!!!! ADDED CATEGORY COLOR ENTRIES !!!!!!!!!!!!!");
                _context.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading CSV file: {ex.Message}");
        }
    }
}