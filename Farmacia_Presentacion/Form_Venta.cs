using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Farmacia_Entidades;
using Farmacia_Negocio;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

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
            rtbFactura.Font = new System.Drawing.Font("Courier New", 10); // Fuente monoespaciada
            ActualizarVistaFactura();
        }

        private void InicializarValoresVenta()
        {
            venta.ListaVentaDetalle = new List<VentaDetalle_Entidades>();
            venta.FechaComprobante =DateTime.Now;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            LlamarFormularioClientes();
        }
        private void LlamarFormularioClientes()
        {
            Clientes form_Clientes = new Clientes();
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

        private void CargarDatosClientePantalla(int id)
        {
            cliente = Clientes_Negocio.DevolverClientePorId(id);
            textBoxID.Text = cliente.Id.ToString();
            textBox_Apellidos.Text = cliente.Apellidos;
            textBox_Nombres.Text = cliente.Nombres;
            textBox_CedulaRuc.Text = cliente.Cedula;
            textBox_Direccion.Text = cliente.Direccion;
            textBox_Telefono.Text = cliente.Telefono;
            textBox_Correo.Text = cliente.Correo;
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
            try
            {
                // 1. Validaciones básicas
                if (ValidarCantidad())
                    return;

                if (venta.Cliente == null)
                {
                    MessageBox.Show("Por favor ingrese un cliente", "Advertencia",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (_productoSeleccionado == null)
                {
                    MessageBox.Show("Por favor seleccione un producto", "Advertencia",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int cantidad = Convert.ToInt32(textBox_Cantidad.Text);
                if (_productoSeleccionado.Stock <= 0)
                {
                    MessageBox.Show($"El producto {_productoSeleccionado.NombreComercial} no tiene stock disponible",
                                  "Stock agotado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (cantidad > _productoSeleccionado.Stock)
                {
                    MessageBox.Show($"No hay suficiente stock. Disponible: {_productoSeleccionado.Stock}",
                                  "Stock insuficiente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 2. Si es el primer producto, generar número de comprobante (pero no guardar aún)
                if (venta.Id == 0)
                {
                    if (string.IsNullOrEmpty(venta.NumeroComprobante))
                    {
                        venta.NumeroComprobante = GenerarNumeroComprobante();
                    }

                    MessageBox.Show($"Venta iniciada. N°: {venta.NumeroComprobante}",
                                  "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // 3. Verificar si el producto ya está en la venta
                var detalleExistente = venta.ListaVentaDetalle.FirstOrDefault(d => d.producto.Id == _productoSeleccionado.Id);

                // 4. Actualizar la lista de productos localmente (sin guardar en BD aún)
                if (detalleExistente != null)
                {
                    detalleExistente.Cantidad += cantidad;
                    detalleExistente.Subtotal = detalleExistente.Cantidad * detalleExistente.PrecioVenta;
                }
                else
                {
                    AgregarProductoVenta();
                }

                // 5. Actualizar el stock localmente (sin guardar en BD aún)
                _productoSeleccionado.Stock -= cantidad;

                // 6. Actualizar la interfaz
                ActualizarVistaFactura();
                CalcularMontoVenta();
                textBox_NumeroComprobante.Text = venta.NumeroComprobante;

                MessageBox.Show($"Producto agregado. Stock restante: {_productoSeleccionado.Stock}",
                              "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (FormatException)
            {
                MessageBox.Show("Por favor ingrese valores numéricos válidos", "Error de formato",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (OverflowException)
            {
                MessageBox.Show("La cantidad ingresada es demasiado grande", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inesperado: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                LimpiarControlesProducto();
            }
        }
        private void LimpiarControlesProducto()
        {
            textBox_NombreGenerico.Clear();
            text_NombreComercial.Clear();
            textBox_Precio.Clear();
            textBox_Presentacion.Clear();
            textBox_Cantidad.Clear();
            _productoSeleccionado = null;
        }
        private string GenerarNumeroComprobante()
        {
            return $"FAC-{DateTime.Now:yyyyMMddHHmmss}";
        }

        private void LimpiarFormulario(){

            try
            {
                // 1. Reiniciar la entidad venta
                venta = new VentaCabecera_Entidades
                {
                    ListaVentaDetalle = new List<VentaDetalle_Entidades>(),
                    FechaComprobante = DateTime.Now,
                };

                // 2. Limpiar controles de cliente
                textBoxID.Clear();
                textBox_CedulaRuc.Clear();
                textBox_Nombres.Clear();
                textBox_Apellidos.Clear();
                textBox_Telefono.Clear();
                textBox_Correo.Clear();
                textBox_Direccion.Clear();

                // 3. Limpiar controles de productos
                textBox_NombreGenerico.Clear();
                text_NombreComercial.Clear();
                textBox_Precio.Clear();
                textBox_Presentacion.Clear();
                textBox_Cantidad.Clear();
                _productoSeleccionado = null;

                // 4. Limpiar totales
                textBoxIVA.Clear();
                textBoxSUBT.Clear();
                textBoxTOTAL.Clear();

                // 5. Reiniciar DataGridView
                dataGridView_DetalleVenta.DataSource = null;
                dataGridView_DetalleVenta.Rows.Clear();

                // 6. Actualizar fecha y número de comprobante
                venta.NumeroComprobante = GenerarNumeroComprobante();
                textBox_NumeroComprobante.Text = venta.NumeroComprobante;

                // 7. Establecer foco en campo inicial
                textBox_CedulaRuc.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al limpiar formulario: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                           // int ventaId = FacturaNegocio.GuardarVentaCabecera(venta);
                            venta.Id = ventaId;
                            MessageBox.Show("Venta guardada con éxito. ID: " + ventaId, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            // Guardar solo los detalles de la venta usando el ID existente
                            //FacturaNegocio.GuardarVentaDetalle(venta.Id, venta.ListaVentaDetalle);
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

        private void button_Nuevo_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
            rtbFactura.Font = new System.Drawing.Font("Courier New", 10); // Fuente monoespaciada
            ActualizarVistaFactura();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
        private void ActualizarVistaFactura()
        {
            if(venta.Cliente == null || venta.ListaVentaDetalle.Count == 0)
{
                rtbFactura.Text = "No hay datos de factura disponibles";
                return;
            }

            StringBuilder sb = new StringBuilder();

            // Diseño modernizado de la factura
            sb.AppendLine("╔══════════════════════════════════════════════════════════╗");
            sb.AppendLine("║               FARMACIAS ECONOMICAS PLUS                 ║");
            sb.AppendLine("╟──────────────────────────────────────────────────────────╢");
            sb.AppendLine("║ Av. El Rey ∙ Tel: 0988162040 ∙ RUC: 1234567890123       ║");
            sb.AppendLine("╚══════════════════════════════════════════════════════════╝");
            sb.AppendLine();
            sb.AppendLine("          ╔══════════════════════════════════╗");
            sb.AppendLine($"          ║        FACTURA N° {venta.NumeroComprobante.PadRight(10)}    ║");
            sb.AppendLine($"          ║        {DateTime.Now:dd/MM/yyyy HH:mm}          ║");
            sb.AppendLine("          ╚══════════════════════════════════╝");
            sb.AppendLine();

            // Sección de cliente con diseño mejorado
            sb.AppendLine("┌─────────────────── INFORMACIÓN DEL CLIENTE ──────────────────┐");
            sb.AppendLine($"│ Nombre:    {venta.Cliente.Nombres} {venta.Cliente.Apellidos}".PadRight(60) + "│");
            sb.AppendLine($"│ Cédula:    {venta.Cliente.Cedula}".PadRight(60) + "│");
            sb.AppendLine($"│ Dirección: {venta.Cliente.Direccion}".PadRight(60) + "│");
            sb.AppendLine($"│ Teléfono:  {venta.Cliente.Telefono}".PadRight(60) + "│");
            sb.AppendLine("└──────────────────────────────────────────────────────────────┘");
            sb.AppendLine();

            // Tabla de productos con bordes estilizados
            sb.AppendLine("┌──────────────────────────────┬────────┬───────────┬──────────────┐");
            sb.AppendLine("│          PRODUCTO            │ CANT.  │  P.UNIT   │   SUBTOTAL   │");
            sb.AppendLine("├──────────────────────────────┼────────┼───────────┼──────────────┤");

            foreach (var detalle in venta.ListaVentaDetalle)
            {
                sb.AppendLine(
                    "│ " + detalle.producto.NombreComercial.PadRight(28) +
                    "│ " + detalle.Cantidad.ToString().PadLeft(6) +
                    " │ " + detalle.PrecioVenta.ToString("0.00").PadLeft(7) +
                    " │ " + detalle.Subtotal.ToString("0.00").PadLeft(12) + " │"
                );
            }

            sb.AppendLine("└──────────────────────────────┴────────┴───────────┴──────────────┘");
            sb.AppendLine();

            // Totales con diseño mejorado
            double subtotal = venta.ListaVentaDetalle.Sum(d => d.Subtotal);
            double iva = subtotal * 0.12;
            double total = subtotal + iva;

            sb.AppendLine("┌──────────────────────────────────────────────────────────────┐");
            sb.AppendLine($"│ SUBTOTAL: {"".PadLeft(38)}{subtotal.ToString("0.00").PadLeft(15)} │");
            sb.AppendLine($"│ IVA (12%): {"".PadLeft(37)}{iva.ToString("0.00").PadLeft(15)} │");
            sb.AppendLine($"│ TOTAL: {"".PadLeft(41)}{total.ToString("0.00").PadLeft(15)} │");
            sb.AppendLine("└──────────────────────────────────────────────────────────────┘");
            sb.AppendLine();
            sb.AppendLine("            ╔════════════════════════════╗");
            sb.AppendLine("            ║   ¡GRACIAS POR SU COMPRA!  ║");
            sb.AppendLine("            ╚════════════════════════════╝");

            rtbFactura.Text = sb.ToString();
        }

        private void GuardarFacturaComoPDF()
        {
            if (string.IsNullOrEmpty(rtbFactura.Text) || rtbFactura.Text == "No hay datos de factura disponibles")
            {
                MessageBox.Show("No hay datos de factura para guardar", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf";
            saveFileDialog.FileName = $"Factura_{venta.NumeroComprobante}.pdf";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Crear documento PDF
                    Document document = new Document(PageSize.A4, 20, 20, 30, 30);
                    PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(saveFileDialog.FileName, FileMode.Create));

                    document.Open();

                    // Configurar fuente utilizando iTextSharp.text.Font
                    iTextSharp.text.Font font = FontFactory.GetFont(FontFactory.COURIER, 10);

                    // Dividir el texto del RichTextBox por líneas
                    var lines = rtbFactura.Text.Split(new[] { "\n" }, StringSplitOptions.None);

                    foreach (string line in lines)
                    {
                        // Agregar cada línea con la fuente configurada
                        document.Add(new Paragraph(line, font));
                    }

                    document.Close();

                    MessageBox.Show("Factura guardada como PDF exitosamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarFormulario();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al guardar el PDF: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Validar que haya productos en la venta
                if (venta.ListaVentaDetalle.Count == 0)
                {
                    MessageBox.Show("No hay productos en la venta", "Advertencia",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Validar cliente
                if (venta.Cliente == null)
                {
                    MessageBox.Show("Debe seleccionar un cliente", "Advertencia",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Generar número de comprobante si no existe
                if (string.IsNullOrEmpty(venta.NumeroComprobante))
                {
                    venta.NumeroComprobante = GenerarNumeroComprobante();
                }

                // Guardar cabecera de venta si no está guardada
                if (venta.Id == 0)
                {
                    venta.Id = FacturaNegocio.GuardarVentaCabecera(venta);
                }

                // Guardar TODOS los detalles de venta en una sola operación
                FacturaNegocio.GuardarVentaDetalle(venta.Id, venta.ListaVentaDetalle);

                // Actualizar stocks para todos los productos
                //foreach (var detalle in venta.ListaVentaDetalle)
               // {
                    // IMPORTANTE: Usar valor negativo para DESCONTAR el stock
                    //Productos_Negocio.ActualizarStock(detalle.producto.Id, -detalle.Cantidad);
                //}

                // Generar PDF
                GuardarFacturaComoPDF();

                // Mostrar resumen
                MessageBox.Show($"Venta confirmada exitosamente.\nNúmero: {venta.NumeroComprobante}",
                              "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Limpiar formulario
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al confirmar la venta: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form_BusquedaFactura form = new Form_BusquedaFactura();
            form.ShowDialog();
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}