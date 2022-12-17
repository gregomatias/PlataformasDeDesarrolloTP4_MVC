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
                    ViewBag.id = id;
                    return View(await _context.cajas.ToListAsync());
                }
                else
                {
                    ViewBag.id = id;
                    return View(usuario.cajas.ToList());
                }
            }

            return NotFound();

        }

        // GET: CajaDeAhorros/Details/5
        public async Task<IActionResult> Details(int? id, int _id_caja)
        {


            var cajaDeAhorro = await _context.cajas
                .FirstOrDefaultAsync(m => m._id_caja == _id_caja);
            if (cajaDeAhorro == null)
            {
                return NotFound();
            }
            ViewBag.id = id;
            return View(cajaDeAhorro);
        }

        // GET: CajaDeAhorros/Create
        public IActionResult Create(int id)
        {
            //Genera secuencia unica de CBU o Tarjeta
            DateTimeOffset now = (DateTimeOffset)DateTime.UtcNow;
            string nuevo_cbu = now.ToString("yyyyMMddHHmmssfff");
            nuevo_cbu = nuevo_cbu + id;

            ViewBag.id = id;
            ViewBag.nuevo_cbu = nuevo_cbu;
            return View();
        }

        // POST: CajaDeAhorros/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int? id, [Bind("_id_caja,_cbu,_saldo=0")] CajaDeAhorro cajaDeAhorro)
        {


            Usuario usuario = _context.usuarios.Where(u => u._id_usuario == id).FirstOrDefault();
            usuario.cajas.Add(cajaDeAhorro);
            _context.Update(usuario);

            if (ModelState.IsValid)
            {
                _context.Add(cajaDeAhorro);

                await _context.SaveChangesAsync();
             

                return RedirectToAction("Index", new { id = id });
            }
            ViewBag.id = id;
            return View(cajaDeAhorro);
        }

        // GET: CajaDeAhorros/Edit/5
        public async Task<IActionResult> Edit(int? id, int _id_caja )
        {


            var cajaDeAhorro = await _context.cajas.FindAsync(_id_caja);
            if (cajaDeAhorro == null)
            {
                return NotFound();
            }
            ViewBag.id = id;
            return View(cajaDeAhorro);
        }

        // POST: CajaDeAhorros/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("_id_caja,_cbu,_saldo")] CajaDeAhorro cajaDeAhorro)
        {
  

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
                ViewBag.id = id;

                return RedirectToAction("Index", new { id = id });
            }
            ViewBag.id = id;
            return View(cajaDeAhorro);
        }

        // GET: CajaDeAhorros/Delete/5
        public async Task<IActionResult> Delete(int? id,int _id_caja)
        {


            var cajaDeAhorro = await _context.cajas
                .FirstOrDefaultAsync(m => m._id_caja == _id_caja);
            if (cajaDeAhorro == null)
            {
                return NotFound();
            }

            ViewBag.id = id;
            return View(cajaDeAhorro);
        }

        // POST: CajaDeAhorros/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id,int _id_caja)
        {
            if (_context.cajas == null)
            {
                return Problem("Entity set 'MyContext.cajas'  is null.");
            }
            var cajaDeAhorro = await _context.cajas.FindAsync(_id_caja);
            if (cajaDeAhorro != null)
            {
                _context.cajas.Remove(cajaDeAhorro);
            }

            await _context.SaveChangesAsync();
            ViewBag.id = id;

            return RedirectToAction("Index", new { id = id });
        }

        private bool CajaDeAhorroExists(int id)
        {
            return _context.cajas.Any(e => e._id_caja == id);
        }

        public async Task<IActionResult> Depositar(int? id, int? id_caja)
        {
            if (id_caja == null || _context.cajas == null)
            {
                return NotFound();
            }

            var cajaDeAhorro = await _context.cajas.FindAsync(id_caja);
            if (cajaDeAhorro == null)
            {
                return NotFound();
            }
            //Id de usuario
            ViewBag.id = id;
            return View(cajaDeAhorro);
        }

        //Post Depositar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Depositar(int id, int _id_caja, double _saldo)

        {
            CajaDeAhorro cajaDeAhorro = _context.cajas.Where(c => c._id_caja == _id_caja).FirstOrDefault();

            cajaDeAhorro._saldo = cajaDeAhorro._saldo + Math.Abs(_saldo);

            Movimiento movimiento = new Movimiento(_id_caja, "Deposito", _saldo, DateTime.Now);
            cajaDeAhorro._movimientos.Add(movimiento);
            _context.movimientos.Add(movimiento);

            if (cajaDeAhorro == null)
            {
                return Problem("Ocurrio un problema, consulte con el administrador");
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
                return RedirectToAction("Index", new { id = id });
            }

            return RedirectToAction("Index", new { id = id });
        }

        public async Task<IActionResult> Retirar(int? id, int? id_caja)
        {
            if (id_caja == null || _context.cajas == null)
            {
                return NotFound();
            }

            var cajaDeAhorro = await _context.cajas.FindAsync(id_caja);
            if (cajaDeAhorro == null)
            {
                return NotFound();
            }
            //Id de usuario
            ViewBag.id = id;
            return View(cajaDeAhorro);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Retirar(int id, int _id_caja, double _saldo)

        {


            CajaDeAhorro cajaDeAhorro = _context.cajas.Where(c => c._id_caja == _id_caja).FirstOrDefault();
            cajaDeAhorro._saldo = cajaDeAhorro._saldo - Math.Abs(_saldo);
            Movimiento movimiento = new Movimiento(_id_caja, "Retiro", _saldo, DateTime.Now);
            cajaDeAhorro._movimientos.Add(movimiento);
            _context.movimientos.Add(movimiento);

            if (cajaDeAhorro == null)
            {

                return Problem("Ocurrio un problema, consulte con el administrador");
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
                return RedirectToAction("Index", new { id = id });
            }

            return RedirectToAction("Index", new { id = id });
        }


        public async Task<IActionResult> Transferir(int? id, int? id_caja)
        {
            if (id_caja == null || _context.cajas == null)
            {
                return NotFound();
            }

            var cajaDeAhorro = await _context.cajas.FindAsync(id_caja);
            if (cajaDeAhorro == null)
            {
                return NotFound();
            }

            ViewData["_cbu"] = new SelectList(_context.cajas.ToList(), "_cbu", "_cbu");
            ViewBag.id = id;
            ViewData["mensaje"] = "";
            return View(cajaDeAhorro);
        }

        //Post Depositar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Transferir(int id, int _id_caja, double montoTransferencia, string cbuDestino)

        {
            CajaDeAhorro cajaDeAhorroOrigen = _context.cajas.Where(c => c._id_caja == _id_caja).FirstOrDefault();
            CajaDeAhorro cajaDeAhorroDestino = _context.cajas.Where(c => c._cbu == cbuDestino).FirstOrDefault();





            if (cajaDeAhorroOrigen == null || cajaDeAhorroDestino == null)
            {
                return Problem("Ocurrio un problema, consulte con el administrador");
            }



            if (ModelState.IsValid)
            {
                try
                {
                    if (cajaDeAhorroOrigen._saldo >= montoTransferencia)
                    {

                        cajaDeAhorroOrigen._saldo -= montoTransferencia;
                        cajaDeAhorroDestino._saldo += montoTransferencia;

                        Movimiento movimientoOrigen = new Movimiento(_id_caja, "Retirpo por Transferencia", montoTransferencia, DateTime.Now);
                        Movimiento movimientoDestino = new Movimiento(_id_caja, "Acreditación por Transferencia", montoTransferencia, DateTime.Now);
                        cajaDeAhorroOrigen._movimientos.Add(movimientoOrigen);
                        cajaDeAhorroDestino._movimientos.Add(movimientoDestino);
                        _context.movimientos.Add(movimientoOrigen);
                        _context.movimientos.Add(movimientoDestino);
                        _context.Update(cajaDeAhorroOrigen);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        ViewData["mensaje"] = "Saldo insuficiente";
                        ViewData["_cbu"] = new SelectList(_context.cajas.ToList(), "_cbu", "_cbu");
                        ViewBag.id = id;
                        return View(cajaDeAhorroOrigen);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CajaDeAhorroExists(cajaDeAhorroOrigen._id_caja))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", new { id = id });
            }

            return RedirectToAction("Index", new { id = id });
        }


    }//Fin Clase


}
