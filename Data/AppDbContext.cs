using JEMP_API_HeyGIA.Modelos;
using Microsoft.EntityFrameworkCore;

namespace JEMP_API_HeyGIA.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Ruleta> Ruletas { get; set; }
        public DbSet<Apuesta> Apuestas { get; set; }
    }
}
