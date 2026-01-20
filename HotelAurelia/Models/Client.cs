namespace HotelAurelia.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string? Preferences { get; set; }

        public int PointsFidelite { get; set; }

        // Navigation
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

        public ProgrammeFidelite? ProgrammeFidelite { get; set; }
        public int? ProgrammeFideliteId { get; set; }
    }
}
