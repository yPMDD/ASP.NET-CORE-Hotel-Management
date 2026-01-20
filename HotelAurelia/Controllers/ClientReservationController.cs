// Controllers/ClientReservationController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelAurelia.Areas.Identity.Data;
using HotelAurelia.Models;
using HotelAurelia.ViewModels;
using System.Security.Claims;

namespace HotelAurelia.Controllers
{
    [Authorize(Roles = "Client")]
    public class ClientReservationController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly HotelAureliaDbContext _context;
        private readonly ILogger<ClientReservationController> _logger;

        public ClientReservationController(
            UserManager<User> userManager,
            HotelAureliaDbContext context,
            ILogger<ClientReservationController> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        // GET: /ClientReservation/Book
        public async Task<IActionResult> Book()
        {
            try
            {
                var viewModel = new ClientReservationViewModel
                {
                    // Default dates
                    DateDebut = DateTime.Today.AddDays(1),
                    DateFin = DateTime.Today.AddDays(3),

                    // Get available rooms
                    AvailableChambres = await _context.Chambres
                        .Where(c => c.Etat == EtatChambre.Disponible)
                        .OrderBy(c => c.Tarif)
                        .Select(c => new ChambreOption
                        {
                            Id = c.Id,
                            Numero = c.Numero,
                            Type = c.Type ?? "Standard",
                            Capacite = c.Capacite,
                            Tarif = c.Tarif,
                            Image = c.image
                        })
                        .ToListAsync(),

                    // Get available services
                    AvailableServices = await _context.Services
                        .OrderBy(s => s.Nom)
                        .Select(s => new ServiceOption
                        {
                            Id = s.Id,
                            Nom = s.Nom,
                            Description = s.Description,
                            TypeService = s.TypeService,
                            Tarif = s.Tarif
                        })
                        .ToListAsync()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading reservation form");
                TempData["ErrorMessage"] = "An error occurred while loading the booking form.";
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: /ClientReservation/Book
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(ClientReservationViewModel model)
        {
            try
            {
                // Basic validation
                if (model.DateFin <= model.DateDebut)
                {
                    ModelState.AddModelError("DateFin", "Check-out date must be after check-in date.");
                }

                if (model.DateDebut < DateTime.Today)
                {
                    ModelState.AddModelError("DateDebut", "Check-in date cannot be in the past.");
                }

                if (!ModelState.IsValid)
                {
                    // Reload data
                    model.AvailableChambres = await _context.Chambres
                        .Where(c => c.Etat == EtatChambre.Disponible)
                        .OrderBy(c => c.Tarif)
                        .Select(c => new ChambreOption
                        {
                            Id = c.Id,
                            Numero = c.Numero,
                            Type = c.Type ?? "Standard",
                            Capacite = c.Capacite,
                            Tarif = c.Tarif,
                            Image = c.image
                        })
                        .ToListAsync();

                    model.AvailableServices = await _context.Services
                        .OrderBy(s => s.Nom)
                        .Select(s => new ServiceOption
                        {
                            Id = s.Id,
                            Nom = s.Nom,
                            Description = s.Description,
                            TypeService = s.TypeService,
                            Tarif = s.Tarif
                        })
                        .ToListAsync();

                    return View(model);
                }

                // Verify room availability
                var selectedRoom = await _context.Chambres
                    .FirstOrDefaultAsync(c => c.Id == model.ChambreId && c.Etat == EtatChambre.Disponible);

                if (selectedRoom == null)
                {
                    ModelState.AddModelError("ChambreId", "Selected room is no longer available.");
                    return View(model);
                }

                // Calculate nights and costs
                var nights = (model.DateFin - model.DateDebut).Days;
                var roomTotal = selectedRoom.Tarif * nights;
                var serviceTotal = 0m;

                // Handle selected services
                int? serviceId = null;
                if (model.ServiceId.HasValue)
                {
                    var service = await _context.Services.FindAsync(model.ServiceId.Value);
                    if (service != null)
                    {
                        serviceTotal = service.Tarif;
                        serviceId = service.Id;
                    }
                }

                var totalCost = roomTotal + serviceTotal;

                // Get current user
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    return RedirectToAction("Login", "Account", new { area = "Identity" });
                }

                // Calculate fidelity points based on total amount (10 points per 100 DH)
                var pointsToAdd = (int)(totalCost / 100) * 10;

                // METHOD 1: Create reservation WITHOUT the user property
                var reservation = new Reservation
                {
                    DateDebut = model.DateDebut,
                    DateFin = model.DateFin,
                    CoutTotal = totalCost,
                    Statut = StatutReservation.EnAttente,
                    // DO NOT set the user property here
                    // user = ...  // Leave this out!
                    chambreId = selectedRoom.Id,
                    ServiceId = serviceId
                };

                _context.Entry(reservation).Property("UserId").CurrentValue = currentUser.Id;

                // Update room status
                selectedRoom.Etat = EtatChambre.Reservee;
                _context.Update(selectedRoom);

                // Add fidelity points to user
                currentUser.pointsFidelite += pointsToAdd;
                await _userManager.UpdateAsync(currentUser);

                // Save reservation to database
                _context.Reservations.Add(reservation);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Booking successful! Your reservation ID is #{reservation.Id}. " +
                                            $"You earned {pointsToAdd} fidelity points. Your total points: {currentUser.pointsFidelite}";

                return RedirectToAction("Index", "Profile");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating reservation");

                // Add detailed error message for debugging
                var errorMessage = "An error occurred while processing your booking.";
                if (ex.InnerException != null)
                {
                    errorMessage += $" Details: {ex.InnerException.Message}";
                }

                ModelState.AddModelError("", errorMessage);

                // Reload data
                model.AvailableChambres = await _context.Chambres
                    .Where(c => c.Etat == EtatChambre.Disponible)
                    .OrderBy(c => c.Tarif)
                    .Select(c => new ChambreOption
                    {
                        Id = c.Id,
                        Numero = c.Numero,
                        Type = c.Type ?? "Standard",
                        Capacite = c.Capacite,
                        Tarif = c.Tarif,
                        Image = c.image
                    })
                    .ToListAsync();

                model.AvailableServices = await _context.Services
                    .OrderBy(s => s.Nom)
                    .Select(s => new ServiceOption
                    {
                        Id = s.Id,
                        Nom = s.Nom,
                        Description = s.Description,
                        TypeService = s.TypeService,
                        Tarif = s.Tarif
                    })
                    .ToListAsync();

                return View(model);
            }
        }
    }
}