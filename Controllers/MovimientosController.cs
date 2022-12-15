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
    public class MovimientosController : Controller
    {
        private readonly MyContext _context;

        public MovimientosController(MyContext context)
        {
            _context = context;
        }

        // GET: Movimientos
        public async Task<IActionResult> Index(int id, string? filtroDetalle, double? filtroMonto, DateTime? filtroFecha)
        {


            var myContext = _context.movimientos.Include(m => m._cajaDeAhorro);
            _context.usuarios.Include(u => u.cajas).Load();
            Usuario usuario = _context.usuarios.Where(u => u._id_usuario == id).FirstOrDefault();




            if (usuario._esUsuarioAdmin)
            {

                if (filtroDetalle != null || filtroMonto != null || filtroFecha != null)
                {
                    var modelo = _context.movimientos.ToList();

                    if ( filtroDetalle != null)
                    {
                        modelo = modelo.Where(m => m._detalle.Contains(filtroDetalle)).ToList();

                    }
                    if (filtroMonto != null)
                    {
                        modelo = modelo.Where(m => m._monto== filtroMonto).ToList();
                    }
                    if (filtroFecha != null)
                    {
                        modelo = modelo.Where(m => m._monto == filtroMonto).ToList();
                    }

 
                    ViewBag.Id = id;
                    return View( modelo.ToList());

                }
                else
                {
                    ViewBag.Id = id;
                    return View(await myContext.ToListAsync());
                }

            }
            else
            {
                List<Movimiento> movimientosDeTodasLasCajasDelUsuario = new List<Movimiento>();
                foreach (CajaDeAhorro caja in usuario.cajas)
                {
                    var movimietosDeUnaCajaDelUsuario = myContext.Where(m => m._id_CajaDeAhorro == caja._id_caja).ToList();
                    foreach (Movimiento movimiento in movimietosDeUnaCajaDelUsuario)
                        movimientosDeTodasLasCajasDelUsuario.Add(movimiento);
                }


                if (filtroDetalle != null || filtroMonto != null || filtroFecha != null)
                {
                    var modelo = movimientosDeTodasLasCajasDelUsuario.ToList();

                    if ( filtroDetalle != null)
                    {
                        modelo = modelo.Where(m => m._detalle.Contains(filtroDetalle)).ToList();

                    }
                    if (filtroMonto != null)
                    {
                        modelo = modelo.Where(m => m._monto== filtroMonto).ToList();
                    }
                    if (filtroFecha != null)
                    {
                        modelo = modelo.Where(m => m._monto == filtroMonto).ToList();
                    }

 
                    ViewBag.Id = id;
                    return View( modelo.ToList());

                }
                else
                {
                    ViewBag.Id = id;
                    return View(movimientosDeTodasLasCajasDelUsuario.ToList());
                }





            }

        }



        // GET: Movimientos/Create
        public IActionResult Create()
        {
            ViewData["_id_CajaDeAhorro"] = new SelectList(_context.cajas, "_id_caja", "_cbu");
            return View();
        }

        // POST: Movimientos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("_id_CajaDeAhorro,_id_Movimiento,_detalle,_monto,_fecha")] Movimiento movimiento)
        {
            if (ModelState.IsValid)
            {
                _context.Add(movimiento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["_id_CajaDeAhorro"] = new SelectList(_context.cajas, "_id_caja", "_cbu", movimiento._id_CajaDeAhorro);
            return View(movimiento);
        }


        private bool MovimientoExists(int id)
        {
            return _context.movimientos.Any(e => e._id_Movimiento == id);
        }


    }
}
