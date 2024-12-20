using Microsoft.EntityFrameworkCore;

public class NeonDbContext(DbContextOptions<NeonDbContext> options) : DbContext(options)
{
    public required DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Đảm bảo bảng được đặt tên là "users" với chữ thường
        modelBuilder.Entity<User>().ToTable("users");

        // Cấu hình thêm nếu cần (ví dụ: ràng buộc độ dài)
        modelBuilder.Entity<User>(entity =>
        {

            entity.Property(u => u.Email)
                  .HasMaxLength(100)
                  .IsRequired();

            entity.Property(u => u.Password)
                  .IsRequired();
        });
    }
}