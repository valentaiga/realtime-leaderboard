using BackOffice.Identity.Data;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Identity.Migrations;

public class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : DbContext(options)
{
    public DbSet<UserDto> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserDto>(e =>
        {
            e.ToTable(UserDto.TableName);
            e.HasKey(x => x.Id);

            e.Property(x => x.Id).HasColumnName("id").IsRequired().ValueGeneratedNever();
            e.Property(x => x.Username).HasColumnName("username").HasMaxLength(256).IsRequired();
            e.Property(x => x.PasswordHash).HasColumnName("password_hash").HasMaxLength(32).IsRequired();
        });
    }
}