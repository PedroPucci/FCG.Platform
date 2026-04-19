using Microsoft.EntityFrameworkCore;

namespace FCG.Platform.Infrastracture.Connections
{
    public static class DataModelConfiguration
    {
        public static void ConfigureModels(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<UserEntity>(entity =>
            //{
            //    entity.HasKey(u => u.Id);

            //    entity.Property(u => u.Id)
            //          .UseIdentityByDefaultColumn()
            //          .ValueGeneratedOnAdd();

            //    entity.Property(u => u.Email).IsRequired();
            //    entity.Property(u => u.Password).IsRequired();
            //    entity.Property(u => u.Cpf).HasMaxLength(14);
            //    entity.Property(u => u.IsActive).IsRequired();

            //    entity.HasOne(u => u.PessoalProfile)
            //          .WithOne(p => p.User)
            //          .HasForeignKey<PessoalProfileEntity>(p => p.UserId)
            //          .OnDelete(DeleteBehavior.Cascade);

            //    entity.HasOne(u => u.ProfessionalProfile)
            //          .WithOne(p => p.User)
            //          .HasForeignKey<ProfessionalProfileEntity>(p => p.UserId)
            //          .OnDelete(DeleteBehavior.Cascade);

            //    entity.HasMany(u => u.Courses)
            //          .WithOne(c => c.User)
            //          .HasForeignKey(c => c.UserId)
            //          .OnDelete(DeleteBehavior.Cascade);

            //    entity.HasOne(u => u.Department)
            //          .WithOne(d => d.User)
            //          .HasForeignKey<DepartmentEntity>(d => d.UserId)
            //          .OnDelete(DeleteBehavior.Restrict);

            //    entity.HasMany(u => u.HistoryPasswords)
            //          .WithOne(ph => ph.User)
            //          .HasForeignKey(ph => ph.UserId)
            //          .OnDelete(DeleteBehavior.Cascade);

            //    entity.HasMany(u => u.UserBadges)
            //          .WithOne(ub => ub.User)
            //          .HasForeignKey(ub => ub.UserId)
            //          .OnDelete(DeleteBehavior.Cascade);

            //    entity.HasMany(u => u.CourseNotes)
            //          .WithOne(n => n.Student)
            //          .HasForeignKey(n => n.StudentId)
            //          .OnDelete(DeleteBehavior.Restrict);
            //});
        }
    }
}