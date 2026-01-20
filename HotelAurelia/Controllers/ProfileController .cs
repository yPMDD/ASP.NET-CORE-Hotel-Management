// Controllers/ProfileController.cs
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
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly HotelAureliaDbContext _context;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(
            UserManager<User> userManager,
            HotelAureliaDbContext context,
            ILogger<ProfileController> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        // GET: /Profile
        public async Task<IActionResult> Index()
        {
            try
            {
                // Get current user
                var currentUser = await _userManager.GetUserAsync(User);

                if (currentUser == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                // Set ViewBag for pointsFidelite
                ViewBag.PointsFidelite = currentUser.pointsFidelite;

                // Get user's reservations with details
                var reservations = await _context.Reservations
                    .Where(r => r.user.Id == currentUser.Id)
                    .Include(r => r.Chambre)
                    .Include(r => r.Service)
                    .OrderByDescending(r => r.DateDebut)
                    .ToListAsync();

                // Map to view model
                var reservationViewModels = reservations.Select(r => new ReservationWithDetails
                {
                    Id = r.Id,
                    DateDebut = r.DateDebut,
                    DateFin = r.DateFin,
                    CoutTotal = r.CoutTotal,
                    Statut = r.Statut,
                    ChambreNumero = r.Chambre.Numero,
                    ChambreType = r.Chambre.Type ?? "Standard",
                    ChambreImage = r.Chambre.image,
                    ChambreTarif = r.Chambre.Tarif,
                    ServiceNom = r.Service?.Nom,
                    ServiceDescription = r.Service?.Description,
                    ServiceTarif = r.Service?.Tarif
                }).ToList();

                // Create view model
                var viewModel = new ClientProfileViewModel
                {
                    User = currentUser,
                    Reservations = reservationViewModels
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading profile page");
                TempData["ErrorMessage"] = "An error occurred while loading your profile.";
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: /Profile/ReservationDetails/{id}
        public async Task<IActionResult> ReservationDetails(int id)
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);

                var reservation = await _context.Reservations
                    .Include(r => r.Chambre)
                    .Include(r => r.Service)
                    .FirstOrDefaultAsync(r => r.Id == id && r.user.Id == currentUser.Id);

                if (reservation == null)
                {
                    return NotFound();
                }

                var viewModel = new ReservationWithDetails
                {
                    Id = reservation.Id,
                    DateDebut = reservation.DateDebut,
                    DateFin = reservation.DateFin,
                    CoutTotal = reservation.CoutTotal,
                    Statut = reservation.Statut,
                    ChambreNumero = reservation.Chambre.Numero,
                    ChambreType = reservation.Chambre.Type ?? "Standard",
                    ChambreImage = reservation.Chambre.image,
                    ChambreTarif = reservation.Chambre.Tarif,
                    ServiceNom = reservation.Service?.Nom,
                    ServiceDescription = reservation.Service?.Description,
                    ServiceTarif = reservation.Service?.Tarif
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading reservation details");
                TempData["ErrorMessage"] = "An error occurred while loading reservation details.";
                return RedirectToAction("Index");
            }
        }

        // POST: /Profile/CancelReservation/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelReservation(int id)
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);

                var reservation = await _context.Reservations
                    .FirstOrDefaultAsync(r => r.Id == id && r.user.Id == currentUser.Id);

                if (reservation == null)
                {
                    return NotFound();
                }

                // Check if reservation can be cancelled
                if (reservation.DateDebut <= DateTime.Now.AddDays(1))
                {
                    TempData["ErrorMessage"] = "Reservations can only be cancelled at least 24 hours before check-in.";
                    return RedirectToAction("Index");
                }

                reservation.Annuler();
                _context.Update(reservation);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Reservation cancelled successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling reservation");
                TempData["ErrorMessage"] = "An error occurred while cancelling the reservation.";
                return RedirectToAction("Index");
            }
        }
    }
}