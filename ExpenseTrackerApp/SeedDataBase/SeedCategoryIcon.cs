using ExpenseTrackerApp.Data;
using ExpenseTrackerApp.Models;

namespace ExpenseTrackerApp.SeedDataBase;

public class SeedCategoryIcon
{
    
    private readonly ApplicationDbContext _context;

    public SeedCategoryIcon(ApplicationDbContext context)
    {
        _context = context;
    }

    public void ReadCSV()
    {
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ExpenseTrackerApp", "SeedDataBase", "Data", "CategoryIcon.csv");
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
                        var icon = new CategoryIcon()
                        {
                            Name = values[1].Trim().Trim('"'), 
                            Code = values[2].Trim().Trim('"')// Remove extra spaces and quotes
                        };

                        // Add to context
                        _context.categoriesIcons.Add(icon);
                    }
                }

                // Save changes after adding all items
                Console.WriteLine("!!!!!!!!!!!!!!!!!!!!! ADDED CATEGORY ICON ENTRIES !!!!!!!!!!!!!");
                _context.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading CSV file: {ex.Message}");
        }
    }
}