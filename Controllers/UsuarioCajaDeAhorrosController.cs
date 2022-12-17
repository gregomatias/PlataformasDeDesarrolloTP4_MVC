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
    public class UsuarioCajaDeAhorrosController : Controller
    {
        private readonly MyContext _context;

        public UsuarioCajaDeAhorrosController(MyContext context)
        {
            _context = context;
        }

        // GET: UsuarioCajaDeAhorros
        public async Task<IActionResult> Index(int? id)
        {
            var myContext = _context.UsuarioCajaDeAhorro.Include(u => u.caja).Include(u => u.user);
            ViewBag.id = id;
            return View(await myContext.ToListAsync());

         
        }

   

        // GET: UsuarioCajaDeAhorros/Create
        public IActionResult Create(int id)
        {
            ViewData["_id_caja"] = new SelectList(_context.cajas, "_id_caja", "_cbu");
            ViewData["_id_usuario"] = new SelectList(_context.usuarios, "_id_usuario", "_id_usuario");
            ViewBag.id = id;
            return View();
        }

        // POST: UsuarioCajaDeAhorros/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int id,[Bind("_id_caja,_id_usuario")] UsuarioCajaDeAhorro usuarioCajaDeAhorro)
        {
            if (ModelState.IsValid)
            {
                _context.Add(usuarioCajaDeAhorro);
                await _context.SaveChangesAsync();
                ViewBag.id = id;
                return RedirectToAction("Index", new { id = id });
            }
            ViewData["_id_caja"] = new SelectList(_context.cajas, "_id_caja", "_cbu", usuarioCajaDeAhorro._id_caja);
            ViewData["_id_usuario"] = new SelectList(_context.usuarios, "_id_usuario", "_id_usuario", usuarioCajaDeAhorro._id_usuario);
            ViewBag.id = id;
            return View(usuarioCajaDeAhorro);
        }

   

        // GET: UsuarioCajaDeAhorros/Delete/5
        public async Task<IActionResult> Delete(int? id,int _id_caja,int _id_usuario)
        {
 

            var usuarioCajaDeAhorro = await _context.UsuarioCajaDeAhorro
                .Include(u => u.caja)
                .Include(u => u.user)
                .FirstOrDefaultAsync(m => m._id_caja == _id_caja && m._id_usuario==_id_usuario);
            if (usuarioCajaDeAhorro == null)
            {
                return NotFound();
            }

            ViewBag.id = id;
            return View(usuarioCajaDeAhorro);
        }

        // POST: UsuarioCajaDeAhorros/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id,int _id_caja,int _id_usuario)
        {
            if (_context.UsuarioCajaDeAhorro == null)
            {
                return Problem("Entity set 'MyContext.UsuarioCajaDeAhorro'  is null.");
            }
            var usuarioCajaDeAhorro = await _context.UsuarioCajaDeAhorro.FindAsync(_id_caja, _id_usuario);
            if (usuarioCajaDeAhorro != null)
            {
                _context.UsuarioCajaDeAhorro.Remove(usuarioCajaDeAhorro);
            }
            
            await _context.SaveChangesAsync();
            ViewBag.id = id;
            return RedirectToAction("Index", new { id = id });
        }

        private bool UsuarioCajaDeAhorroExists(int id)
        {
          return _context.UsuarioCajaDeAhorro.Any(e => e._id_caja == id);
        }
    }
}
