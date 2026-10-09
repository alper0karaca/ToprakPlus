using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToprakPlusServer.Domain.Users;

namespace ToprakPlusServer.Infrastructure.Configurations;

public sealed class UserConfiguration :IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(i => i.Id);
        builder.OwnsOne(x => x.FirstName);
        builder.OwnsOne(x => x.LastName);
        builder.OwnsOne(x => x.FullName);
        builder.OwnsOne(x => x.Email);
        builder.OwnsOne(x => x.UserName);
        builder.OwnsOne(x => x.Password);
        builder.OwnsOne(x => x.ForgotPasswordCode);
        builder.OwnsOne(x => x.ForgotPasswordDate);
        builder.OwnsOne(x => x.IsForgotPasswordCompleted);
        
    }
}