using HotelAurelia.Areas.Identity.Data;
using HotelAurelia.Models;
using HotelAurelia.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace HotelAurelia.Controllers
{
    public class HomeController : Controller
    {
        private readonly HotelAureliaDbContext _context;
        private readonly ApplicationDbContext _identityContext;
        private readonly ILogger<HomeController> _logger;
        public HomeController(  HotelAureliaDbContext context,
                                ApplicationDbContext identityContext,
                                ILogger<HomeController> logger)
        {
            _context = context;
            _identityContext = identityContext;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        // In your HomeController or DashboardController
        [Authorize(Roles = "Gerant")]
        [Authorize(Roles = "Gerant")]
        public async Task<IActionResult> GerantDashboard()
        {
            try
            {
                var dashboardData = new GerantDashboardViewModel();

                // Get total rooms count from HotelAureliaDbContext
                dashboardData.TotalChambres = await _context.Chambres.CountAsync();

                // Get available rooms count
                dashboardData.ChambresDisponibles = await _context.Chambres
                    .Where(c => c.Etat == EtatChambre.Disponible)
                    .CountAsync();

                // Get total reservations count
                dashboardData.TotalReservations = await _context.Reservations.CountAsync();

                // Get this week's reservations
                var startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                var endOfWeek = startOfWeek.AddDays(7);
                dashboardData.ReservationsCetteSemaine = await _context.Reservations
                    .Where(r => r.DateDebut >= startOfWeek && r.DateDebut < endOfWeek)
                    .CountAsync();

                // Get receptionnistes count from ApplicationDbContext (Identity)
                dashboardData.TotalReceptionnistes = await _identityContext.Users
                    .Where(u => u.Role == "Receptionniste")
                    .CountAsync();

                // Get this month's revenue
                var startOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                var endOfMonth = startOfMonth.AddMonths(1);
                var reservationsThisMonth = await _context.Reservations
                    .Where(r => r.DateDebut >= startOfMonth && r.DateDebut < endOfMonth)
                    .ToListAsync();

                dashboardData.RevenuMois = reservationsThisMonth.Sum(r => r.CoutTotal);

                // Get last month's revenue for comparison
                var lastMonthStart = startOfMonth.AddMonths(-1);
                var lastMonthEnd = startOfMonth;
                var reservationsLastMonth = await _context.Reservations
                    .Where(r => r.DateDebut >= lastMonthStart && r.DateDebut < lastMonthEnd)
                    .ToListAsync();

                var revenueLastMonth = reservationsLastMonth.Sum(r => r.CoutTotal);
                dashboardData.PourcentageChangementRevenu = revenueLastMonth > 0
                    ? (int)((dashboardData.RevenuMois - revenueLastMonth) / revenueLastMonth * 100)
                    : 100;

                // Get recent reservations (last 10)
                dashboardData.ReservationsRecentes = await _context.Reservations
                    .Include(r => r.user)
                    .Include(r => r.Chambre)
                    .OrderByDescending(r => r.DateDebut)
                    .Take(10)
                    .Select(r => new ReservationResume
                    {
                        Id = r.Id,
                        ClientNom = r.user.Nom + " " + r.user.Prenom,
                        ChambreNumero = r.Chambre.Numero,
                        DateDebut = r.DateDebut,
                        DateFin = r.DateFin,
                        CoutTotal = r.CoutTotal,
                        Statut = r.Statut
                    })
                    .ToListAsync();

                // Get popular rooms (most booked)
                dashboardData.ChambresPopulaires = await _context.Chambres
                    .Include(c => c.Reservations)
                    .OrderByDescending(c => c.Reservations.Count)
                    .Take(3)
                    .Select(c => new ChambreResume
                    {
                        Id = c.Id,
                        Numero = c.Numero,
                        Type = c.Type,
                        Image = c.image,
                        Capacite = c.Capacite,
                        Tarif = c.Tarif,
                        Etat = c.Etat,
                        NombreReservations = c.Reservations.Count
                    })
                    .ToListAsync();

                return View(dashboardData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading GerantDashboard");
                // Return view with empty data if error
                return View(new GerantDashboardViewModel());
            }
        }



    }


}
