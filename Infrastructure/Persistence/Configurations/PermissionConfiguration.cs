using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SortSchedule.Domain.Entities;
using SortSchedule.Domain.Enums;

namespace SortSchedule.Infrastructure.Persistence.Configurations;

public static class DefaultPermissionIds
{
    public static readonly Guid ScheduleRead = Guid.Parse("D1A562B1-4A59-42E1-9D67-33C87332C19E");
    public static readonly Guid ScheduleManage = Guid.Parse("92A3A69A-40D9-4DB8-8F2B-DF9D23ED1D27");
    public static readonly Guid ScheduleUpdate = Guid.Parse("61A5C1F0-8041-456A-8E4E-3BC38A2B0E76");

    public static readonly Guid FreeTimeCreate = Guid.Parse("F8E1C2E6-B94F-4C82-936A-B6A78D54A43D");

    public static readonly Guid FeedbackCreate = Guid.Parse("A2E7F3D0-4B19-4A92-9C21-6A2B3C4D5E6F");
    public static readonly Guid FeedbackRead = Guid.Parse("C4B72A1F-3D8E-45A1-9F2C-6D3E4F5A1B2C");
    public static readonly Guid FeedbackUpdate = Guid.Parse("E5C83B20-4E9F-46B2-A03D-7E4F5A6B2C3D");

    public static readonly Guid UserCreate = Guid.Parse("D1C83B20-4E9F-46B2-A03D-7E4F5A6B2D1C");
}

public sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.Resource)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.Action)
            .IsRequired();

        builder.HasIndex(p => new { p.Resource, p.Action })
            .IsUnique();

        builder.HasData(
            new Permission { Id = DefaultPermissionIds.ScheduleRead, Name = "Read Schedule", Resource = "Schedule", Action = PermissionAction.Read },
            new Permission { Id = DefaultPermissionIds.ScheduleManage, Name = "Manage Schedule", Resource = "Schedule", Action = PermissionAction.Manage },
            new Permission { Id = DefaultPermissionIds.ScheduleUpdate, Name = "Update Schedule", Resource = "Schedule", Action = PermissionAction.Update },
            new Permission { Id = DefaultPermissionIds.FreeTimeCreate, Name = "Create Free Time", Resource = "FreeTime", Action = PermissionAction.Create },
            new Permission { Id = DefaultPermissionIds.FeedbackCreate, Name = "Create Feedback", Resource = "Feedback", Action = PermissionAction.Create },
            new Permission { Id = DefaultPermissionIds.FeedbackRead, Name = "Read Feedback", Resource = "Feedback", Action = PermissionAction.Read },
            new Permission { Id = DefaultPermissionIds.FeedbackUpdate, Name = "Update Feedback", Resource = "Feedback", Action = PermissionAction.Update },
            new Permission { Id = DefaultPermissionIds.UserCreate, Name = "Create User", Resource = "User", Action = PermissionAction.Create }
        );
    }
}
