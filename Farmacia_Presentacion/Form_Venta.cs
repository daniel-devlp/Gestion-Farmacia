using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using Farmacia_Entidades;
using Farmacia_Negocio;

namespace Farmacia_Presentacion
{
    public partial class Form_Venta : Form
    {
        //variables de inicio
        private int ventaId;
        VentaCabecera_Entidades venta = new VentaCabecera_Entidades();
        Producto_Entidades _productoSeleccionado;
        Cliente_Entidades _clienteSeleccionado;
        Cliente_Entidades cliente = new Cliente_Entidades();
        public Form_Venta()
        {
            InitializeComponent();
        }
        private void Form_Venta_Load(object sender, EventArgs e)
        {
            InicializarValoresVenta();
        }

        private void InicializarValoresVenta()
        {
            venta.ListaVentaDetalle = new List<VentaDetalle_Entidades>();
            venta.FechaComprobante = DateTime.Now;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            LlamarFormularioClientes();
        }
        private void LlamarFormularioClientes()
        {
            Form_Clientes form_Clientes = new Form_Clientes();
            form_Clientes.ShowDialog();
            _clienteSeleccionado = form_Clientes.ClienteSeleccionado;
            if (form_Clientes.ClienteSeleccionado != null)
            {
                textBox_CedulaRuc.Text = _clienteSeleccionado.Cedula.ToString();
                textBox_Nombres.Text = _clienteSeleccionado.Nombres.ToString();
                textBox_Apellidos.Text = _clienteSeleccionado.Apellidos.ToString();
                textBox_Telefono.Text = _clienteSeleccionado.Telefono.ToString();
                textBox_Correo.Text = _clienteSeleccionado.Correo.ToString();
                textBox_Direccion.Text = _clienteSeleccionado.Direccion.ToString();
                textBoxID.Text = _clienteSeleccionado.Id.ToString();
                venta.Cliente = _clienteSeleccionado;
            }
        }
        #region Codigo Cabecera

        private void BuscarClientePorCedula(string cedulaRuc)
        {
            //esta variable es true cuando encuentre un cliente por medio de la busqueda
            bool resultado = false;
            //buscar por el criterio
            foreach (var item in Clientes_Negocio.DevolverListadoClientes())
            {
                if (item.Cedula == cedulaRuc)
                {
                    //Asociamos el cliente encontrado al objeto venta.cliente
                    venta.Cliente = item;
                    //Cargar los datos en pantalla
                    CargarDatosClientePantalla(item.Id);
                    resultado = true;
                }
            }
            //No encontro el cliente
            if (!resultado)
            {
                EncerarDatos();
                MessageBox.Show("No se encontro a un cliente con esa Cedula",
                    "Cliente no encontrado",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox_CedulaRuc.Focus();
            }
        }

        private void EncerarDatos()
        {
            textBox_Apellidos.Text = string.Empty;
            textBox_Nombres.Text = string.Empty;
            textBox_Correo.Text = string.Empty;
            textBox_Telefono.Text = string.Empty;
            textBox_Direccion.Text = string.Empty;
            textBoxID.Text = string.Empty;
        }

        private void CargarDatosClientePantalla(int id)
        {
            cliente = Clientes_Negocio.DevolverClientePorId(id);
            textBoxID.Text = cliente.Id.ToString();
            textBox_Apellidos.Text = cliente.Apellidos;
            textBox_Nombres.Text = cliente.Nombres;
            textBox_CedulaRuc.Text = cliente.Cedula;
            textBox_Direccion.Text= cliente.Direccion;
            textBox_Telefono.Text= cliente.Telefono;
            textBox_Correo.Text= cliente.Correo;
        }

        #endregion

        private void button_Productos_Click(object sender, EventArgs e)
        {
            LlamarFormularioProductos();
        }

        private void LlamarFormularioProductos()
        {
            Form_Producto form_Producto = new Form_Producto();
            form_Producto.ShowDialog();
            _productoSeleccionado = form_Producto.ProductoSeleccionado;
            if (form_Producto.ProductoSeleccionado != null)
            {
                textBox_NombreGenerico.Text = _productoSeleccionado.NombreGenerico;
                text_NombreComercial.Text = _productoSeleccionado.NombreComercial;
                textBox_Precio.Text = _productoSeleccionado.Precio.ToString();
                textBox_Presentacion.Text = _productoSeleccionado.Presentacion;
                textBox_Cantidad.Focus();
            }
        }

        private void button_Agregar_Click(object sender, EventArgs e)
        {
            if (ValidarCantidad())
                return;
            if (venta.Cliente != null)
            {
                AgregarProductoVenta();
                CalcularMontoVenta();
            }
            else
            {
                MessageBox.Show("Por favor Ingrese un cliente", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private bool ValidarCantidad()
        {
            if (textBox_Cantidad.Text == "" || textBox_Cantidad.Text == "0")
            {
                MessageBox.Show("Ingrese la cantidad del Producto",
                    "Advertencia",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                textBox_Cantidad.Focus();
                return true;
            }
            return false;
        }

        private void CalcularMontoVenta()
        {
            CalcularIVA();
            CalcularSubtotal();
            CalcularTotal();
        }
        private void CalcularTotal()
        {
            var iva = Convert.ToDouble(textBoxIVA.Text);
            var subtototal = Convert.ToDouble(textBoxSUBT.Text);
            var total = iva + subtototal;
            textBoxTOTAL.Text = total.ToString("0.00");

        }

        private void CalcularSubtotal()
        {
            double subtotal = 0;
            foreach (var item in venta.ListaVentaDetalle)
            {
                subtotal += item.Subtotal;
            }
            textBoxSUBT.Text = subtotal.ToString("0.00");
        }

        private void CalcularIVA()
        {
            double subtotal = 0, iva = 0;
            foreach (var item in venta.ListaVentaDetalle)
            {
                subtotal += item.Subtotal;
            }
            iva = subtotal * 0.12;
            textBoxIVA.Text = iva.ToString("0.00");
        }

        private void AgregarProductoVenta()
        {
            if (_productoSeleccionado != null)
            {
                int cantidad = Convert.ToInt32(textBox_Cantidad.Text);
                double precio = Convert.ToDouble(textBox_Precio.Text);
                double subtotal = cantidad * precio;

                // Añadir el detalle de venta a la lista
                venta.ListaVentaDetalle.Add(new VentaDetalle_Entidades(
                    _productoSeleccionado.Id, _productoSeleccionado, cantidad, precio, subtotal
                ));

                // Crear una lista de resumen
                List<VentaDetalleResumen> listaResumen = new List<VentaDetalleResumen>();
                foreach (var item in venta.ListaVentaDetalle)
                {
                    listaResumen.Add(new VentaDetalleResumen(
                        item.Id,
                        item.producto.NombreComercial,
                        item.producto.Presentacion,
                        item.Cantidad,
                        item.PrecioVenta,
                        item.Subtotal
                    ));
                }

                // Validar si la venta tiene cliente y detalles de venta
                if (venta.Cliente != null && venta.ListaVentaDetalle.Count > 0)
                {
                    try
                    {
                        // Guardar la venta si es nueva o actualizar si ya existe
                        if (venta.Id == 0)
                        {
                            // Guardar la cabecera de la venta y obtener el ID
                            int ventaId = FacturaNegocio.GuardarVentaCabecera(venta);
                            venta.Id = ventaId;
                            MessageBox.Show("Venta guardada con éxito. ID: " + ventaId, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            // Guardar solo los detalles de la venta usando el ID existente
                            FacturaNegocio.GuardarVentaDetalle(venta.Id, venta.ListaVentaDetalle);
                            MessageBox.Show("Producto añadido a la venta existente. ID: " + venta.Id, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }

                        // Opcionalmente, puedes reiniciar el formulario o limpiar los campos aquí
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al guardar la venta: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Debe ingresar un cliente y al menos un producto.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                // Actualizar el DataGridView con la lista de resumen
                dataGridView_DetalleVenta.DataSource = null;
                dataGridView_DetalleVenta.DataSource = listaResumen;
            }
            else
            {
                MessageBox.Show("Por favor seleccion un producto", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        #region Validacion de Ingreso de Los Text Boxs
        private void textBox_CedulaRuc_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsDigit(e.KeyChar)) && (e.KeyChar != (char)Keys.Back))
            {
                e.Handled = true;
                return;
            }
        }

        private void textBox_Telefono_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsDigit(e.KeyChar)) && (e.KeyChar != (char)Keys.Back))
            {
                e.Handled = true;
                return;
            }
        }

        private void textBox_Apellidos_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsLetter(e.KeyChar)) && (e.KeyChar != ((char)Keys.Back) && e.KeyChar != (char)Keys.Space))
            {
                e.Handled = true;
                return;
            }
        }

        private void textBox_Nombres_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsLetter(e.KeyChar)) && (e.KeyChar != ((char)Keys.Back) && e.KeyChar != (char)Keys.Space))
            {
                e.Handled = true;
                return;
            }
        }

