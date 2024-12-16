using Microsoft.EntityFrameworkCore;

public class NeonDbContext : DbContext
{
    public NeonDbContext(DbContextOptions<NeonDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        // Các cấu hình khác nếu cần
        // Đảm bảo tên bảng là "users" với chữ thường
        modelBuilder.Entity<User>().ToTable("users");
    }
}