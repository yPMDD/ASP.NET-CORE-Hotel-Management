// ViewModels/ClientReservationViewModel.cs
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HotelAurelia.ViewModels
{
    public class ClientReservationViewModel
    {
        [Required(ErrorMessage = "Check-in date is required")]
        [Display(Name = "Check-in Date")]
        [DataType(DataType.Date)]
        public DateTime DateDebut { get; set; }

        [Required(ErrorMessage = "Check-out date is required")]
        [Display(Name = "Check-out Date")]
        [DataType(DataType.Date)]
        public DateTime DateFin { get; set; }

        [Required(ErrorMessage = "Please select a room")]
        [Display(Name = "Room")]
        public int ChambreId { get; set; }

        [Display(Name = "Additional Service (Optional)")]
        public int? ServiceId { get; set; }

        [Display(Name = "Special Requests")]
        [StringLength(500, ErrorMessage = "Maximum 500 characters")]
        public string? SpecialRequests { get; set; }

        // These won't be bound from form, only populated in GET
        [BindNever]
        public List<ChambreOption> AvailableChambres { get; set; } = new List<ChambreOption>();

        [BindNever]
        public List<ServiceOption> AvailableServices { get; set; } = new List<ServiceOption>();
    }

    public class ChambreOption
    {
        public int Id { get; set; }
        public string Numero { get; set; } = null!;
        public string Type { get; set; } = null!;
        public int Capacite { get; set; }
        public decimal Tarif { get; set; }
        public string? Image { get; set; }
    }

    public class ServiceOption
    {
        public int Id { get; set; }
        public string Nom { get; set; } = null!;
        public string? Description { get; set; }
        public string TypeService { get; set; } = null!;
        public decimal Tarif { get; set; }
    }
}