// ViewModels/ReservationDetailsViewModel.cs
using HotelAurelia.Models;
using System;

namespace HotelAurelia.ViewModels
{
    public class ReservationDetailsViewModel
    {
        public int Id { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public int Nuits => (DateFin - DateDebut).Days;
        public decimal CoutTotal { get; set; }
        public StatutReservation Statut { get; set; }
        public DateTime DateCreation { get; set; }

        // User Information
        public string UserId { get; set; } = null!;
        public string UserNom { get; set; } = null!;
        public string UserPrenom { get; set; } = null!;
        public string UserEmail { get; set; } = null!;
        public string UserPhone { get; set; } = null!;
        public string UserTel { get; set; } = null!;

        // Room Information
        public int ChambreId { get; set; }
        public string ChambreNumero { get; set; } = null!;
        public string ChambreType { get; set; } = null!;
        public decimal ChambreTarif { get; set; }
        public string? ChambreImage { get; set; }

        // Service Information (if any)
        public int? ServiceId { get; set; }
        public string? ServiceNom { get; set; }
        public string? ServiceDescription { get; set; }
        public decimal? ServiceTarif { get; set; }

        // Special requests or notes field
        public string? Notes { get; set; }
    }
}