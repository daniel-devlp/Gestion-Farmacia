using Farmacia_Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Farmacia_Datos_LINQ;
using Farmacia_Datos_SQLServer;

namespace Farmacia_Negocio
{
    public static class Clientes_Negocio
    {
        public static Cliente_Entidades Guardar(Cliente_Entidades cliente)
        {
            if (cliente.Id != 0)
            {
                //Nueva Persona Inscrita
                return Cliente_Datos.Actualizar(cliente);
            }
            else
            {
                //Actualizacion de Persona Inscrita
                return Cliente_Datos.Nuevo(cliente);
            }
        }
        public static List<Cliente_Entidades> DevolverListadoClientes()
        {
            return Cliente_Datos.DevolverListadoClientes();
        }
        public static Cliente_Entidades DevolverClientePorId(int idCliente)
        {
            return Cliente_Datos.DevolverClientePorId(idCliente);
        }
    }
}
