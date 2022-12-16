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
    public class PagosController : Controller
    {
        private readonly MyContext _context;

        public PagosController(MyContext context)
        {
            _context = context;
        }

        // GET: Pagos
        public async Task<IActionResult> Index(int id)
        {
            var myContext = _context.pagos.Include(p => p._usuario);
            _context.usuarios.Include(u => u._pagos).Load();
            Usuario usuario = _context.usuarios.Where(u => u._id_usuario == id).FirstOrDefault();



            if (usuario._esUsuarioAdmin)
            {
                ViewBag.id = id;
                return View(await myContext.ToListAsync());
            }
            else
            {
                ViewBag.id = id;
                return View(usuario._pagos.ToList());
            }


        }



        // GET: Pagos/Create
        public IActionResult Create(int id)
        {
            _context.usuarios.Include(u => u.cajas).Load();
            _context.usuarios.Include(u => u._tarjetas).Load();
            Usuario usuario = _context.usuarios.Where(u => u._id_usuario == id).FirstOrDefault();


            if (usuario != null)
            {
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
            }
            else { return Problem("Hubo un error, contacte con el administrador"); }

            ViewBag.id = id;
            return View();

        }

        // POST: Pagos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int id, int _id_usuario, double _monto, string _detalle, bool _pagado = false, string _metodo = "Pendiente Pago", int _id_metodo = -1)
        {


            Pago pago = new Pago(_id_usuario, _monto, _pagado, _metodo, _detalle, _id_metodo);

            if (ModelState.IsValid)
            {
                _context.Add(pago);
                await _context.SaveChangesAsync();

            }
            return RedirectToAction("Index", new { id = id });
        }



        // GET: Pagos/Delete/5
        public async Task<IActionResult> Delete(int? id,int id_pago)
        {
            if (id == null || _context.pagos == null)
            {
                return NotFound();
            }

            var pago = await _context.pagos
                .Include(p => p._usuario)
                .FirstOrDefaultAsync(m => m._id_pago == id_pago);
            if (pago == null)
            {
                return NotFound();
            }

            ViewBag.id = id;
            ViewData["mensaje"] = "";
            return View(pago);
        }

        // POST: Pagos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int _id_usuario, int _id_pago)
        {
            if (_context.pagos == null)
            {
                return Problem("Entity set 'MyContext.pagos'  is null.");
            }
            var pago = await _context.pagos.FindAsync(_id_pago);
            if (pago != null)
            {
                if (pago._pagado == true)
                {
                    _context.pagos.Remove(pago);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    ViewBag.id = _id_usuario;
                    ViewData["mensaje"] = "El pago  debe estar pagado para poder eliminarlo";
                    return View(pago);
                }
            }


            return RedirectToAction("Index", new { id = _id_usuario });
        }

        private bool PagoExists(int id)
        {
            return _context.pagos.Any(e => e._id_pago == id);
        }

        // GET: Pagar Parametro desde el index asp-route-id_tarjeta="@item._id_pago
        public IActionResult Pagar(int id, int id_pago)
        {
            _context.usuarios.Include(u => u.cajas).Load();
            _context.usuarios.Include(u => u._tarjetas).Load();
            Pago pago = _context.pagos.Where(p => p._id_pago == id_pago).FirstOrDefault();
            Usuario usuario = _context.usuarios.Where(u => u._id_usuario == pago._id_usuario).FirstOrDefault();


            if (usuario == null || pago == null) return NotFound();

            ViewData["cajas"] = new SelectList(usuario.cajas.ToList(), "_cbu", "_cbu");
            ViewData["tarjetas"] = new SelectList(usuario._tarjetas.ToList(), "_numero", "_numero");
            ViewBag.id = id;
            ViewData["mensaje"] = "";
            return View(pago);

        }

        // POST: Pagos/Pagar
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pagar(int id, int _id_pago, string? numeroTarjeta, string? cbu)
        {
            Pago pago = _context.pagos.Where(p => p._id_pago == _id_pago).FirstOrDefault();

            if (pago == null) return Problem("No se encontro el pago, consulte con un administrador");

            if (pago._pagado == true)
            {
                ViewBag.id = id;
                ViewData["mensaje"] = "El servicio ya esta pagado";
                return View(pago);
            }

            if (cbu == null && numeroTarjeta == null)
            {
                ViewBag.id = id;
                ViewData["mensaje"] = "Se requiere un medio de pago";
                return View(pago);
            }
            else if (cbu != null && numeroTarjeta != null)
            {
                ViewBag.id = id;
                ViewData["mensaje"] = "Se requiere SOLO un medio de pago";
                return View(pago);

            }



            if (numeroTarjeta != null)//Opcion Pago con tarjeta
            {
                TarjetaDeCredito tarjeta = _context.tarjetas.Where(t => t._numero == numeroTarjeta).FirstOrDefault();
                if (tarjeta == null) return Problem("No se encontro la tarjeta, consulte ocn un administrador");

                if (tarjeta._consumos <= tarjeta._limite)
                {
                    tarjeta._consumos += pago._monto;
                    tarjeta._limite -= pago._monto;
                    pago._id_metodo = tarjeta._id_tarjeta;
                    pago._metodo = tarjeta._numero;
                    pago._pagado = true;

                    if (ModelState.IsValid)
                    {

                        _context.Update(tarjeta);
                        _context.Update(pago);
                        await _context.SaveChangesAsync();
                        return RedirectToAction("Index", new { id = id });


                    }

                }
                else
                {
                    ViewBag.id = id;
                    ViewData["mensaje"] = "Limite de tarjeta insuficiente";
                    return View(pago);

                }


            }
            else//Opcion caja de ahorro:
            {
                CajaDeAhorro caja = _context.cajas.Where(c => c._cbu == cbu).FirstOrDefault();
                if (caja._saldo >= pago._monto)
                {
                    caja._saldo -= pago._monto;
                    pago._id_metodo = caja._id_caja;
                    pago._metodo = caja._cbu;
                    pago._pagado = true;

                    Movimiento movimiento = new Movimiento(caja._id_caja, "Pago Servicios", pago._monto, DateTime.Now);
                    caja._movimientos.Add(movimiento);



                    if (ModelState.IsValid)
                    {

                        _context.Update(caja);
                        _context.Update(pago);
                        _context.movimientos.Add(movimiento);
                        await _context.SaveChangesAsync();
                        return RedirectToAction("Index", new { id = id });
                    }

                }
                else
                {
                    ViewBag.id = id;
                    ViewData["mensaje"] = "Saldo de la cuenta insuficiente";
                    return View(pago);
                }


            }
            return RedirectToAction("Index", new { id = id });
        }

    }
}
