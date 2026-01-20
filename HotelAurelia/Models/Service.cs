using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelAurelia.Models
{
    public class Service
    {
        public int Id { get; set; }

        [Required]
        public string Nom { get; set; } = null!;

        public string? Description { get; set; }

        public string? TypeService { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Tarif { get; set; }

        // FK vers Hotel
        //public int HotelId { get; set; }
        //public Hotel Hotel { get; set; } = null!;

        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
