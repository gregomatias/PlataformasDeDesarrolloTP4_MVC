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
    //Solo el admin EsAdmin=True
    [Authorize(Roles = "True")]
    public class UsuariosController : Controller
    {
        private readonly MyContext _context;

        public UsuariosController(MyContext context)
        {
            _context = context;
        }

        // GET: Usuarios
        
        public async Task<IActionResult> Index(int? id)
        {


            ViewBag.id = id;
            return View(await _context.usuarios.ToListAsync());
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id, int id_usuario)
        {
            if (id_usuario == null || _context.usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.usuarios
                .FirstOrDefaultAsync(m => m._id_usuario == id_usuario);
            if (usuario == null)
            {
                return NotFound();
            }

            ViewBag.id = id;
            return View(usuario);
        }

        // GET: Usuarios/Create
        public IActionResult Create(int id)
        {
            ViewBag.id = id;
            return View();
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int id,[Bind("_id_usuario,_dni,_nombre,_apellido,_mail,_password,_intentosFallidos,_esUsuarioAdmin,_bloqueado")] Usuario usuario)
        {

   



            if (ModelState.IsValid)
            {
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { id = id });
            }
            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id,int? id_usuario)
        {
            if (id_usuario == null || _context.usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.usuarios.FindAsync(id_usuario);
            if (usuario == null)
            {
                return NotFound();
            }
            ViewBag.id = id;
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("_id_usuario,_dni,_nombre,_apellido,_mail,_password,_intentosFallidos,_esUsuarioAdmin,_bloqueado")] Usuario usuario)
        {


            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario._id_usuario))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                ViewBag.id = id;
                return RedirectToAction("Index", new { id = id });
            }
            ViewBag.id = id;
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id , int id_usuario)
        {
 

            var usuario = await _context.usuarios
                .FirstOrDefaultAsync(m => m._id_usuario == id_usuario);
            if (usuario == null)
            {
                return NotFound();
            }
            ViewBag.id = id;
            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id,int _id_usuario)
        {
            if (_context.usuarios == null)
            {
                return Problem("Entity set 'MyContext.usuarios'  is null.");
            }
            var usuario = await _context.usuarios.FindAsync(_id_usuario);
            if (usuario != null)
            {
                _context.usuarios.Remove(usuario);
            }

            await _context.SaveChangesAsync();

            ViewBag.id = id;
            return RedirectToAction("Index", new { id = id });
        }

        private bool UsuarioExists(int id)
        {
          return _context.usuarios.Any(e => e._id_usuario == id);
        }
    }
}
