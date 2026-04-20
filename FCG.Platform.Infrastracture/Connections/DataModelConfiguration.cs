using FCG.Platform.Domain.Entities.Entity;
using Microsoft.EntityFrameworkCore;

namespace FCG.Platform.Infrastracture.Connections
{
    public static class DataModelConfiguration
    {
        public static void ConfigureModels(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserEntity>(entity =>
            {
                entity.HasKey(u => u.Id);

                entity.Property(u => u.Id)
                      .UseIdentityColumn()
                      .ValueGeneratedOnAdd();

                entity.Property(u => u.Name)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.Property(u => u.Email)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.Property(u => u.Password)
                      .IsRequired()
                      .HasMaxLength(255);

                entity.HasIndex(u => u.Email)
                      .IsUnique();
            });

            modelBuilder.Entity<GameEntity>(entity =>
            {
                entity.HasKey(g => g.Id);

                entity.Property(g => g.Id)
                      .UseIdentityColumn()
                      .ValueGeneratedOnAdd();

                entity.Property(g => g.Name)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.Property(g => g.Description)
                      .HasMaxLength(500);
            });

            modelBuilder.Entity<UserGameEntity>(entity =>
            {
                entity.HasKey(ug => ug.Id);

                entity.Property(ug => ug.Id)
                      .UseIdentityColumn()
                      .ValueGeneratedOnAdd();

                entity.Property(ug => ug.UserId)
                      .IsRequired();

                entity.Property(ug => ug.GameId)
                      .IsRequired();

                entity.HasOne(ug => ug.User)
                      .WithMany()
                      .HasForeignKey(ug => ug.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ug => ug.Game)
                      .WithMany()
                      .HasForeignKey(ug => ug.GameId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(ug => new { ug.UserId, ug.GameId })
                      .IsUnique();
            });
        }
    }
}