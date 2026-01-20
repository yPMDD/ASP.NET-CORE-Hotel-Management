using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HotelAurelia.Areas.Identity.Data;

namespace HotelAurelia.Areas.Identity.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            // Configure Discriminator
            builder.Entity<User>()
                .HasDiscriminator<string>("Discriminator")
                .HasValue<User>("User");

            // Set default value for Discriminator column
            builder.Entity<User>()
                .Property("Discriminator")
                .HasDefaultValue("User");

            // Your existing configurations
            builder.Entity<User>(entity =>
            {
                entity.Property(e => e.Nom)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Prenom)
                    .IsRequired();

                entity.Property(e => e.Tel)
                    .IsRequired();

                entity.Property(e => e.Role)
                    .IsRequired();

                entity.Property(e => e.pointsFidelite)
                    .IsRequired()
                    .HasDefaultValue(0);

            });
        }
    }
}