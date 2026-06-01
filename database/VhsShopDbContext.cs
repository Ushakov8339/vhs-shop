using Microsoft.EntityFrameworkCore;
using VhsShop.model;

namespace VhsShop.database;

/// <summary>
/// Контекст базы данных магазина VHS кассет
/// DB поля соответствуют таблицам PostgreSQL
/// </summary>
public class VhsShopDbContext(DbContextOptions<VhsShopDbContext> options) : DbContext(options)
{
    public DbSet<Cassette> Cassettes { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Purchase> Purchases { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Настройки таблицы VHS кассет
        modelBuilder.Entity<Cassette>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.MovieTitle).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Director).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Genre).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Condition).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Language).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Rating).HasMaxLength(10).IsRequired();
            entity.Property(x => x.Price).HasPrecision(18, 2);
            entity.Property(x => x.Description).HasMaxLength(1000);
        });

        // Настройки таблицы клиентов
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.FullName).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(300).IsRequired();
        });

        // Настройки таблицы покупок
        modelBuilder.Entity<Purchase>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CustomerName).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Total).HasPrecision(18, 2);
            entity.Property(x => x.CassetteIds)
                .HasColumnType("uuid[]")
                .IsRequired();
        });
    }
}

