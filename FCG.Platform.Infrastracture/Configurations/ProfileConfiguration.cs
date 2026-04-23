using FCG.Platform.Domain.Entities.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FCG.Platform.Infrastracture.Configurations
{
    public class ProfileConfiguration : IEntityTypeConfiguration<ProfileEntity>
    {
        public void Configure(EntityTypeBuilder<ProfileEntity> builder)
        {
            builder.HasData(
                new ProfileEntity
                {
                    Id = "0",
                    Name = "Administrator",
                    NormalizedName = "ADMINISTRATOR"
                },
                new ProfileEntity
                {
                    Id = "1",
                    Name = "Usuario",
                    NormalizedName = "USUARIO"
                }
            );
        }
    }
}