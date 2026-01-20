using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HotelAurelia.Areas.Identity.Data;

namespace HotelAurelia.Models
{
    public enum StatutReservation
    {
        EnAttente,
        Confirmee,
        Annulee,
        Payee
    }
    public class Reservation
    {
        public int Id { get; set; }

        [Required]
        public DateTime DateDebut { get; set; }

        [Required]
        public DateTime DateFin { get; set; }

        public StatutReservation Statut { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CoutTotal { get; set; }

        // Client
        
        public User user { get; set; }

        // Paiement
        //public int? PaiementId { get; set; }
        //public Paiement Paiement { get; set; }

        // Chambre (optional, if reservation is for a room)
        public int chambreId { get; set; }
        public Chambre Chambre { get; set; }

        // Service (optional, if reservation is for a service)
        public int? ServiceId { get; set; }
        public Service Service { get; set; }
        // Salle de Réunion (optional, if reservation is for a meeting room)
        //public int? SalleReunionId { get; set; }
        //public SalleReunion SalleReunion { get; set; }

        // Polymorphic element: Chambre / Service / SalleReunion
        //public int ElementId { get; set; }

        //[Required]
        //public string TypeElement { get; set; } // "Chambre", "Service", "SalleReunion"

        public void Confirmer()
        {
            Statut = StatutReservation.Confirmee;
        }

        public void Annuler()
        {
            Statut = StatutReservation.Annulee;
        }
    }
}
