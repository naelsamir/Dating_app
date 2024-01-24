using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data;
public class DataContext:IdentityDbContext<AppUser,AppRoles,int,IdentityUserClaim<int>,AppUserRole,IdentityUserLogin<int>,
IdentityRoleClaim<int>,IdentityUserToken<int>>
{
    private DbSet<AppUser> users;


    public DataContext(DbContextOptions options):base(options)
    {}
    public DbSet<UserLike> Likes{get;set;}
    public DbSet<Message> Message{get;set;}

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<AppUser>()
        .HasMany(ur=>ur.UserRoles)
        .WithOne(ur=>ur.User)
        .HasForeignKey(ur=>ur.UserId)
        .IsRequired();

        builder.Entity<AppRoles>()
        .HasMany(ur=>ur.UserRoles)
        .WithOne(ur=>ur.Roles)
        .HasForeignKey(ur=>ur.RoleId)
        .IsRequired();

        builder.Entity<UserLike>()
        .HasKey(k=> new {k.SourceUserId,k.TargetUserId});

        builder.Entity<UserLike>()
        .HasOne(s=>s.SourceUser)
        .WithMany(l=>l.LikedUsers)
        .HasForeignKey(s=>s.SourceUserId)
        .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<UserLike>()
        .HasOne(s=>s.TargetUser)
        .WithMany(l=>l.LikedByUsers)
        .HasForeignKey(s=>s.TargetUserId)
        .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Message>()
        .HasOne(u=> u.Recipient)
        .WithMany(m=>m.MessagesRecieved)
        .OnDelete(DeleteBehavior.Restrict);

         builder.Entity<Message>()
        .HasOne(u=> u.Sender)
        .WithMany(m=>m.MessagesSent)
        .OnDelete(DeleteBehavior.Restrict);
    }

}
