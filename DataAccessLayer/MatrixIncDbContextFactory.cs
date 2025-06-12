using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DataAccessLayer
{
    public class MatrixIncDbContextFactory : IDesignTimeDbContextFactory<MatrixIncDbContext>
    {
        public MatrixIncDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MatrixIncDbContext>();
            optionsBuilder.UseSqlite("Data Source=MatrixInc.db");

            return new MatrixIncDbContext(optionsBuilder.Options);
        }
    }
} 