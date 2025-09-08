using Camply.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Camply.Persistence.Factory
{
    public class CamplyDesignTimeDbContextFactory : IDesignTimeDbContextFactory<CamplyDbContext>
    {
        public CamplyDbContext CreateDbContext(string[] args)
        {
            if (args.Length == 0)
                throw new ArgumentException("Connection string must be provided as the first argument.");

            var connectionString = args[0];

            var optionsBuilder = new DbContextOptionsBuilder<CamplyDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            var context = new CamplyDbContext(optionsBuilder.Options);

            context.Database.EnsureCreated();

            context.Database.Migrate();

            return context;
        }
    }
}