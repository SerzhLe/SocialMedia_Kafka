using Microsoft.EntityFrameworkCore;

namespace Post.Query.Infrastructure.DataAccess
{
    public class DataContextFactory
    {
        private readonly Action<DbContextOptionsBuilder> _configureDbContext;

        public DataContextFactory(Action<DbContextOptionsBuilder> configureDbContext)
        {
            _configureDbContext = configureDbContext;
        }

        public DataContext CreateDataContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<DataContext>();

            _configureDbContext(optionsBuilder);

            return new DataContext(optionsBuilder.Options);
        }
    }
}
