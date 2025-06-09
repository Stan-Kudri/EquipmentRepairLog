using EquipmentRepairDocument.Core.Data.Users;
using EquipmentRepairDocument.Core.Extension;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EquipmentRepairDocument.Core.DBContext.Configuration
{
    public class UserConfiguranion : EntityBaseConfiguration<User>
    {
        protected override void ConfigureModel(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("user");
            builder.HasIndex(e => e.Username).IsUnique();
            builder.Property(e => e.Username).IsRequired().HasColumnName("username").HasMaxLength(128);
            builder.Property(e => e.PasswordHash).IsRequired().HasColumnName("password_hash").HasMaxLength(128);
            builder.Property(e => e.UserRole).HasColumnName("role").HasDefaultValue(UserRole.Viewer).SmartEnumNameConversion();
        }
    }
}
