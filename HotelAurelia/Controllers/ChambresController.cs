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
    public class ChambresController : Controller
    {
        private readonly HotelAureliaDbContext _context;

        public ChambresController(HotelAureliaDbContext context)
        {
            _context = context;
        }

        // GET: Chambres
        public async Task<IActionResult> Index()
        {
            return View(await _context.Chambres.ToListAsync());
        }

        // GET: Chambres/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chambre = await _context.Chambres
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chambre == null)
            {
                return NotFound();
            }

            return View(chambre);
        }

        // GET: Chambres/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Chambres/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
       
public async Task<IActionResult> Create([Bind("Id,Numero,Type,Capacite,Tarif,Etat")] Chambre chambre, IFormFile imageFile)
    {
        if (ModelState.IsValid)
        {
            // Handle image upload
            if (imageFile != null && imageFile.Length > 0)
            {
                // Generate unique filename
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/image/chambres", fileName);

                // Ensure directory exists
                var directoryPath = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                // Save the file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                // Save the filename to the database
                chambre.image = fileName;
            }

            _context.Add(chambre);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(chambre);
    }

    // GET: Chambres/Edit/5
    public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chambre = await _context.Chambres.FindAsync(id);
            if (chambre == null)
            {
                return NotFound();
            }
            return View(chambre);
        }

        // POST: Chambres/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Numero,Type,Capacite,Tarif,Etat")] Chambre chambre)
        {
            if (id != chambre.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chambre);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChambreExists(chambre.Id))
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
            return View(chambre);
        }

        // GET: Chambres/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chambre = await _context.Chambres
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chambre == null)
            {
                return NotFound();
            }

            return View(chambre);
        }

        // POST: Chambres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var chambre = await _context.Chambres.FindAsync(id);
            if (chambre != null)
            {
                _context.Chambres.Remove(chambre);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChambreExists(int id)
        {
            return _context.Chambres.Any(e => e.Id == id);
        }
    }
}
