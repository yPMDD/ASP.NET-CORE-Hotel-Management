using HotelAurelia.Areas.Identity.Data; 
namespace HotelAurelia.Models
{
    public class Gerant : User
    {
        public string? Departement { get; set; }
    }
}
