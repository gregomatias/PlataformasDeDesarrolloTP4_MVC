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
    public class PlazoFijosController : Controller
    {
        private readonly MyContext _context;

        public PlazoFijosController(MyContext context)
        {
            _context = context;
        }

        // GET: PlazoFijos
        public async Task<IActionResult> Index()
        {
            var myContext = _context.plazosFijos.Include(p => p._titular);
            return View(await myContext.ToListAsync());
        }

        // GET: PlazoFijos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.plazosFijos == null)
            {
                return NotFound();
            }

            var plazoFijo = await _context.plazosFijos
                .Include(p => p._titular)
                .FirstOrDefaultAsync(m => m._id_plazoFijo == id);
            if (plazoFijo == null)
            {
                return NotFound();
            }

            return View(plazoFijo);
        }

        // GET: PlazoFijos/Create
        public IActionResult Create()
        {
            ViewData["_id_usuario"] = new SelectList(_context.usuarios, "_id_usuario", "_id_usuario");
            return View();
        }

        // POST: PlazoFijos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("_id_plazoFijo,_id_usuario,_monto,_fechaIni,_fechaFin,_tasa,_pagado")] PlazoFijo plazoFijo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(plazoFijo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["_id_usuario"] = new SelectList(_context.usuarios, "_id_usuario", "_id_usuario", plazoFijo._id_usuario);
            return View(plazoFijo);
        }

        // GET: PlazoFijos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.plazosFijos == null)
            {
                return NotFound();
            }

            var plazoFijo = await _context.plazosFijos.FindAsync(id);
            if (plazoFijo == null)
            {
                return NotFound();
            }
            ViewData["_id_usuario"] = new SelectList(_context.usuarios, "_id_usuario", "_id_usuario", plazoFijo._id_usuario);
            return View(plazoFijo);
        }

        // POST: PlazoFijos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("_id_plazoFijo,_id_usuario,_monto,_fechaIni,_fechaFin,_tasa,_pagado")] PlazoFijo plazoFijo)
        {
            if (id != plazoFijo._id_plazoFijo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(plazoFijo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlazoFijoExists(plazoFijo._id_plazoFijo))
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
            ViewData["_id_usuario"] = new SelectList(_context.usuarios, "_id_usuario", "_id_usuario", plazoFijo._id_usuario);
            return View(plazoFijo);
        }

        // GET: PlazoFijos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.plazosFijos == null)
            {
                return NotFound();
            }

            var plazoFijo = await _context.plazosFijos
                .Include(p => p._titular)
                .FirstOrDefaultAsync(m => m._id_plazoFijo == id);
            if (plazoFijo == null)
            {
                return NotFound();
            }

            return View(plazoFijo);
        }

        // POST: PlazoFijos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.plazosFijos == null)
            {
                return Problem("Entity set 'MyContext.plazosFijos'  is null.");
            }
            var plazoFijo = await _context.plazosFijos.FindAsync(id);
            if (plazoFijo != null)
            {
                _context.plazosFijos.Remove(plazoFijo);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlazoFijoExists(int id)
        {
          return _context.plazosFijos.Any(e => e._id_plazoFijo == id);
        }
    }
}
