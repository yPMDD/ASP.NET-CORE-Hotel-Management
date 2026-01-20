using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelAurelia.Models
{
    public class SalleReunion
    {
        public int Id { get; set; }

        [Required]
        public string Nom { get; set; } = null!;

        public int Capacite { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TarifHoraire { get; set; }

        public bool Disponible { get; set; }

        // FK vers Hotel
        public int HotelId { get; set; }
        public Hotel Hotel { get; set; } = null!;

        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
