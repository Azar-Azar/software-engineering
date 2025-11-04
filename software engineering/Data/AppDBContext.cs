using Microsoft.EntityFrameworkCore;

namespace software_engineering.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options)
            : base(options)
        { }
        // connects the C# model Book to a table in the DB
        public DbSet<Models.Users> user { get; set; }
        




    }
}
