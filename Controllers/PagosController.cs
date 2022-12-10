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
    public class PagosController : Controller
    {
        private readonly MyContext _context;

        public PagosController(MyContext context)
        {
            _context = context;
        }

        // GET: Pagos
        public async Task<IActionResult> Index()
        {
            var myContext = _context.pagos.Include(p => p._usuario);
            return View(await myContext.ToListAsync());
        }

        // GET: Pagos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.pagos == null)
            {
                return NotFound();
            }

            var pago = await _context.pagos
                .Include(p => p._usuario)
                .FirstOrDefaultAsync(m => m._id_pago == id);
            if (pago == null)
            {
                return NotFound();
            }

            return View(pago);
        }

        // GET: Pagos/Create
        public IActionResult Create()
        {
            ViewData["_id_usuario"] = new SelectList(_context.usuarios, "_id_usuario", "_id_usuario");
            return View();
        }

        // POST: Pagos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("_id_pago,_id_usuario,_monto,_pagado,_metodo,_detalle,_id_metodo")] Pago pago)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pago);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["_id_usuario"] = new SelectList(_context.usuarios, "_id_usuario", "_id_usuario", pago._id_usuario);
            return View(pago);
        }

        // GET: Pagos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.pagos == null)
            {
                return NotFound();
            }

            var pago = await _context.pagos.FindAsync(id);
            if (pago == null)
            {
                return NotFound();
            }
            ViewData["_id_usuario"] = new SelectList(_context.usuarios, "_id_usuario", "_id_usuario", pago._id_usuario);
            return View(pago);
        }

        // POST: Pagos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("_id_pago,_id_usuario,_monto,_pagado,_metodo,_detalle,_id_metodo")] Pago pago)
        {
            if (id != pago._id_pago)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pago);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PagoExists(pago._id_pago))
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
            ViewData["_id_usuario"] = new SelectList(_context.usuarios, "_id_usuario", "_id_usuario", pago._id_usuario);
            return View(pago);
        }

        // GET: Pagos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.pagos == null)
            {
                return NotFound();
            }

            var pago = await _context.pagos
                .Include(p => p._usuario)
                .FirstOrDefaultAsync(m => m._id_pago == id);
            if (pago == null)
            {
                return NotFound();
            }

            return View(pago);
        }

        // POST: Pagos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.pagos == null)
            {
                return Problem("Entity set 'MyContext.pagos'  is null.");
            }
            var pago = await _context.pagos.FindAsync(id);
            if (pago != null)
            {
                _context.pagos.Remove(pago);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PagoExists(int id)
        {
          return _context.pagos.Any(e => e._id_pago == id);
        }
    }
}
