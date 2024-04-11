using Microsoft.EntityFrameworkCore;
using RedisLink.Models;

namespace RedisLink.Data;

public class AppDbContext : DbContext
{
    public DbSet<Driver> Drivers { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options):
        base(options)
    {
        
    }
}