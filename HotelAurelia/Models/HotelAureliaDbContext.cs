using HotelAurelia.Areas.Identity.Data;   // ApplicationUser
using HotelAurelia.Models;               // tes modèles (Hotel, Chambre, Client, ...)
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

public class HotelAureliaDbContext : DbContext
{
    public HotelAureliaDbContext() : base()
    {
    }
    public HotelAureliaDbContext(DbContextOptions<DbContext> options)
        : base(options)
    {
    }
    //public HotelAureliaDbContext(
    //    DbContextOptions<HotelAureliaDbContext> options
    //) : base(options)
    //{
    //}
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=Bd_Aurelia;Trusted_Connection=True;MultipleActiveResultSets=true");
        }
    }

    //public DbSet<Client> Clients { get; set; }
    //public DbSet<Gerant> Gerants { get; set; }
    public DbSet<Receptionniste> Receptionnistes { get; set; }

    //public DbSet<Hotel> Hotels { get; set; }
    public DbSet<Chambre> Chambres { get; set; }
    public DbSet<Service> Services { get; set; }
    //public DbSet<SalleReunion> SallesReunion { get; set; }

    //public DbSet<ProgrammeFidelite> ProgrammesFidelite { get; set; }

    public DbSet<Reservation> Reservations { get; set; }
    //public DbSet<Paiement> Paiements { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    { 
        base.OnModelCreating(builder);

        // ---------- Inheritance avec ApplicationUser (TPH) ----------
        // EF Core mappe automatiquement Client / Gerant / Receptionniste
        // dans la même table AspNetUsers via TPH. [web:50][web:56]


        builder.Entity<Receptionniste>(entity =>
        {
            entity.ToTable("Receptionnistes");
            entity.HasKey(r => r.Id);

            entity.Property(r => r.Poste).HasMaxLength(100);

            // SINGLE explicit FK - NO cascade delete
            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);  // ← FIXED: Restrict, not Cascade
        });

        builder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DateDebut).IsRequired();
            entity.Property(e => e.DateFin).IsRequired();

            entity.Property(e => e.CoutTotal)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0m);

            // Default status
            entity.Property(e => e.Statut)
                .HasDefaultValue(StatutReservation.EnAttente);

            // Relationships
            entity.HasOne(e => e.user)
                .WithMany()  // Add .HasForeignKey(e => e.UserId) if needed
                .HasForeignKey("UserId")  // Convention will look for UserId
                .OnDelete(DeleteBehavior.Restrict);  // Prevent cascade delete

            entity.HasOne(e => e.Chambre)
                .WithMany()  // Add navigation back if Chambre has ICollection<Reservation>
                .HasForeignKey(e => e.chambreId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Service)
                .WithMany()  // Add navigation back if Service has ICollection<Reservation>
                .HasForeignKey(e => e.ServiceId)
                .IsRequired(false)  // Nullable FK
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Chambre configuration
        builder.Entity<Chambre>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Numero)
                .IsRequired()
                .HasMaxLength(50);  // Reasonable limit

            entity.Property(e => e.Type)
                .HasMaxLength(100);

            entity.Property(e => e.Tarif)
                .HasColumnType("decimal(10,2)")
                .HasDefaultValue(0m);

            // Default state
            entity.Property(e => e.Etat)
                .HasDefaultValue(EtatChambre.Disponible);

            // One-to-many with Reservations (inverse of Reservation.Chambre)
            entity.HasMany(e => e.Reservations)
                .WithOne(r => r.Chambre)
                .HasForeignKey(r => r.chambreId)
                .OnDelete(DeleteBehavior.Restrict);  // Don't delete chambre if reservations exist
        });


        // Service configuration

        // Service configuration
        builder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Nom)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Description)
                .HasMaxLength(500);

            entity.Property(e => e.TypeService)
                .HasMaxLength(100);

            entity.Property(e => e.Tarif)
                .HasColumnType("decimal(10,2)")
                .HasDefaultValue(0m);

            // One-to-many with Reservations (inverse of Reservation.Service)
            entity.HasMany(e => e.Reservations)
                .WithOne(r => r.Service)
                .HasForeignKey(r => r.ServiceId)
                .OnDelete(DeleteBehavior.SetNull);  // Nullify reservations if service deleted
        });




        // Chambre configuration (minimal - expand as needed)
        //builder.Entity<Chambre>(entity =>
        //{
        //    entity.HasKey(e => e.Id);  // Assuming Chambre has int Id
        //});

        //// Service configuration (minimal - expand as needed)
        //builder.Entity<Service>(entity =>
        //{
        //    entity.HasKey(e => e.Id);  // Assuming Service has int Id
        //});





        //builder.Entity<Chambre>()
        //    .Property(c => c.Etat)
        //    .HasDefaultValue(EtatChambre.Disponible);



        //builder.Entity<Client>()
        //    .HasMany(c => c.Reservations)
        //    .WithOne(r => r.Client)
        //    .HasForeignKey(r => r.ClientId);

        //builder.Entity<Client>()
        //    .HasOne(c => c.ProgrammeFidelite)
        //    .WithMany(p => p.Clients)
        //    .HasForeignKey(c => c.ProgrammeFideliteId)
        //    .OnDelete(DeleteBehavior.SetNull);

        // ---------- Hotel 1‑* Chambre / Service / Salle ----------


        // ---------- Reservation 1‑1 Paiement ----------
        //builder.Entity<Reservation>()
        //    .HasOne(r => r.Paiement)
        //    .WithOne(p => p.Reservation)
        //    .HasForeignKey<Paiement>(p => p.ReservationId); 

        // ---------- Reservation -> élément réservé (Chambre / Service / Salle) ----------
        // Design simple : on stocke ElementId + TypeElement dans Reservation.
        // On empêche les FK directes pour laisser ce mapping "polymorphique"
        // géré au niveau de l'application.

        //builder.Entity<Reservation>()
        //    .Property(r => r.TypeElement)
        //    .HasMaxLength(50)
        //    .IsRequired();

        //// Option : index pour les recherches par élément
        //builder.Entity<Reservation>()
        //    .HasIndex(r => new { r.TypeElement, r.ElementId });

        //// Les navigations Chambre/Service/Salle dans Reservation restent
        //// non mappées comme FK pour éviter un modèle trop complexe :
        //builder.Entity<Reservation>()
        //    .Ignore(r => r.Chambre);
        //builder.Entity<Reservation>()
        //    .Ignore(r => r.Service);
        //builder.Entity<Reservation>()
        //    .Ignore(r => r.SalleReunion);

        // ---------- Conventions supplémentaires ----------
        //builder.Entity<Chambre>()
        //    .Property(c => c.Tarif)
        //    .HasColumnType("decimal(10,2)");

        //builder.Entity<Service>()
        //    .Property(s => s.Tarif)
        //    .HasColumnType("decimal(10,2)");

        ////builder.Entity<SalleReunion>()
        ////    .Property(s => s.TarifHoraire)
        ////    .HasColumnType("decimal(10,2)");

        //builder.Entity<Reservation>()
        //    .Property(r => r.CoutTotal)
        //    .HasColumnType("decimal(10,2)");

        //builder.Entity<Paiement>()
        //    .Property(p => p.Montant)
        //    .HasColumnType("decimal(10,2)");

        

    }
}
