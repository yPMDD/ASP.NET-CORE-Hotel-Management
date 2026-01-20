using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelAurelia.Models
{
    public enum MethodePaiement
    {
        CarteBancaire,
        Especes,
        Virement,
        EnLigne
    }
    public enum StatutPaiement
    {
        EnAttente,
        Effectue,
        Echoue
    }

    public class Paiement
    {
        public int Id { get; set; }

        public DateTime DatePaiement { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Montant { get; set; }

        public MethodePaiement Methode { get; set; }

        public StatutPaiement Statut { get; set; }

        [StringLength(100)]
        public string ReferenceTransaction { get; set; }

        public int ReservationId { get; set; }
        public Reservation Reservation { get; set; }

        public bool ValiderPaiement()
        {
            if (Statut == StatutPaiement.Effectue)
                return true;

            return false;
        }

        public bool AnnulerPaiement()
        {
            if (Statut == StatutPaiement.EnAttente)
            {
                Statut = StatutPaiement.Echoue;
                return true;
            }
            return false;
        }
    }
}
