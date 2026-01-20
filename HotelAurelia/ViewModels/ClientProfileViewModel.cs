// ViewModels/ClientProfileViewModel.cs
using HotelAurelia.Areas.Identity.Data;
using HotelAurelia.Models;

namespace HotelAurelia.ViewModels
{
    public class ClientProfileViewModel
    {
        // User Information
        public User User { get; set; } = null!;

        // Reservations
        public List<ReservationWithDetails> Reservations { get; set; } = new List<ReservationWithDetails>();

        // Calculated properties
        public int TotalReservations => Reservations.Count;
        public int ActiveReservations => Reservations.Count(r =>
            r.Statut == StatutReservation.Confirmee ||
            r.Statut == StatutReservation.Payee);
    }

    public class ReservationWithDetails
    {
        public int Id { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public int Nights => (DateFin - DateDebut).Days;
        public decimal CoutTotal { get; set; }
        public StatutReservation Statut { get; set; }

        // Chambre details
        public string ChambreNumero { get; set; } = null!;
        public string ChambreType { get; set; } = null!;
        public string? ChambreImage { get; set; }
        public decimal ChambreTarif { get; set; }

        // Service details (if any)
        public string? ServiceNom { get; set; }
        public string? ServiceDescription { get; set; }
        public decimal? ServiceTarif { get; set; }
    }
}