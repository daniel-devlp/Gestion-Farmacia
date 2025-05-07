using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmacia_Entidades
{
    public class Producto_Entidades
    {
        public int Id { get; set; }
        public string NombreComercial { get; set; }
        public string NombreGenerico { get; set; }
        public string Presentacion { get; set; }
        public double Precio { get; set; }
        public int Stock { get; set; }
        public Producto_Entidades()
        {

        }

        public Producto_Entidades(int id, string nombreComercial, string nombreGenerico,
            string presentacion, double precio, int stock)
        {
            Id = id;
            NombreComercial = nombreComercial;
            NombreGenerico = nombreGenerico;
            Presentacion = presentacion;
            Precio = precio;
            Stock = stock;
        }
    }
}
