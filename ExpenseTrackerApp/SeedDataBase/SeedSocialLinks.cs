using ExpenseTrackerApp.Data;
using ExpenseTrackerApp.Models;

namespace ExpenseTrackerApp.SeedDataBase;

public class SeedSocialLinks
{
    private readonly ApplicationDbContext _context;

    public SeedSocialLinks(ApplicationDbContext context)
    {
        _context = context;
    }

    public void ReadCSV()
    {
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ExpenseTrackerApp", "SeedDataBase", "Data", "SocialLinks.csv");
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
                        var socialLink = new SocialLink()
                        {
                            Platform = values[1].Trim().Trim('"'),
                            Url = values[2].Trim().Trim('"'),
                            IconClass = values[3].Trim().Trim('"'),
                            FooterId = int.Parse(values[4].Trim().Trim('"'))
                        };

                        // Add to context
                        _context.socialLinks.Add(socialLink);
                    }
                }

                // Save changes after adding all items
                Console.WriteLine("!!!!!!!!!!!!!!!!!!!!! ADDED SOCIAL LINKS ENTRIES !!!!!!!!!!!!!");
                _context.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading CSV file: {ex.Message}");
        }
    }
}