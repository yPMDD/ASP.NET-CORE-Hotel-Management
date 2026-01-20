// ViewModels/GerantDashboardViewModel.cs
using HotelAurelia.Models;
using System;

namespace HotelAurelia.ViewModels
{
    public class GerantDashboardViewModel
    {
        public int TotalChambres { get; set; }
        public int ChambresDisponibles { get; set; }
        public int TotalReservations { get; set; }
        public int ReservationsCetteSemaine { get; set; }
        public int TotalReceptionnistes { get; set; }
        public decimal RevenuMois { get; set; }
        public int PourcentageChangementRevenu { get; set; }
        public List<ReservationResume> ReservationsRecentes { get; set; } = new List<ReservationResume>();
        public List<ChambreResume> ChambresPopulaires { get; set; } = new List<ChambreResume>();
    }

    public class ReservationResume
    {
        public int Id { get; set; }
        public string ClientNom { get; set; } = null!;
        public string ChambreNumero { get; set; } = null!;
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public decimal CoutTotal { get; set; }
        public StatutReservation Statut { get; set; }
    }

    public class ChambreResume
    {
        public int Id { get; set; }
        public string Numero { get; set; } = null!;
        public string? Type { get; set; }
        public string? Image { get; set; }
        public int Capacite { get; set; }
        public decimal Tarif { get; set; }
        public EtatChambre Etat { get; set; }
        public int NombreReservations { get; set; }
    }
}