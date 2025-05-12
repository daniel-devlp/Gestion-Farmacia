using Farmacia_Entidades;
using Farmacia_Negocio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Farmacia_Presentacion
{
    public partial class Clientes : Form
    {
        public Cliente_Entidades ClienteSeleccionado { get; set; }

        private List<Cliente_Entidades> _clientesOriginales;

        public Clientes()
        {
            InitializeComponent();
        }

        private void Form_Clientes_Load(object sender, EventArgs e)
        {
            _clientesOriginales = Clientes_Negocio.DevolverListadoClientes();
            dataGridViewClientes.DataSource = _clientesOriginales;
        }

        private void txtBuscarCedula_TextChanged(object sender, EventArgs e)
        {
            FiltrarPorCedula();
        }

        private void txtBuscarApellido_TextChanged(object sender, EventArgs e)
        {
            FiltrarPorApellido();
        }

        private void FiltrarPorCedula()
        {
            string cedula = textBox_Cedula.Text.Trim().ToLower();

            var listaFiltrada = _clientesOriginales
                .Where(c => !string.IsNullOrEmpty(c.Cedula) && c.Cedula.ToLower().Contains(cedula))
                .ToList();

            dataGridViewClientes.DataSource = null;
            dataGridViewClientes.DataSource = listaFiltrada;
        }

        private void FiltrarPorApellido()
        {
            string apellido = textBox_Apellido.Text.Trim().ToLower();

            var listaFiltrada = _clientesOriginales
                .Where(c => !string.IsNullOrEmpty(c.Apellidos) && c.Apellidos.ToLower().Contains(apellido))
                .ToList();

            dataGridViewClientes.DataSource = null;
            dataGridViewClientes.DataSource = listaFiltrada;
        }

        private void dataGridViewClientes_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var id = Convert.ToUInt32(dataGridViewClientes.Rows[e.RowIndex].Cells["Id"].Value.ToString());
            ClienteSeleccionado = DevolverCoincidencia((int)id);
            this.Close();
        }
        private Cliente_Entidades DevolverCoincidencia(int id)
        {
            foreach (var item in Clientes_Negocio.DevolverListadoClientes())
            {
                if (item.Id == id)
                {
                    return item;
                }
            }
            return null;
        }
    }
}
