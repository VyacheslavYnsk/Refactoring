using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<UserDto> Users { get; set; }

    public DbSet<HallDto> Halls { get; set; }

    public DbSet<SeatCategory> SeatCategories { get; set; }

    public DbSet<SeatDto> Seats { get; set; }





    protected override void OnModelCreating(ModelBuilder modelBuilder)

    {
    }
}