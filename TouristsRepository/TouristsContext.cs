using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TouristsCore.Entities;

namespace TouristsRepository;

public class TouristsContext : IdentityDbContext<User,IdentityRole<Guid>,Guid>
{
     public TouristsContext(DbContextOptions<TouristsContext>  options) : base(options)
     {
          
     }

     protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
     {
          configurationBuilder.Properties<Enum>().HaveConversion<string>();
          // change any Enum to string in DB
     }

     protected override void OnModelCreating(ModelBuilder builder)
     {
          base.OnModelCreating(builder);
          builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        
          builder.Entity<User>().ToTable("Users");
          builder.Entity<IdentityRole<Guid>>().ToTable("Roles");
          builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
          builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
          builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
          builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
          builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");
     }
     public DbSet<GuideProfile>  GuideProfiles { get; set; }
     public DbSet<TouristProfile>   TouristProfiles { get; set; }
     public DbSet<Booking>  Bookings { get; set; }
     public  DbSet<FileRecord>   FileRecords { get; set; }
     public DbSet<Language>  Languages { get; set; }
     public DbSet<GuideLanguage> GuideLanguages { get; set; }
     public DbSet<Payment>   Payments { get; set; }
     public  DbSet<Review>    Reviews { get; set; }
     public DbSet<Tour>       Tours { get; set; }
     // Chat : 
     public DbSet<Chat>       Chats { get; set; }
     public DbSet<ChatParticipant>   ChatParticipants { get; set; }
     public  DbSet<Message>  Messages { get; set; }
     public  DbSet<MessageVisibility>   MessageVisibility { get; set; }
     public DbSet<UserConnection>   UserConnections { get; set; }
     public  DbSet<UserGroup>    UserGroups { get; set; }
}