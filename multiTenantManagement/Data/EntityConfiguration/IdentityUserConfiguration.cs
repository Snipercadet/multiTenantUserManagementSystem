using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using multiTenantManagement.Models.Identities;

namespace multiTenantManagement.Data.EntityConfiguration
{
    public class IdentityUserConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public static void ApplyUserConfiguration(ModelBuilder builder, string tenantId)
        {
            builder.Entity<AppUser>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(x => x.Id);
                entity.HasQueryFilter(x => x.TenantId == tenantId);
            });

            builder.Entity<AppRole>(entity =>
            {
                entity.ToTable("Roles");
                entity.HasKey(x => x.Id);
                entity.HasQueryFilter(x => x.TenantId == tenantId);
            });

            builder.Entity<UserRole>(entity =>
            {
                entity.ToTable("UserRoles");
                entity.HasKey(x => new {x.RoleId,x.UserId});
                entity.HasQueryFilter(x => x.TenantId == tenantId);
            });

            builder.Entity<RoleClaim>(entity =>
            {
                entity.ToTable("RoleClaims");
                entity.HasKey(x => x.Id);
                entity.HasQueryFilter(x => x.TenantId == tenantId);

            });

            builder.Entity<UserLogin>(entity =>
            {
                entity.ToTable("UserLogins");
                entity.HasKey(x => x.UserId);
                entity.HasQueryFilter(x => x.TenantId == tenantId);
            });
            builder.Entity<UserToken>(entity =>
            {
                entity.ToTable("UserTokens");
                entity.HasKey(x => x.UserId);
                entity.HasQueryFilter(x => x.TenantId == tenantId);
            });
            builder.Entity<UserClaim>(entity =>
            {
                entity.ToTable("UserClaims");
                entity.HasKey(x => x.Id);
                entity.HasQueryFilter(x => x.TenantId == tenantId);
            });
        }

        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.HasIndex(x => x.Email);
        }
    }
}
