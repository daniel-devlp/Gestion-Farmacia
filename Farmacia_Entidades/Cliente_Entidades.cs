using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmacia_Entidades
{
    public class Cliente_Entidades
    {
        public int Id { get; set; }
        public string Apellidos { get; set; }
        public string Nombres { get; set; }
        public string Cedula { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }

        public Cliente_Entidades()
        {

        }
        public Cliente_Entidades(int id, string apellidos, string nombres, string cedula, string direccion, string telefono, string correo)
        {
            Id = id;
            Apellidos = apellidos;
            Nombres = nombres;
            Cedula = cedula;
            Direccion = direccion;
            Telefono = telefono;
            Correo = correo;
        }
        public String NombreCompleto()
        {
            return Apellidos + " " + Nombres;
        }
    }
}
