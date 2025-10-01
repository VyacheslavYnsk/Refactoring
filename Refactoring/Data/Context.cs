using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }


    public DbSet<UserModel> Users { get; set; }

    public DbSet<Hall> Halls { get; set; }
    
    public DbSet<Film> Films { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)

    {
    }
}