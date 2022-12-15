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
    public class PlazoFijosController : Controller
    {
        private readonly MyContext _context;

        public PlazoFijosController(MyContext context)
        {
            _context = context;
        }

        // GET: PlazoFijos
        public async Task<IActionResult> Index(int id)
        {
            var myContext = _context.plazosFijos.Include(p => p._titular);
            _context.usuarios.Include(u => u._plazosFijos).Load();
            Usuario usuario = _context.usuarios.Where(u => u._id_usuario == id).FirstOrDefault();



            if (usuario._esUsuarioAdmin)
            {
                ViewBag.id = id;
                return View(await myContext.ToListAsync());
            }
            else
            {
                ViewBag.id = id;
                return View(usuario._plazosFijos.ToList());
            }



        }


        // GET: PlazoFijos/Create
        public IActionResult Create(int id)

        {
            _context.usuarios.Include(u => u.cajas).Load();
            Usuario usuario = _context.usuarios.Where(u => u._id_usuario == id).FirstOrDefault();

            ViewData["_cbu"] = new SelectList(usuario.cajas.ToList(), "_cbu", "_cbu");



            ViewData["_id_usuario"] = new SelectList(_context.usuarios, "_id_usuario", "_id_usuario");
            ViewBag.id = id;
            ViewData["mensaje"] = "";
            return View();
        }

        // POST: PlazoFijos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int id, double _monto, DateTime _fechaIni, DateTime _fechaFin, string cbu)
        {

            PlazoFijo plazoFijo = new PlazoFijo(id, _monto, _fechaIni, _fechaFin, 1.5, false);
            CajaDeAhorro caja = _context.cajas.Where(c => c._cbu == cbu).FirstOrDefault();

            if (plazoFijo == null || caja == null) return NotFound();


            if (caja._saldo >= _monto)
            {

                if (ModelState.IsValid)
                {
                    caja._saldo -= _monto;
                    Movimiento movimiento = new Movimiento(caja._id_caja, "Crea Plazo Fijo", _monto, DateTime.Now);
                    caja._movimientos.Add(movimiento);
                    _context.Update(caja);
                    _context.Add(plazoFijo);
                    _context.movimientos.Add(movimiento);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", new { id = id });
                }

            }
            else
            {
                ViewBag.id = id;
                ViewData["mensaje"] = "Saldo de la cuenta insuficiente";
                return View(plazoFijo);
            }


            ViewData["_id_usuario"] = new SelectList(_context.usuarios, "_id_usuario", "_id_usuario", plazoFijo._id_usuario);
            ViewBag.id = id;
            return View(plazoFijo);
        }





        // GET: PlazoFijos/Delete/5
        public async Task<IActionResult> Delete(int? id, int id_plazoFijo)
        {
            if (id == null || _context.plazosFijos == null)
            {
                return NotFound();
            }

            var plazoFijo = await _context.plazosFijos
                .Include(p => p._titular)
                .FirstOrDefaultAsync(m => m._id_plazoFijo == id_plazoFijo);
            if (plazoFijo == null)
            {
                return NotFound();
            }
            ViewBag.id = id;
            return View(plazoFijo);
        }

        // POST: PlazoFijos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int _id_plazoFijo)
        {


            if (_context.plazosFijos == null)
            {
                return Problem("Entity set 'MyContext.plazosFijos'  is null.");
            }
            var plazoFijo = await _context.plazosFijos.FindAsync(_id_plazoFijo);
            if (plazoFijo != null)
            {
                if (plazoFijo._pagado == true && plazoFijo._fechaFin < DateTime.Now)
                {

                    _context.plazosFijos.Remove(plazoFijo);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    ViewBag.id = id;
                    ViewData["mensaje"] = "El plazo fijo debe esta pagado, y finalizado";
                    return View(plazoFijo);

                }

            }



            return RedirectToAction("Index", new { id = id });
        }

        private bool PlazoFijoExists(int id)
        {
            return _context.plazosFijos.Any(e => e._id_plazoFijo == id);
        }


        // GET: Pagar Parametro desde el index asp-route-id_tarjeta="@item._id_tarjeta
        public async Task<IActionResult> Pagar(int id, int id_plazoFijo)
        {
            _context.usuarios.Include(u => u.cajas).Load();

            var plazoFijo = await _context.plazosFijos.FindAsync(id_plazoFijo);
            Usuario usuario = _context.usuarios.Where(u => u._id_usuario == plazoFijo._id_usuario).FirstOrDefault();

            if (usuario == null || plazoFijo == null) return NotFound();

            if (usuario._esUsuarioAdmin)
            {
                ViewData["_cbu"] = new SelectList(_context.cajas, "_cbu", "_cbu");
            }
            else
            {

                ViewData["_cbu"] = new SelectList(usuario.cajas.ToList(), "_cbu", "_cbu");
            }



            ViewBag.id = id;
            ViewData["mensaje"] = "";
            return View(plazoFijo);

        }

        // POST: PlazoFijo/Pagar
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pagar(int id, int _id_plazoFijo, string cbu)
        {

            var plazoFijo = await _context.plazosFijos.FindAsync(_id_plazoFijo);
            CajaDeAhorro caja = _context.cajas.Where(c => c._cbu == cbu).FirstOrDefault();

            if (caja == null || plazoFijo == null) return NotFound();


            System.TimeSpan diffResult = DateTime.Now.Subtract(plazoFijo._fechaIni);

            if (plazoFijo._pagado == true)
            {
                ViewBag.id = id;
                ViewData["mensaje"] = "El plazo fijo ya fue pagado";
                return View(plazoFijo);

            }
            if (diffResult.Days >= 30)
            {
                double monto = plazoFijo._monto + (plazoFijo._monto * (plazoFijo._tasa/365) * diffResult.Days); 
                caja._saldo += monto;
                plazoFijo._pagado = true;
                Movimiento movimiento = new Movimiento(caja._id_caja, "Acreditación PLazo Fijo", monto, DateTime.Now);
                caja._movimientos.Add(movimiento);

                if (ModelState.IsValid)
                {

                    _context.Update(caja);
                    _context.Update(plazoFijo);
                    _context.movimientos.Add(movimiento);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index", new { id = id });
                }

            }
            else
            {
                ViewBag.id = id;
                ViewData["mensaje"] = "Tiempo de acreditación minima 30 días";
                return View(plazoFijo);
            }




            return RedirectToAction("Index", new { id = id });
        }


    }
}
