using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<UserModel> Users { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)

    {
    }
}