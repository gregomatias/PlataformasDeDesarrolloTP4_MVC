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
    public class TarjetaDeCreditosController : Controller
    {
        private readonly MyContext _context;

        public TarjetaDeCreditosController(MyContext context)
        {
            _context = context;
        }

        // GET: TarjetaDeCreditos
        public async Task<IActionResult> Index(int id)
        {
            _context.usuarios.Include(u => u._tarjetas).Load();
            Usuario usuario = _context.usuarios.Where(u => u._id_usuario == id).FirstOrDefault();



            if (usuario != null)
            {
                if (usuario._esUsuarioAdmin)
                {
                    ViewBag.id = id;
                    return View(await _context.tarjetas.ToListAsync());
                }
                else
                {
                    ViewBag.id = id;
                    return View(usuario._tarjetas.ToList());
                }
            }

            return NotFound();
        }

        // GET: TarjetaDeCreditos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.tarjetas == null)
            {
                return NotFound();
            }

            var tarjetaDeCredito = await _context.tarjetas
                .Include(t => t._titular)
                .FirstOrDefaultAsync(m => m._id_tarjeta == id);
            if (tarjetaDeCredito == null)
            {
                return NotFound();
            }

            return View(tarjetaDeCredito);
        }

        // GET: TarjetaDeCreditos/Create
        public IActionResult Create(int id)
        {
            Usuario usuario = _context.usuarios.Where(u => u._id_usuario == id).FirstOrDefault();
            if (usuario == null) return NotFound();

            if (usuario._esUsuarioAdmin)
            {
                ViewData["_id_usuario"] = new SelectList(_context.usuarios, "_id_usuario", "_id_usuario");
            }
            else
            {
                ICollection<Usuario> usuarios = new List<Usuario>();
                usuarios.Add(usuario);
                ViewData["_id_usuario"] = new SelectList(usuarios, "_id_usuario", "_id_usuario");
            }

            //Genera secuencia unica de CBU o Tarjeta
            DateTimeOffset now = (DateTimeOffset)DateTime.UtcNow;
            string nuevo_numero = now.ToString("yyyyMMddHHmmssfff");
            nuevo_numero = "T" + nuevo_numero + id;

            ViewBag.id = id;
            ViewBag.nuevo_numero = nuevo_numero;
            ViewBag.codigoV = 0;
            ViewBag.limite = 500000;
            ViewBag.consumos = 0;
            return View();

        }

        // POST: TarjetaDeCreditos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int id, [Bind("_id_tarjeta,_id_usuario,_numero,_codigoV,_limite,_consumos")] TarjetaDeCredito tarjetaDeCredito)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tarjetaDeCredito);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { id = id });
            }

            return RedirectToAction("Index", new { id = id });
        }

        // GET: TarjetaDeCreditos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.tarjetas == null)
            {
                return NotFound();
            }

            var tarjetaDeCredito = await _context.tarjetas.FindAsync(id);
            if (tarjetaDeCredito == null)
            {
                return NotFound();
            }
            ViewData["_id_usuario"] = new SelectList(_context.usuarios, "_id_usuario", "_id_usuario", tarjetaDeCredito._id_usuario);
            return View(tarjetaDeCredito);
        }

        // POST: TarjetaDeCreditos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("_id_tarjeta,_id_usuario,_numero,_codigoV,_limite,_consumos")] TarjetaDeCredito tarjetaDeCredito)
        {
            if (id != tarjetaDeCredito._id_tarjeta)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tarjetaDeCredito);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TarjetaDeCreditoExists(tarjetaDeCredito._id_tarjeta))
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
            ViewData["_id_usuario"] = new SelectList(_context.usuarios, "_id_usuario", "_id_usuario", tarjetaDeCredito._id_usuario);
            return View(tarjetaDeCredito);
        }

        // GET: TarjetaDeCreditos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.tarjetas == null)
            {
                return NotFound();
            }

            var tarjetaDeCredito = await _context.tarjetas
                .Include(t => t._titular)
                .FirstOrDefaultAsync(m => m._id_tarjeta == id);
            if (tarjetaDeCredito == null)
            {
                return NotFound();
            }

            ViewBag.id = id;
            ViewData["mensaje"] = "";
            return View(tarjetaDeCredito);
        }

        // POST: TarjetaDeCreditos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int _id_usuario, int _id_tarjeta)
        {


            if (_context.tarjetas == null)
            {
                return Problem("Entity set 'MyContext.tarjetas'  is null.");
            }
            var tarjetaDeCredito = await _context.tarjetas.FindAsync(_id_tarjeta);
           

            if (tarjetaDeCredito != null)
            {
                if (tarjetaDeCredito._consumos == 0){

                    _context.tarjetas.Remove(tarjetaDeCredito);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    ViewBag.id = _id_usuario;
                    ViewData["mensaje"] = "La tarjeta debe estar en cero para poder cancelarla";
                    return View(tarjetaDeCredito);
                }
              
            }


            //return RedirectToAction(nameof(Index));
            return RedirectToAction("Index", new { id = _id_usuario });
        }

        private bool TarjetaDeCreditoExists(int id)
        {
            return _context.tarjetas.Any(e => e._id_tarjeta == id);
        }

        // GET: Pagar Parametro desde el index asp-route-id_tarjeta="@item._id_tarjeta
        public IActionResult Pagar(int id, int id_tarjeta)
        {
            _context.usuarios.Include(u => u.cajas).Load();
            Usuario usuario = _context.usuarios.Where(u => u._id_usuario == id).FirstOrDefault();
            TarjetaDeCredito tarjeta = _context.tarjetas.Where(t => t._id_tarjeta == id_tarjeta).FirstOrDefault();

            if (usuario == null || tarjeta == null) return NotFound();

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
            return View(tarjeta);

        }

        // POST: TarjetaDeCreditos/Pagar
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pagar(int id, int _id_tarjeta, double _consumos, string _numero, string cbu)
        {

            TarjetaDeCredito tarjeta = _context.tarjetas.Where(t => t._id_tarjeta == _id_tarjeta).FirstOrDefault();
            CajaDeAhorro caja = _context.cajas.Where(c => c._cbu == cbu).FirstOrDefault();

            if (caja == null || tarjeta == null) return NotFound();



            if (caja._saldo >= tarjeta._consumos)
            {
                caja._saldo -= tarjeta._consumos;
                tarjeta._consumos = 0;
                Movimiento movimiento = new Movimiento(caja._id_caja, "Pago Tarjeta", _consumos, DateTime.Now);
                caja._movimientos.Add(movimiento);

                if (ModelState.IsValid)
                {

                    _context.Update(caja);
                    _context.Update(tarjeta);
                    _context.movimientos.Add(movimiento);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", new { id = id });
                }

            }
            else
            {
                ViewBag.id = id;
                ViewData["mensaje"] = "Saldo de la cuenta insuficiente";
                return View(tarjeta);
            }




            return RedirectToAction("Index", new { id = id });
        }


    }
}
