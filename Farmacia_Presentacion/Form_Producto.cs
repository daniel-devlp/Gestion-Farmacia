using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Farmacia_Entidades;
using Farmacia_Negocio;

namespace Farmacia_Presentacion
{
    public partial class Form_Producto : Form
    {
        public Producto_Entidades ProductoSeleccionado { get; set; }
        private List<Producto_Entidades> _productoOriginales;
        public Form_Producto()
        {
            InitializeComponent();
        }

        private void Form_Producto_Load(object sender, EventArgs e)
        {
            CargarProductosEnDataGridView();
            _productoOriginales = Productos_Negocio.DevolverListadoProductos();
        }

        private void CargarProductosEnDataGridView()
        {
            dataGridView_Productos.DataSource = Productos_Negocio.DevolverListadoProductos();
        }

        private void dataGridView_Productos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var id = Convert.ToUInt32( dataGridView_Productos.Rows[e.RowIndex].Cells["Id"].Value.ToString());  
            ProductoSeleccionado = DevolverCoincidencia((int)id);
            this.Close();
        }

        private void FiltrarPorNombre()
        {
            string apellido = textBox_Nombre.Text.Trim().ToLower();

            var listaFiltrada = _productoOriginales
                .Where(c => !string.IsNullOrEmpty(c.NombreComercial) && c.NombreComercial.ToLower().Contains(apellido))
                .ToList();

            dataGridView_Productos.DataSource = null;
            dataGridView_Productos.DataSource = listaFiltrada;
        }

        private Producto_Entidades DevolverCoincidencia(int id)
        {
            foreach (var item in Productos_Negocio.DevolverListadoProductos())
            {
                if (item.Id==id)
                {
                    return item;
                }
            }
            return null;
        }

        private void textBox_Nombre_TextChanged(object sender, EventArgs e)
        {
            FiltrarPorNombre();
        }
    }
}
