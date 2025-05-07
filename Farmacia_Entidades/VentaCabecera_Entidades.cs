using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmacia_Entidades
{
    public class VentaCabecera_Entidades
    {
        public int Id { get; set; }
        public Cliente_Entidades Cliente { get; set; }
        public DateTime FechaComprobante { get; set; }
        public string NumeroComprobante { get; set; }
        public List<VentaDetalle_Entidades> ListaVentaDetalle { get; set; }
        public VentaCabecera_Entidades() { }

        public VentaCabecera_Entidades(int id, Cliente_Entidades cliente, DateTime fechaComprobante, string numeroComprobante, List<VentaDetalle_Entidades> listaVentaDetalle)
        {
            Id = id;
            Cliente = cliente;
            FechaComprobante = fechaComprobante;
            NumeroComprobante = numeroComprobante;
            ListaVentaDetalle = listaVentaDetalle;
        }

        public VentaCabecera_Entidades(int id, Cliente_Entidades cliente, DateTime fechaComprobante, string numeroComprobante)
        {
            Id = id;
            Cliente = cliente;
            FechaComprobante = fechaComprobante;
            NumeroComprobante = numeroComprobante;
        }
    }
}
