using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP4.Models;

    public class PlazoFijo
    {
	public PlazoFijo() { }

	
        public PlazoFijo( int id_usuario, double monto, DateTime fechaIni, DateTime fechaFin, double tasa, bool pagado)
        {
            
            _id_usuario = id_usuario;
            _monto = monto;
		_fechaIni = fechaIni;
            _fechaFin = fechaFin;
            _tasa = tasa;
            _pagado = pagado;
        }
        private int id;

	public int _id_plazoFijo
	{
		get { return id; }
		set { id = value; }
	}

	private int id_usuario;

	public int _id_usuario
	{
		get { return id_usuario; }
		set { id_usuario = value; }
	} 


	private Usuario titular;

	public Usuario _titular
	{
		get { return titular; }
		set { titular = value; }
	}

	private double monto;

    [Range(0, double.MaxValue, ErrorMessage = "Ingrese un valor mayor a 0")]
    public double _monto
	{
		get { return monto; }
		set { monto = value; }
	}

	private DateTime fechaIni;

	public DateTime _fechaIni
	{
		get { return fechaIni; }
		set { fechaIni = value; }
	}

	private DateTime fechaFin;

	public DateTime _fechaFin
	{
		get { return fechaFin; }
		set { fechaFin = value; }
	}

	private double tasa;

	public double _tasa
	{
		get { return tasa; }
		set { tasa = value; }
	}

	private bool pagado;

	public bool _pagado
	{
		get { return pagado; }
		set { pagado = value; }
	}





}
