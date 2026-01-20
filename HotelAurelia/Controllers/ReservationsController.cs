using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotelAurelia.Models;

namespace HotelAurelia.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly HotelAureliaDbContext _context;

        public ReservationsController(HotelAureliaDbContext context)
        {
            _context = context;
        }

        // GET: Reservations
        public async Task<IActionResult> Index()
        {
            var hotelAureliaDbContext = _context.Reservations.Include(r => r.Chambre).Include(r => r.Service);
            return View(await hotelAureliaDbContext.ToListAsync());
        }

        // GET: Reservations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Chambre)
                .Include(r => r.Service)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // GET: Reservations/Create
        public IActionResult Create()
        {
            ViewData["chambreId"] = new SelectList(_context.Chambres, "Id", "Numero");
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Nom");
            return View();
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DateDebut,DateFin,Statut,CoutTotal,chambreId,ServiceId")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["chambreId"] = new SelectList(_context.Chambres, "Id", "Numero", reservation.chambreId);
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Nom", reservation.ServiceId);
            return View(reservation);
        }

        // GET: Reservations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            ViewData["chambreId"] = new SelectList(_context.Chambres, "Id", "Numero", reservation.chambreId);
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Nom", reservation.ServiceId);
            return View(reservation);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DateDebut,DateFin,Statut,CoutTotal,chambreId,ServiceId")] Reservation reservation)
        {
            if (id != reservation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationExists(reservation.Id))
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
            ViewData["chambreId"] = new SelectList(_context.Chambres, "Id", "Numero", reservation.chambreId);
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Nom", reservation.ServiceId);
            return View(reservation);
        }

        // GET: Reservations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Chambre)
                .Include(r => r.Service)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                // Find the reservation with its room
                var reservation = await _context.Reservations
                    .Include(r => r.Chambre) // Include the room
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (reservation != null)
                {
                    // Update room status to Available
                    if (reservation.Chambre != null)
                    {
                        reservation.Chambre.Etat = EtatChambre.Disponible;
                        _context.Update(reservation.Chambre);
                    }

                    // Remove the reservation
                    _context.Reservations.Remove(reservation);
                }

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Reservation deleted successfully and room is now available.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Log the error
                

                TempData["ErrorMessage"] = "An error occurred while deleting the reservation.";
                return RedirectToAction(nameof(Index));
            }
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.Id == id);
        }
    }
}
