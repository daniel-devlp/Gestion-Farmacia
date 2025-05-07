using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmacia_Entidades
{
    public class VentaDetalle_Entidades
    {
        public int Id { get; set; }
        public Producto_Entidades producto { get; set; }
        public int Cantidad { get; set; }
        public double PrecioVenta { get; set; }
        public double Subtotal { get; set; }
        public VentaDetalle_Entidades()
        {

        }

        public VentaDetalle_Entidades(int id,Producto_Entidades producto,int cantidad,
            double precioVenta, double subtotal)
        {
            Id = id;
            this.producto = producto;
            Cantidad = cantidad;
            PrecioVenta = precioVenta;
            Subtotal = subtotal;
        }
    }


    public class VentaDetalleResumen
    {
        public int Id { get; set; }
        public string NombreComercial { get; set; }
        public string Presentacion { get; set; }
        public int Cantidad { get; set; }
        public double PrecioVenta { get; set; }
        public double Subtotal { get; set; }
        public VentaDetalleResumen()
        {

        }

        public VentaDetalleResumen(int id, string nombreComercial,
            string presentacion, int cantidad, double precioVenta, double subtotal)
        {
            Id = id;
            NombreComercial = nombreComercial;
            Presentacion = presentacion;
            Cantidad = cantidad;
            PrecioVenta = precioVenta;
            Subtotal = subtotal;
        }
    }
}
