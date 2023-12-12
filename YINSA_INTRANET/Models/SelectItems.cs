using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace YINSA_INTRANET.Models
{
	public enum EstadosDoc
	{
		Pendiente = 0,
		Pagado = 1,
		Cancelado = 2
	}

	public enum Meses
	{
		Enero = 1,
		Febrero = 2,
		Marzo = 3,
		Abril = 4,
		Mayo = 5,
		Junio = 6,
		Julio = 7,
		Agosto = 8,
		Septiembre = 9,
		Octubre = 10,
		Noviembre = 11,
		Diciembre = 12
	}

	public enum TipoReporte
	{
		[Display(Name = "Historial")]
		EstadoCuenta = 0,

		[Display(Name = "Estado de Cuenta")]
		FacturasPendientes = 1,

		[Display(Name = "Facturas Canceladas")]
		FacturasCanceladas = 2

		//[Display(Name = "Estado de cuenta")]
		//EstadoCuenta = 0,

		//[Display(Name = "Facturas Pendientes")]
		//FacturasPendientes = 1

	}

	public enum Departamentos
	{
		Sistemas = 0,
		Contabilidad = 1,
		Ventas = 2,
		Compras = 3,
		[Display(Name = "Almacén")]
		Almacen = 4,
		[Display(Name = "Recusos Humanos")]
		RRHH = 5,
		[Display(Name = "Administación")]
		Administracion = 6,
		[Display(Name = "Planeación")]
		Planeacion = 7,
	}

	public enum Puestos
	{
		Auxiliar = 0,
		Supervisor = 1,
		Gerente = 2,
		Director = 3,
		Contador = 4,
		Reclutador = 5,
		[Display(Name = "Técnico")]
		Tecnico = 6
	}

	public enum GrupoProveedor
	{ 
		//Proveedores = 101, --5,508
		[Display(Name = "Fleteros de Piedra")] 
		FleterosPiedra = 106,
		Filiales = 107
	}
	public enum GrupoCliente
	{ //,{"GroupCode":110,"GroupName":"papeleria y utlies"}]
		Ferretero = 100,
		Agricola = 102,
		Constructor = 103,
		Distribuidor = 104,
		[Display(Name = "Acuícola")]
		Acuicola = 105,
		[Display(Name = "Otros Servicios")]
		Otros = 108,
		Consignatario = 109,
		[Display(Name = "Papelería y útiles")]
		Papeleria = 110
	}
}
