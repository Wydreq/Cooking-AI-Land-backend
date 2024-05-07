using CookingAILand.Core.DAL.Entities;
using CookingAILand.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CookingAILand.Core.DAL.EntitiesConfiguration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(x => x.FirstName).IsRequired().HasMaxLength(25);
        builder.Property(x => x.LastName).IsRequired().HasMaxLength(25);
        builder.Property(x => x.DateOfBirth).IsRequired();
    }
}