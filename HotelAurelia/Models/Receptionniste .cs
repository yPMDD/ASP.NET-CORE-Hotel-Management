using HotelAurelia.Areas.Identity.Data;

namespace HotelAurelia.Models
{
    public class Receptionniste
    {
        public int Id { get; set; }
        public string? Poste { get; set; }

        // ONLY this FK property
        public string UserId { get; set; } = null!;
        // NO navigation property User to avoid EF confusion
        public User User { get; set; } = null!;
    }

}
    


