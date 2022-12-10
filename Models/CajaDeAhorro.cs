using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TP4.Models;

public  class CajaDeAhorro
{

    private List<Movimiento> movimientos { get; set; } = new List<Movimiento>();
    
    public ICollection<Usuario> titulares { get; } = new List<Usuario>();

    public List<UsuarioCajaDeAhorro> usuarioCajas { get; set; }

    public CajaDeAhorro() { }

    public CajaDeAhorro(string cbu, double saldo)
    {
        _cbu = cbu;
        //titulares.Add(usuario);
        _saldo = saldo;

    }

    public CajaDeAhorro(int id,string cbu, int usuario, double saldo)
    {
        _id_caja = id;
        _cbu = cbu;
        //_titulares.Add(usuario);
			_saldo = saldo;

    }

    public CajaDeAhorro(int id, string cbu, double saldo)
    {
        _id_caja = id;
        _cbu = cbu;
        _saldo = saldo;

    }



    private int id = 0;

		public int _id_caja
		{
			get { return id; }
			set { id = value; }
		}

		private string cbu;

		public string _cbu
		{
			get { return cbu; }
			set { cbu = value; }
		}

    
		public List<Movimiento> _movimientos
		{
			get { return movimientos; }
			set { movimientos =value; }
		}
		
   

		private double saldo;


		public double _saldo
		{
			get { return saldo; }
			set { saldo = value; }
		}

    public override string ToString()
    {
        return "Id: " + _id_caja + " CBU: " + _cbu + " Saldo: " + _saldo+ " QMovimientos:" + _movimientos.Count();
    }



}
