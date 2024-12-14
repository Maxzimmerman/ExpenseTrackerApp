using ExpenseTrackerApp.Data;
using ExpenseTrackerApp.Models;

namespace ExpenseTrackerApp.SeedDataBase.Data;

public class SeedFooter
{
    private readonly ApplicationDbContext _context;

    public SeedFooter(ApplicationDbContext context)
    {
        _context = context;
    }

    public void ReadCSV()
    {
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ExpenseTrackerApp", "SeedDataBase", "Data", "Footer.csv");
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
                        var footer = new Footer()
                        {
                            CopryRightHolder = values[1].Trim().Trim('"')
                        };

                        // Add to context
                        _context.footers.Add(footer);
                    }
                }

                // Save changes after adding all items
                Console.WriteLine("!!!!!!!!!!!!!!!!!!!!! ADDED FOOTER ENTRIES !!!!!!!!!!!!!");
                _context.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading CSV file: {ex.Message}");
        }
    }
}