        private void textBox_Direccion_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsLetterOrDigit(e.KeyChar)) && (e.KeyChar != ((char)Keys.Back) && e.KeyChar != (char)Keys.Space))
            {
                e.Handled = true;
                return;
            }
        }

        private void textBox_Correo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsLetterOrDigit(e.KeyChar)) && (e.KeyChar != '@') && (e.KeyChar != '.') && (e.KeyChar != (char)Keys.Back) && (e.KeyChar != (char)Keys.Delete))
            {
                e.Handled = true;
                return;
            }
        }

        private void textBox_Cantidad_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsDigit(e.KeyChar)) && (e.KeyChar != (char)Keys.Back))
            {
                e.Handled = true;
                return;
            }
        }
        #endregion


        
        private void nuevaVentaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBoxID.Text = string.Empty;
            textBox_CedulaRuc.Clear();
            textBox_Nombres.Clear();
            textBox_Apellidos.Clear();
            textBox_Telefono.Clear();
            textBox_Correo.Clear();
            textBox_Direccion.Clear();
            textBoxIVA.Clear();
            textBoxSUBT.Clear();
            textBoxTOTAL.Clear();
            textBox_Cantidad.Clear();
            venta.ListaVentaDetalle.Clear();
            dataGridView_DetalleVenta.DataSource = null;
            venta = new VentaCabecera_Entidades();  
        }

        private void button_Clientes_Click(object sender, EventArgs e)
        {
            GuardarCliente();
            CargarDatosClientePantalla(cliente.Id);
        }
        private void GuardarCliente()
        {
            cliente.Apellidos = textBox_Apellidos.Text.Trim().ToUpper();
            cliente.Nombres = textBox_Nombres.Text.Trim().ToUpper();
            cliente.Cedula = textBox_CedulaRuc.Text;
            cliente.Direccion = textBox_Direccion.Text.Trim().ToUpper();
            cliente.Telefono = textBox_Telefono.Text;
            cliente.Correo = textBox_Correo.Text;
            if (cliente != null)
            {
                cliente = Clientes_Negocio.Guardar(cliente);
                textBoxID.Text = cliente.Id.ToString();
                MessageBox.Show("Datos Guardados con exito", "Exito");
            }
            else
            {
                MessageBox.Show("Error al guardar los datos", "Error");
            }
        }
    }
}
