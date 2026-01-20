using HotelAurelia.Areas.Identity.Data;
using HotelAurelia.Models;
using HotelAurelia.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace HotelAurelia.Controllers
{
    public class ReceptionnistesController : Controller
    {
        private readonly IEmailSender _emailSender;
        private readonly HotelAureliaDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _identityContext;
        private readonly ILogger<ReceptionnistesController> _logger;
        public ReceptionnistesController(HotelAureliaDbContext context , UserManager<User> userManager , ApplicationDbContext identityContext,
            ILogger<ReceptionnistesController> logger,
    IEmailSender emailSender)
        {
            _userManager = userManager;
            _context = context;
            _identityContext = identityContext;
            _logger = logger;
            _emailSender = emailSender;
        }
        // GET: /Receptionniste/Reservations
        public async Task<IActionResult> Reservations(string search = "", string status = "", string sortBy = "date", string sortOrder = "desc")
        {
            try
            {
                // Start with base query
                var query = _context.Reservations
                    .Include(r => r.user)
                    .Include(r => r.Chambre)
                    .Include(r => r.Service)
                    .AsQueryable();

                // Apply search filter
                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(r =>
                        r.user.Nom.Contains(search) ||
                        r.user.Prenom.Contains(search) ||
                        r.user.Email.Contains(search) ||
                        r.user.PhoneNumber.Contains(search) ||
                        r.Chambre.Numero.Contains(search) ||
                        (r.Service != null && r.Service.Nom.Contains(search))
                    );
                }

                // Apply status filter
                if (!string.IsNullOrEmpty(status) && Enum.TryParse<StatutReservation>(status, out var statusEnum))
                {
                    query = query.Where(r => r.Statut == statusEnum);
                }

                // Apply sorting
                query = sortBy.ToLower() switch
                {
                    "client" => sortOrder == "asc"
                        ? query.OrderBy(r => r.user.Nom).ThenBy(r => r.user.Prenom)
                        : query.OrderByDescending(r => r.user.Nom).ThenByDescending(r => r.user.Prenom),
                    "chambre" => sortOrder == "asc"
                        ? query.OrderBy(r => r.Chambre.Numero)
                        : query.OrderByDescending(r => r.Chambre.Numero),
                    "date" => sortOrder == "asc"
                        ? query.OrderBy(r => r.DateDebut)
                        : query.OrderByDescending(r => r.DateDebut),
                    "total" => sortOrder == "asc"
                        ? query.OrderBy(r => r.CoutTotal)
                        : query.OrderByDescending(r => r.CoutTotal),
                    _ => sortOrder == "asc"
                        ? query.OrderBy(r => r.DateDebut)
                        : query.OrderByDescending(r => r.DateDebut)
                };

                // Execute query and map to view model
                var reservations = await query
                    .Select(r => new ReservationDetailsViewModel
                    {
                        Id = r.Id,
                        DateDebut = r.DateDebut,
                        DateFin = r.DateFin,
                        CoutTotal = r.CoutTotal,
                        Statut = r.Statut,

                        // User info
                        UserId = r.user.Id,
                        UserNom = r.user.Nom,
                        UserPrenom = r.user.Prenom,
                        UserEmail = r.user.Email ?? string.Empty,
                        UserPhone = r.user.PhoneNumber ?? string.Empty,
                        UserTel = r.user.Tel ?? string.Empty,

                        // Room info
                        ChambreId = r.chambreId,
                        ChambreNumero = r.Chambre.Numero,
                        ChambreType = r.Chambre.Type ?? "Standard",
                        ChambreTarif = r.Chambre.Tarif,
                        ChambreImage = r.Chambre.image,

                        // Service info
                        ServiceId = r.ServiceId,
                        ServiceNom = r.Service != null ? r.Service.Nom : null,
                        ServiceDescription = r.Service != null ? r.Service.Description : null,
                        ServiceTarif = r.Service != null ? r.Service.Tarif : null
                    })
                    .ToListAsync();

                // Pass filter values to view
                ViewBag.Search = search;
                ViewBag.Status = status;
                ViewBag.SortBy = sortBy;
                ViewBag.SortOrder = sortOrder;

                // Get counts for status filter
                ViewBag.TotalCount = await _context.Reservations.CountAsync();
                ViewBag.PendingCount = await _context.Reservations.CountAsync(r => r.Statut == StatutReservation.EnAttente);
                ViewBag.ConfirmedCount = await _context.Reservations.CountAsync(r => r.Statut == StatutReservation.Confirmee);
                ViewBag.PaidCount = await _context.Reservations.CountAsync(r => r.Statut == StatutReservation.Payee);
                ViewBag.CancelledCount = await _context.Reservations.CountAsync(r => r.Statut == StatutReservation.Annulee);

                return View(reservations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading reservations for receptionniste");
                TempData["ErrorMessage"] = "Une erreur est survenue lors du chargement des réservations.";
                return View(new List<ReservationDetailsViewModel>());
            }
        }

        // GET: /Receptionniste/ReservationDetails/5
        public async Task<IActionResult> ReservationDetails(int id)
        {
            try
            {
                var reservation = await _context.Reservations
                    .Include(r => r.user)
                    .Include(r => r.Chambre)
                    .Include(r => r.Service)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (reservation == null)
                {
                    TempData["ErrorMessage"] = "Réservation non trouvée.";
                    return RedirectToAction(nameof(Reservations));
                }

                var viewModel = new ReservationDetailsViewModel
                {
                    Id = reservation.Id,
                    DateDebut = reservation.DateDebut,
                    DateFin = reservation.DateFin,
                    CoutTotal = reservation.CoutTotal,
                    Statut = reservation.Statut,

                    // User info
                    UserId = reservation.user.Id,
                    UserNom = reservation.user.Nom,
                    UserPrenom = reservation.user.Prenom,
                    UserEmail = reservation.user.Email ?? string.Empty,
                    UserPhone = reservation.user.PhoneNumber ?? string.Empty,
                    UserTel = reservation.user.Tel ?? string.Empty,

                    // Room info
                    ChambreId = reservation.chambreId,
                    ChambreNumero = reservation.Chambre.Numero,
                    ChambreType = reservation.Chambre.Type ?? "Standard",
                    ChambreTarif = reservation.Chambre.Tarif,
                    ChambreImage = reservation.Chambre.image,

                    // Service info
                    ServiceId = reservation.ServiceId,
                    ServiceNom = reservation.Service?.Nom,
                    ServiceDescription = reservation.Service?.Description,
                    ServiceTarif = reservation.Service?.Tarif
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading reservation details");
                TempData["ErrorMessage"] = "Une erreur est survenue lors du chargement des détails.";
                return RedirectToAction(nameof(Reservations));
            }
        }

        // POST: /Receptionniste/UpdateStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, StatutReservation newStatus)
        {
            try
            {
                var reservation = await _context.Reservations.FindAsync(id);

                if (reservation == null)
                {
                    TempData["ErrorMessage"] = "Réservation non trouvée.";
                    return RedirectToAction(nameof(Reservations));
                }

                reservation.Statut = newStatus;
                _context.Update(reservation);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Statut de la réservation #{id} mis à jour avec succès.";
                return RedirectToAction(nameof(ReservationDetails), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating reservation status");
                TempData["ErrorMessage"] = "Une erreur est survenue lors de la mise à jour du statut.";
                return RedirectToAction(nameof(ReservationDetails), new { id });
            }
        }

        // GET: /Receptionniste/Clients
        public async Task<IActionResult> Clients(string search = "")
        {
            try
            {
                // Get all users with role "Client"
                var query = _identityContext.Users
                    .Where(u => u.Role == "Client")
                    .AsQueryable();

                // Apply search filter
                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(u =>
                        u.Nom.Contains(search) ||
                        u.Prenom.Contains(search) ||
                        u.Email.Contains(search) ||
                        u.PhoneNumber.Contains(search) ||
                        u.Tel.Contains(search)
                    );
                }

                var clients = await query
                    .OrderBy(u => u.Nom)
                    .ThenBy(u => u.Prenom)
                    .ToListAsync();

                // Get reservation counts for each client
                var clientReservations = new Dictionary<string, int>();
                foreach (var client in clients)
                {
                    var count = await _context.Reservations
                        .CountAsync(r => EF.Property<string>(r, "UserId") == client.Id);
                    clientReservations[client.Id] = count;
                }

                ViewBag.ClientReservations = clientReservations;
                ViewBag.Search = search;

                return View(clients);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading clients");
                TempData["ErrorMessage"] = "Une erreur est survenue lors du chargement des clients.";
                return View(new List<User>());
            }
        }

        // GET: /Receptionniste/ClientDetails/{id}
        public async Task<IActionResult> ClientDetails(string id)
        {
            try
            {
                var client = await _identityContext.Users
                    .FirstOrDefaultAsync(u => u.Id == id && u.Role == "Client");

                if (client == null)
                {
                    TempData["ErrorMessage"] = "Client non trouvé.";
                    return RedirectToAction(nameof(Clients));
                }

                // Get client's reservations
                var reservations = await _context.Reservations
                    .Include(r => r.Chambre)
                    .Include(r => r.Service)
                    .Where(r => EF.Property<string>(r, "UserId") == id)
                    .OrderByDescending(r => r.DateDebut)
                    .Select(r => new ReservationDetailsViewModel
                    {
                        Id = r.Id,
                        DateDebut = r.DateDebut,
                        DateFin = r.DateFin,
                        CoutTotal = r.CoutTotal,
                        Statut = r.Statut,
                        ChambreNumero = r.Chambre.Numero,
                        ChambreType = r.Chambre.Type ?? "Standard",
                        ServiceNom = r.Service != null ? r.Service.Nom : null
                    })
                    .ToListAsync();

                ViewBag.Reservations = reservations;
                ViewBag.TotalReservations = reservations.Count;
                ViewBag.TotalSpent = reservations.Sum(r => r.CoutTotal);

                return View(client);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading client details");
                TempData["ErrorMessage"] = "Une erreur est survenue lors du chargement des détails du client.";
                return RedirectToAction(nameof(Clients));
            }
        }
    


        // GET: Receptionnistes
        public async Task<IActionResult> Index()
        {
            var receptionnistes = await _context.Receptionnistes
                .Include(r => r.User)  // ✅ Load related User data
                .ToListAsync();

            return View(receptionnistes);
        }



        // GET: Receptionnistes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var receptionniste = await _context.Receptionnistes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (receptionniste == null)
            {
                return NotFound();
            }

            return View(receptionniste);
        }

        // GET: Receptionnistes/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateReceptionnisteViewModel());
        }

        [HttpPost]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateReceptionnisteViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // 1. Create User
                    var user = new User
                    {
                        UserName = model.Email,
                        Email = model.Email,
                        NormalizedEmail = model.Email.ToUpper(),
                        Nom = model.Nom,
                        Prenom = model.Prenom,
                        Tel = model.Tel,
                        Role = "Receptionniste"
                    };

                    var result = await _userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        // 2. Assign role
                        await _userManager.AddToRoleAsync(user, "Receptionniste");

                        // 3. Create Receptionniste
                        var receptionniste = new Receptionniste
                        {
                            UserId = user.Id,
                            Poste = model.Poste,
                            User = user
                        };

                        _context.Receptionnistes.Add(receptionniste);
                        await _context.SaveChangesAsync();

                        // 4. Send email with credentials
                        await SendWelcomeEmail(user, model.Password);

                        TempData["SuccessMessage"] = $"Réceptionniste créé avec succès. Un email a été envoyé à {user.Email} avec les identifiants de connexion.";
                        return RedirectToAction(nameof(Index));
                    }

                    // Errors from user creation
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating receptionniste");
                    ModelState.AddModelError(string.Empty, "Une erreur est survenue lors de la création du réceptionniste.");
                }
            }
            return View(model);
        }

        private async Task SendWelcomeEmail(User user, string password)
        {
            try
            {
                var subject = "Bienvenue chez Hotel Aurelia - Vos identifiants de connexion";

                var body = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #FFD700 0%, #e6c200 100%); color: #2C3E50; padding: 20px; text-align: center; border-radius: 8px 8px 0 0; }}
        .content {{ background: #f8f9fa; padding: 30px; border-radius: 0 0 8px 8px; }}
        .credentials {{ background: white; padding: 20px; border-radius: 6px; border-left: 4px solid #FFD700; margin: 20px 0; }}
        .footer {{ text-align: center; margin-top: 30px; color: #6c757d; font-size: 0.9rem; }}
        .important {{ color: #dc3545; font-weight: bold; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Hotel Aurelia</h1>
            <h2>Bienvenue dans notre équipe !</h2>
        </div>
        
        <div class='content'>
            <p>Bonjour {user.Prenom} {user.Nom},</p>
            
            <p>Votre compte réceptionniste a été créé avec succès sur le système de gestion de l'Hôtel Aurelia.</p>
            
            <div class='credentials'>
                <h3>Vos identifiants de connexion :</h3>
                <p><strong>Email :</strong> {user.Email}</p>
                <p><strong>Mot de passe :</strong> {password}</p>
                <p><strong>Poste :</strong> Réceptionniste</p>
                
                <p class='important'>⚠️ Pour des raisons de sécurité, nous vous recommandons de changer votre mot de passe dès votre première connexion.</p>
            </div>
            
            <p><strong>Pour vous connecter :</strong></p>
            <ol>
                <li>Rendez-vous sur : <a href='{GetLoginUrl()}'>Page de connexion</a></li>
                <li>Utilisez les identifiants ci-dessus</li>
                <li>Accédez à votre tableau de bord réceptionniste</li>
            </ol>
            
            <p><strong>Vos responsabilités :</strong></p>
            <ul>
                <li>Gestion des réservations</li>
                <li>Accueil des clients</li>
                <li>Suivi des chambres</li>
                <li>Gestion des services additionnels</li>
            </ul>
            
            <p>Pour toute question concernant votre compte ou vos responsabilités, veuillez contacter le gérant.</p>
            
            <p>Cordialement,<br>
            <strong>L'équipe de direction<br>
            Hotel Aurelia</strong></p>
        </div>
        
        <div class='footer'>
            <p>Cet email a été envoyé automatiquement. Merci de ne pas y répondre.</p>
            <p>© {DateTime.Now.Year} Hotel Aurelia. Tous droits réservés.</p>
        </div>
    </div>
</body>
</html>";

                await SendEmailAsync(user.Email, subject, body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending welcome email to receptionniste {Email}", user.Email);
                // Don't throw the exception - we don't want to fail the creation if email fails
            }
        }
        private async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                await _emailSender.SendEmailAsync(email, subject, htmlMessage);
                _logger.LogInformation("Email sent successfully to {Email}", email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to {Email}", email);
                // Don't throw - just log the error
            }
        }

        private string GetLoginUrl()
        {
            // Get the base URL from the current request
            var request = HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            return $"{baseUrl}/Identity/Account/Login";
        }

        // POST: Receptionnistes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,Poste,UserId")] Receptionniste receptionniste)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(receptionniste);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(receptionniste);
        //}

        // GET: Receptionnistes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var receptionniste = await _context.Receptionnistes.FindAsync(id);
            if (receptionniste == null)
            {
                return NotFound();
            }
            return View(receptionniste);
        }

        // POST: Receptionnistes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Poste,UserId")] Receptionniste receptionniste)
        {
            if (id != receptionniste.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(receptionniste);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReceptionnisteExists(receptionniste.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(receptionniste);
        }

        // GET: Receptionnistes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var receptionniste = await _context.Receptionnistes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (receptionniste == null)
            {
                return NotFound();
            }

            return View(receptionniste);
        }

        // POST: Receptionnistes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // 1. Find the Receptionniste in HotelAureliaDbContext
            var receptionniste = await _context.Receptionnistes.FindAsync(id);

            if (receptionniste != null)
            {
                // 2. Get the User ID from Receptionniste
                var userId = receptionniste.UserId;

                // 3. Remove Receptionniste from HotelAureliaDbContext
                _context.Receptionnistes.Remove(receptionniste);

                // 4. Save changes to Receptionnistes table FIRST
                await _context.SaveChangesAsync();

                // 5. Now remove the User from ApplicationDbContext (AspNetUsers)
                // You need to inject ApplicationDbContext in your controller constructor
                var user = await _identityContext.Users.FindAsync(userId);
                if (user != null)
                {
                    _identityContext.Users.Remove(user);
                    await _identityContext.SaveChangesAsync();
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ReceptionnisteExists(int id)   
        {
            return _context.Receptionnistes.Any(e => e.Id == id);
        }
    }
}
