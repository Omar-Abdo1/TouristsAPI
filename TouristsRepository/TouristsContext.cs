using System.Linq.Expressions;
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
          
          builder.Entity<TouristProfile>()
               .HasIndex(t => t.UserId)
               .IsUnique();

          builder.Entity<GuideProfile>()
               .HasIndex(g => g.UserId)
               .IsUnique();
          
          foreach (var entityType in builder.Model.GetEntityTypes())
          {
               // Apply Global Query Filter (Hide Deleted Items)
               if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
               {
                    // build the query   e=>e.IsDeleted == false 
                    var parameter = Expression.Parameter(entityType.ClrType, "e"); // e
                    var property = Expression.Property(parameter, nameof(ISoftDeletable.IsDeleted)); // isDeleted
                    var falseConstant = Expression.Constant(false); // false constant
                    var lambda = Expression.Lambda(Expression.Equal(property, falseConstant), parameter); 
                    // combination 

                    builder.Entity(entityType.ClrType).HasQueryFilter(lambda);
               }
               
               // Make Non - Clustered Index on CreatedAt For Fast Sorting and Cursor Pagination 
               if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
               {
                    builder.Entity(entityType.ClrType).HasIndex(nameof(BaseEntity.CreatedAt));
               }

               // (Change Cascade to Restrict globally)
               var foreignKeys = entityType.GetForeignKeys()
                    .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

               foreach (var fk in foreignKeys)
               {
                    fk.DeleteBehavior = DeleteBehavior.Restrict;
               }
          }   
          
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
     public DbSet<TourMedia> TourMedia { get; set; }
     public DbSet<TourSchedule>  TourSchedule { get; set; }
     // Chat : 
     public DbSet<Chat>       Chats { get; set; }
     public DbSet<ChatParticipant>   ChatParticipants { get; set; }
     public  DbSet<Message>  Messages { get; set; }
     public  DbSet<MessageVisibility>   MessageVisibility { get; set; }
     public DbSet<UserConnection>   UserConnections { get; set; }
     public  DbSet<UserGroup>    UserGroups { get; set; }
}