using ExpenseTrackerApp.Data;
using ExpenseTrackerApp.Models;

namespace ExpenseTrackerApp.SeedDataBase;

public class SeedCategoryType
{
    private readonly ApplicationDbContext _context;

    public SeedCategoryType(ApplicationDbContext context)
    {
        _context = context;
    }

    public void ReadCSV()
    {
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ExpenseTrackerApp", "SeedDataBase", "Data", "CategoryType.csv");
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
                    if (values.Length >= 2)
                    {
                        var type = new CategoryType
                        {
                            Id = int.Parse(values[0].Trim()),
                            Name = values[1].Trim().Trim('"') // Remove extra spaces and quotes
                        };

                        // Add to context
                        _context.categoriesTypes.Add(type);
                    }
                }

                // Save changes after adding all items
                Console.WriteLine("!!!!!!!!!!!!!!!!!!!!! ADDED ENTRIES !!!!!!!!!!!!!");
                _context.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading CSV file: {ex.Message}");
        }
    }
}