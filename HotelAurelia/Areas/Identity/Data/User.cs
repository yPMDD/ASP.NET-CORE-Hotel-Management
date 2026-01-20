using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace HotelAurelia.Areas.Identity.Data
{
    public class User : IdentityUser
    {
        [Required(ErrorMessage = "Le nom est obligatoire")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Le nom doite etre minmum 8 caracteres")]
        
        public String Nom { get; set; }
        [Required]
        public String Prenom { get; set; }
        [Required]
        public String Tel { get; set; }
        [Required]
        public String Role { get; set; }
        [Required]
        public int pointsFidelite { get; set; } = 0;

    }

}
