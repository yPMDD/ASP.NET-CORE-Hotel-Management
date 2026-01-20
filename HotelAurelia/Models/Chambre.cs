using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelAurelia.Models
{
    public enum EtatChambre
    {
        Disponible,
        Reservee,
        Maintenance
    }
    public class Chambre
    {
        public int Id { get; set; }

        [Required]
        public string Numero { get; set; } = null!;

        public string? image { get; set; }
        public string? Type { get; set; }

        public int Capacite { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Tarif { get; set; }

        public EtatChambre Etat { get; set; } = EtatChambre.Disponible;

        // FK vers Hotel
        //public int HotelId { get; set; }
        //public Hotel Hotel { get; set; } = null!;

        // Navigation Réservations (via Reservation.ElementId/TypeElement)
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
