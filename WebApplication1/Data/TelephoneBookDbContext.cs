using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;

namespace WebApplication1.Data;

public class TelephoneBookDbContext : DbContext
{
    public DbSet<TelephoneBookModel> TelephoneBook { get; set; }
    public TelephoneBookDbContext(DbContextOptions options):base(options) {}
}