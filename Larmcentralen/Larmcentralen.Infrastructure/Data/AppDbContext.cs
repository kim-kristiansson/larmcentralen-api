using Larmcentralen.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Larmcentralen.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Area> Areas => Set<Area>();
    public DbSet<Equipment> Equipment => Set<Equipment>();
    public DbSet<Alarm> Alarms => Set<Alarm>();
    public DbSet<Solution> Solutions => Set<Solution>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Area>(e =>
        {
            e.HasIndex(a => a.Title).IsUnique();
            e.Property(a => a.Title).HasMaxLength(100);
        });

        modelBuilder.Entity<Equipment>(e =>
        {
            e.Property(eq => eq.Title).HasMaxLength(200);
            e.Property(eq => eq.Category).HasMaxLength(50);

            e.HasOne(eq => eq.Area)
                .WithMany(a => a.Equipment)
                .HasForeignKey(eq => eq.AreaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Alarm>(e =>
        {
            e.Property(a => a.Title).HasMaxLength(300);
            e.Property(a => a.AlarmCode).HasMaxLength(50);
            e.Property(a => a.Severity).HasMaxLength(20);

            e.HasOne(a => a.Equipment)
                .WithMany(eq => eq.Alarms)
                .HasForeignKey(a => a.EquipmentId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(a => new { a.AlarmCode, a.EquipmentId }).IsUnique();
        });

        modelBuilder.Entity<Solution>(e =>
        {
            e.Property(s => s.Title).HasMaxLength(300);
            e.Property(s => s.EstimatedTime).HasMaxLength(50);

            e.HasOne(s => s.Alarm)
                .WithMany(a => a.Solutions)
                .HasForeignKey(s => s.AlarmId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}