using Microsoft.EntityFrameworkCore;
using backend.api.Models;

namespace backend.api.Data
{
    public class TrainDataDbContext : DbContext
    {
        public TrainDataDbContext(DbContextOptions<TrainDataDbContext> options) : base(options) { }

        public DbSet<TrainData> TrainDatas { get; set; }
    }
}
