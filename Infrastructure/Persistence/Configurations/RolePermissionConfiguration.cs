using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SortSchedule.Domain.Entities;
using SortSchedule.Domain.Enums;

namespace SortSchedule.Infrastructure.Persistence.Configurations;

public sealed class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("RolePermissions");

        builder.HasKey(rp => new { rp.RoleId, rp.PermissionId });

        builder.HasOne(rp => rp.Role)
            .WithMany(r => r.Permissions)
            .HasForeignKey(rp => rp.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(rp => rp.Permission)
            .WithMany()
            .HasForeignKey(rp => rp.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(rp => rp.Effect)
            .IsRequired()
            .HasDefaultValue(PermissionEffect.Allow);

        builder.HasData(
            // Student
            new RolePermission { RoleId = DefaultRoleIds.Student, PermissionId = DefaultPermissionIds.ScheduleRead, Effect = PermissionEffect.Allow },

            // Lecturer
            new RolePermission { RoleId = DefaultRoleIds.Lecturer, PermissionId = DefaultPermissionIds.ScheduleRead, Effect = PermissionEffect.Allow },
            new RolePermission { RoleId = DefaultRoleIds.Lecturer, PermissionId = DefaultPermissionIds.FreeTimeCreate, Effect = PermissionEffect.Allow },
            new RolePermission { RoleId = DefaultRoleIds.Lecturer, PermissionId = DefaultPermissionIds.FeedbackCreate, Effect = PermissionEffect.Allow },

            // Admin
            new RolePermission { RoleId = DefaultRoleIds.Admin, PermissionId = DefaultPermissionIds.ScheduleManage, Effect = PermissionEffect.Allow },
            new RolePermission { RoleId = DefaultRoleIds.Admin, PermissionId = DefaultPermissionIds.ScheduleUpdate, Effect = PermissionEffect.Allow },
            new RolePermission { RoleId = DefaultRoleIds.Admin, PermissionId = DefaultPermissionIds.ScheduleRead, Effect = PermissionEffect.Allow },
            new RolePermission { RoleId = DefaultRoleIds.Admin, PermissionId = DefaultPermissionIds.FeedbackRead, Effect = PermissionEffect.Allow },
            new RolePermission { RoleId = DefaultRoleIds.Admin, PermissionId = DefaultPermissionIds.FeedbackUpdate, Effect = PermissionEffect.Allow },
            new RolePermission { RoleId = DefaultRoleIds.Admin, PermissionId = DefaultPermissionIds.UserCreate, Effect = PermissionEffect.Allow }
        );
    }
}
