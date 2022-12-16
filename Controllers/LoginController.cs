﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Policy;
using TP4.Models;

//1.- REFERENCES AUTHENTICATION COOKIE
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace TP4.Controllers
{
    public class LoginController : Controller
    {
        private readonly MyContext _context;

        public LoginController(MyContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index(string? mensaje)
        {
            if (mensaje != null)
            {
                ViewData["mensaje"] = mensaje;
                return View();
            }
            else
            {
                return View();
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logueo(int _dni, string _password)
        {

            try
            {
                //&& !u._bloqueado && u._password == _password
             //   var usuario = await _context.usuarios.FirstOrDefaultAsync(u => u._dni == _dni);
                var usuario = await _context.usuarios.FirstOrDefaultAsync(m => m._dni == _dni);


                if (usuario != null)
                {

                    if (usuario._password == _password && usuario._bloqueado == false)
                    {



                        //2.- CONFIGURACION DE LA AUTENTICACION
                        #region AUTENTICACTION
                        var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuario._apellido),
                    new Claim("Correo", usuario._mail),
                    new Claim("EsAdmin", usuario._esUsuarioAdmin.ToString()),
                     new Claim(ClaimTypes.Role, usuario._esUsuarioAdmin.ToString()),

                };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                        #endregion



                        return RedirectToAction("Index", "Home", new { IdUsuarioLogueado = usuario._id_usuario });
                    }
                    else
                    {
                        if (!usuario._bloqueado)
                        {
                            if (usuario._intentosFallidos >= 2)
                            {
                                usuario._bloqueado = true;
                                usuario._intentosFallidos += 1;

                            }
                            else { usuario._intentosFallidos += 1; }

                            _context.Update(usuario);
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            return RedirectToAction("Index", "Login", new { mensaje = "Usuario Bloquedo" });
                        }

                        

                    }
                }
                else
                {
                    return RedirectToAction("Index", "Login", new { mensaje = "Usuario y/o contraseña invalidos" });
                }


            }
            catch (Exception ex) { }

            return RedirectToAction("Index", "Login", new { mensaje = "Intentos fallidos"  });
        }

        public async Task<IActionResult> Salir()
        {
            //3.- CONFIGURACION DE LA AUTENTICACION
            #region AUTENTICACTION
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            #endregion

            return RedirectToAction("Index");

        }

    }
}
