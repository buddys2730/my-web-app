using Microsoft.EntityFrameworkCore;
using MyProject.Models;  // ← TrainingData の名前空間を追加

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<TrainingData> TrainingData { get; set; }
}
