﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP4.Models;

public class Usuario
{
    private int id;
    private int dni;
    private string nombre;
    private string apellido;
    private string mail;
    private string password;
    private int intentosFallidos;
    private bool/*int*/ esUsuarioAdmin;
    private bool bloqueado;

    public ICollection<CajaDeAhorro> cajas { get; } = new List<CajaDeAhorro>();
    public List<UsuarioCajaDeAhorro?> usuarioCajas { get; set; }

    
    public List<PlazoFijo> _plazosFijos = new List<PlazoFijo>();
    public List<TarjetaDeCredito> _tarjetas = new List<TarjetaDeCredito>();
    public List<Pago> _pagos = new List<Pago>();




    public Usuario() { }


    public Usuario( int dni, string nombre, string apellido, string mail, string password,  int intentosFallidos, bool esUsuarioAdmin, bool bloqueado)
    {

        _id_usuario = id;
        _dni = dni;
        _nombre = nombre;
        _apellido = apellido;
        _mail = mail;
        _password = password;
        _esUsuarioAdmin = esUsuarioAdmin;
        _intentosFallidos = intentosFallidos;
        _bloqueado = bloqueado;


    }

    public int _id_usuario
    {
        get { return id; }
        set { id = value; }
    }

    public int _dni
    {
        get { return dni; }
        set { dni = value; }
    }

    public string _nombre
    {
        get { return nombre; }
        set { nombre = value; }
    }

    public string _apellido
    {
        get { return apellido; }
        set { apellido = value; }
    }

    public string _mail
    {
        get { return mail; }
        set { mail = value; }
    }

    public string _password
    {
        get { return password; }
        set { password = value; }
    }

    public int _intentosFallidos
    {
        get { return intentosFallidos; }
        set { intentosFallidos = value; }
    }

    public bool _esUsuarioAdmin
    {
        get { return esUsuarioAdmin; }
        set { esUsuarioAdmin = value; }
    }

    public bool _bloqueado
    {
        get { return bloqueado; }
        set { bloqueado = value; }
    }

    public override string ToString()
    {
        return "Id: " + _id_usuario + " Intentos: " + _intentosFallidos + " EsAdmin: " + _esUsuarioAdmin +
            " QCajas:" + cajas.Count() + " QTarjetas:" + _tarjetas.Count() + " QPlazoFijos:" + _plazosFijos.Count() +
            " QPagos:" + _pagos.Count();
    }



}
