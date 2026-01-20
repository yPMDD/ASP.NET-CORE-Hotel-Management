using System.ComponentModel.DataAnnotations;
namespace HotelAurelia.ViewModels  // ← EXACT match
{
    public class CreateReceptionnisteViewModel
    {
        [Required(ErrorMessage = "Le nom est obligatoire")]
        [StringLength(100)]
        public string Nom { get; set; }

        [Required(ErrorMessage = "Le prénom est obligatoire")]
        [StringLength(100)]
        public string Prenom { get; set; }

        [Required(ErrorMessage = "L'email est obligatoire")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Le téléphone est obligatoire")]
        [Phone]
        [StringLength(20)]
        public string Tel { get; set; }

        [Required(ErrorMessage = "Le mot de passe est obligatoire")]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirmez le mot de passe")]
        [Compare("Password", ErrorMessage = "Les mots de passe ne correspondent pas")]
        public string ConfirmPassword { get; set; }

        [StringLength(100)]
        public string? Poste { get; set; }
    }
}