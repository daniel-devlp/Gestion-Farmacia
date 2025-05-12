using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Farmacia_Datos_LINQ;
using Farmacia_Datos_SQLServer;
using Farmacia_Entidades;

namespace Farmacia_Negocio
{
    public static class Productos_Negocio
    {
        public static List<Producto_Entidades> DevolverListadoProductos()
        {
            return Producto_Datos.DevolverListadoProductos();
        }
        public static void DescontarStock(int idProducto, int cantidad)
        {
            Producto_Datos.DescontarStock(idProducto, cantidad);
        }
        public static void ObtenerProductoPorId(int idProducto, int cantidad)
        {
            Producto_Datos.ObtenerProductoPorId(idProducto);
        }
        public static void ActualizarStock(int idProducto, int cantidad)
        {
            Producto_Datos.ActualizarStock(idProducto, cantidad);
        }
    }
}
