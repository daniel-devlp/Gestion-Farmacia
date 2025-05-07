using Farmacia_Datos_LINQ;
using Farmacia_Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmacia_Negocio
{
    public static class FacturaNegocio
    {
        public static int GuardarVentaCabecera(VentaCabecera_Entidades venta)
        {
            return Maestro_Factura_Datos.GuardarVentaCabecera(venta);
        }
        public static void GuardarVentaDetalle(int ventaId, List<VentaDetalle_Entidades> detalles)
        {
            Maestro_Factura_Datos.GuardarVentaDetalle(ventaId,detalles);
        }
    }
}
