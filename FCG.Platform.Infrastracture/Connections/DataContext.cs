using FCG.Platform.Domain.Entities.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace FCG.Platform.Infrastracture.Connections
{
    public class DataContext : IdentityDbContext<UserEntity, ProfileEntity, string>
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<UserEntity> UserEntity { get; set; }
        public DbSet<ProfileEntity> ProfileEntity { get; set; }
        public DbSet<GameEntity> GameEntity { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}