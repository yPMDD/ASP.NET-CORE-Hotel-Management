// Add this class anywhere in your project
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class DbContextFactory : IDesignTimeDbContextFactory<HotelAureliaDbContext>
{
    public HotelAureliaDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DbContext>();
        optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=Bd_Aurelia;Trusted_Connection=True;MultipleActiveResultSets=true");

        return new HotelAureliaDbContext(optionsBuilder.Options);
    }
}