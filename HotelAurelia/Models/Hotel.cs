using System.ComponentModel.DataAnnotations;

namespace HotelAurelia.Models
{
    public class Hotel
    {
        public int Id { get; set; }

        [Required, StringLength(200)]
        public string Nom { get; set; } = null!;

        [Required, StringLength(300)]
        public string Adresse { get; set; } = null!;

        [StringLength(20)]
        public string? Telephone { get; set; }

        [StringLength(200)]
        public string? Email { get; set; }

        // Navigation
        public ICollection<Chambre> Chambres { get; set; } = new List<Chambre>();
        public ICollection<Service> Services { get; set; } = new List<Service>();
        public ICollection<SalleReunion> SallesReunion { get; set; } = new List<SalleReunion>();
    }
}
