using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TP4.Models;

namespace TP4.Controllers
{
    public class UsuarioCajaDeAhorrosController : Controller
    {
        private readonly MyContext _context;

        public UsuarioCajaDeAhorrosController(MyContext context)
        {
            _context = context;
        }

        // GET: UsuarioCajaDeAhorros
        public async Task<IActionResult> Index()
        {
            var myContext = _context.UsuarioCajaDeAhorro.Include(u => u.caja).Include(u => u.user);
            return View(await myContext.ToListAsync());
        }

        // GET: UsuarioCajaDeAhorros/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.UsuarioCajaDeAhorro == null)
            {
                return NotFound();
            }

            var usuarioCajaDeAhorro = await _context.UsuarioCajaDeAhorro
                .Include(u => u.caja)
                .Include(u => u.user)
                .FirstOrDefaultAsync(m => m._id_caja == id);
            if (usuarioCajaDeAhorro == null)
            {
                return NotFound();
            }

            return View(usuarioCajaDeAhorro);
        }

        // GET: UsuarioCajaDeAhorros/Create
        public IActionResult Create()
        {
            ViewData["_id_caja"] = new SelectList(_context.cajas, "_id_caja", "_cbu");
            ViewData["_id_usuario"] = new SelectList(_context.usuarios, "_id_usuario", "_id_usuario");
            return View();
        }

        // POST: UsuarioCajaDeAhorros/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("_id_caja,_id_usuario")] UsuarioCajaDeAhorro usuarioCajaDeAhorro)
        {
            if (ModelState.IsValid)
            {
                _context.Add(usuarioCajaDeAhorro);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["_id_caja"] = new SelectList(_context.cajas, "_id_caja", "_cbu", usuarioCajaDeAhorro._id_caja);
            ViewData["_id_usuario"] = new SelectList(_context.usuarios, "_id_usuario", "_id_usuario", usuarioCajaDeAhorro._id_usuario);
            return View(usuarioCajaDeAhorro);
        }

        // GET: UsuarioCajaDeAhorros/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.UsuarioCajaDeAhorro == null)
            {
                return NotFound();
            }

            var usuarioCajaDeAhorro = await _context.UsuarioCajaDeAhorro.FindAsync(id);
            if (usuarioCajaDeAhorro == null)
            {
                return NotFound();
            }
            ViewData["_id_caja"] = new SelectList(_context.cajas, "_id_caja", "_cbu", usuarioCajaDeAhorro._id_caja);
            ViewData["_id_usuario"] = new SelectList(_context.usuarios, "_id_usuario", "_id_usuario", usuarioCajaDeAhorro._id_usuario);
            return View(usuarioCajaDeAhorro);
        }

        // POST: UsuarioCajaDeAhorros/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("_id_caja,_id_usuario")] UsuarioCajaDeAhorro usuarioCajaDeAhorro)
        {
            if (id != usuarioCajaDeAhorro._id_caja)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuarioCajaDeAhorro);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioCajaDeAhorroExists(usuarioCajaDeAhorro._id_caja))
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
            ViewData["_id_caja"] = new SelectList(_context.cajas, "_id_caja", "_cbu", usuarioCajaDeAhorro._id_caja);
            ViewData["_id_usuario"] = new SelectList(_context.usuarios, "_id_usuario", "_id_usuario", usuarioCajaDeAhorro._id_usuario);
            return View(usuarioCajaDeAhorro);
        }

        // GET: UsuarioCajaDeAhorros/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.UsuarioCajaDeAhorro == null)
            {
                return NotFound();
            }

            var usuarioCajaDeAhorro = await _context.UsuarioCajaDeAhorro
                .Include(u => u.caja)
                .Include(u => u.user)
                .FirstOrDefaultAsync(m => m._id_caja == id);
            if (usuarioCajaDeAhorro == null)
            {
                return NotFound();
            }

            return View(usuarioCajaDeAhorro);
        }

        // POST: UsuarioCajaDeAhorros/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.UsuarioCajaDeAhorro == null)
            {
                return Problem("Entity set 'MyContext.UsuarioCajaDeAhorro'  is null.");
            }
            var usuarioCajaDeAhorro = await _context.UsuarioCajaDeAhorro.FindAsync(id);
            if (usuarioCajaDeAhorro != null)
            {
                _context.UsuarioCajaDeAhorro.Remove(usuarioCajaDeAhorro);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioCajaDeAhorroExists(int id)
        {
          return _context.UsuarioCajaDeAhorro.Any(e => e._id_caja == id);
        }
    }
}
