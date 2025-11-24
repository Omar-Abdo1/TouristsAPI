using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TouristsCore.Entities;

namespace TouristsRepository;

public class TouristsContext : IdentityDbContext<User>
{
     public TouristsContext(DbContextOptions<TouristsContext>  options) : base(options)
     {
          
     }

     protected override void OnModelCreating(ModelBuilder builder)
     {
          base.OnModelCreating(builder);
          builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        
          builder.Entity<User>().ToTable("Users");
          builder.Entity<IdentityRole>().ToTable("Roles");
          builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
          builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
          builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
          builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
          builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
     }
     DbSet<GuideProfile>  GuideProfiles { get; set; }
     DbSet<TouristProfile>   TouristProfiles { get; set; }
     DbSet<Booking>  Bookings { get; set; }
     DbSet<FileRecord>   FileRecords { get; set; }
     DbSet<Language>  Languages { get; set; }
     DbSet<GuideLanguage> GuideLanguages { get; set; }
     DbSet<Payment>   Payments { get; set; }
     DbSet<Review>    Reviews { get; set; }
     DbSet<Tour>       Tours { get; set; }
     // Chat : 
     DbSet<Chat>       Chats { get; set; }
     DbSet<ChatParticipant>   ChatParticipants { get; set; }
     DbSet<Message>  Messages { get; set; }
     DbSet<MessageVisibility>   MessageVisibility { get; set; }
     DbSet<UserConnection>   UserConnections { get; set; }
     DbSet<UserGroup>    UserGroups { get; set; }
}