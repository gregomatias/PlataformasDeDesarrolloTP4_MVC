using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TP4.Models;

namespace TP4.Controllers
{
    [Authorize]
    public class CajaDeAhorrosController : Controller
    {
        private readonly MyContext _context;

        public CajaDeAhorrosController(MyContext context)
        {
            _context = context;
        }

        // GET: CajaDeAhorros
        public async Task<IActionResult> Index(int? id)

        {
           
             _context.usuarios.Include(u => u.cajas).Load();
            Usuario usuario = _context.usuarios.Where(u => u._id_usuario == id).FirstOrDefault();



            if (usuario != null)
            {
                if (usuario._esUsuarioAdmin)
                {
                    return View(await _context.cajas.ToListAsync());
                }
                else
                {
                    return View( usuario.cajas.ToList());
                }
            }

            return NotFound();

        }

        // GET: CajaDeAhorros/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.cajas == null)
            {
                return NotFound();
            }

            var cajaDeAhorro = await _context.cajas
                .FirstOrDefaultAsync(m => m._id_caja == id);
            if (cajaDeAhorro == null)
            {
                return NotFound();
            }

            return View(cajaDeAhorro);
        }

        // GET: CajaDeAhorros/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CajaDeAhorros/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("_id_caja,_cbu,_saldo")] CajaDeAhorro cajaDeAhorro)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cajaDeAhorro);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cajaDeAhorro);
        }

        // GET: CajaDeAhorros/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.cajas == null)
            {
                return NotFound();
            }

            var cajaDeAhorro = await _context.cajas.FindAsync(id);
            if (cajaDeAhorro == null)
            {
                return NotFound();
            }
            return View(cajaDeAhorro);
        }

        // POST: CajaDeAhorros/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("_id_caja,_cbu,_saldo")] CajaDeAhorro cajaDeAhorro)
        {
            if (id != cajaDeAhorro._id_caja)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cajaDeAhorro);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CajaDeAhorroExists(cajaDeAhorro._id_caja))
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
            return View(cajaDeAhorro);
        }

        // GET: CajaDeAhorros/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.cajas == null)
            {
                return NotFound();
            }

            var cajaDeAhorro = await _context.cajas
                .FirstOrDefaultAsync(m => m._id_caja == id);
            if (cajaDeAhorro == null)
            {
                return NotFound();
            }

            return View(cajaDeAhorro);
        }

        // POST: CajaDeAhorros/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.cajas == null)
            {
                return Problem("Entity set 'MyContext.cajas'  is null.");
            }
            var cajaDeAhorro = await _context.cajas.FindAsync(id);
            if (cajaDeAhorro != null)
            {
                _context.cajas.Remove(cajaDeAhorro);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CajaDeAhorroExists(int id)
        {
          return _context.cajas.Any(e => e._id_caja == id);
        }
    }
}
