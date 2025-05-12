using Farmacia_Entidades;
using Farmacia_Negocio;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Farmacia_Presentacion
{
    public partial class Form_BusquedaFactura : Form
    {

        private List<Cliente_Entidades> _clientesOriginales;
        public Form_BusquedaFactura()
        {
            InitializeComponent();
        }

        private void Form_BusquedaFactura_Load(object sender, EventArgs e)
        {

        }
        private VentaCabecera_Entidades ObtenerFacturaCompleta(string codigoFactura)
        {
            string queryCabecera = @"SELECT vc.id, vc.id_cliente, vc.FechaComprobante, 
                   vc.NumeroComprobante,
                   c.nombre, c.apellido, c.cedula, c.direccion, c.telefono, c.correo
                   FROM VentaCabecera vc
                   INNER JOIN Clientes c ON vc.id_cliente = c.id
                   WHERE vc.NumeroComprobante = @Codigo";

            using (SqlConnection connection = new SqlConnection("Data Source=Daniel\\PUNTO_A;Initial Catalog=GestionFarmacia;Persist Security Info=True;User ID=sa;Password=admin123;TrustServerCertificate=True;"))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand(queryCabecera, connection))
                {
                    cmd.Parameters.AddWithValue("@Codigo", codigoFactura);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read()) return null;

                        var factura = new VentaCabecera_Entidades
                        {
                            Id = reader.GetInt32(0),
                            Cliente = new Cliente_Entidades
                            {
                                Id = reader.GetInt32(1),
                                Nombres = reader.GetString(4), // Índice ajustado
                                Apellidos = reader.GetString(5),
                                Cedula = reader.GetString(6),
                                Direccion = reader.GetString(7),
                                Telefono = reader.GetString(8),
                                Correo = reader.IsDBNull(9) ? null : reader.GetString(9) // Índice ajustado
                            },
                            FechaComprobante = reader.GetDateTime(2),
                            NumeroComprobante = reader.GetString(3),
                            ListaVentaDetalle = new List<VentaDetalle_Entidades>()
                        };

                        reader.Close();

                        string queryDetalle = @"SELECT vd.id, vd.id_Producto, vd.Cantidad, 
                              vd.PrecioVenta, vd.Subtotal,
                              p.nombreComercial, p.presentacion
                              FROM VentaDetalle vd
                              INNER JOIN Productos p ON vd.id_Producto = p.id
                              WHERE vd.Id_Venta = @IdVenta";

                        using (SqlCommand cmdDetalle = new SqlCommand(queryDetalle, connection))
                        {
                            cmdDetalle.Parameters.AddWithValue("@IdVenta", factura.Id);

                            using (SqlDataReader readerDetalle = cmdDetalle.ExecuteReader())
                            {
                                while (readerDetalle.Read())
                                {
                                    try
                                    {
                                        var detalle = new VentaDetalle_Entidades(
                                            readerDetalle.GetInt32(1),
                                            new Producto_Entidades
                                            {
                                                Id = readerDetalle.GetInt32(1),
                                                NombreComercial = readerDetalle.GetString(5),
                                                Presentacion = readerDetalle.GetString(6)
                                            },
                                            readerDetalle.GetInt32(2),
                                            Convert.ToDouble(readerDetalle["PrecioVenta"]),
                                            Convert.ToDouble(readerDetalle["Subtotal"])
                                        );

                                        factura.ListaVentaDetalle.Add(detalle);
                                    }
                                    catch (InvalidCastException ex)
                                    {
                                        // Registrar el error y continuar con el siguiente registro
                                        Console.WriteLine($"Error al convertir datos: {ex.Message}");
                                        continue;
                                    }
                                }
                            }
                        }

                        return factura;
                    }
                }
            }
        }
        private void MostrarFacturaEnRichTextBox(VentaCabecera_Entidades factura)
        {
            // Limpiar el RichTextBox
            rtb_Factura.Clear();

            // Configurar fuente monoespaciada
            rtb_Factura.Font = new System.Drawing.Font("Courier New", 10);

            // Generar contenido de la factura
            StringBuilder sb = new StringBuilder();

            // Encabezado
            sb.AppendLine("FARMACIA TU SALUD".PadLeft(50));
            sb.AppendLine("Av. Atahualpa, Ciudad: Ambato".PadLeft(50));
            sb.AppendLine("Tel: (02) 123-4567 | RUC: 1234567890123".PadLeft(55));
            sb.AppendLine();
            sb.AppendLine("FACTURA".PadLeft(45));
            sb.AppendLine($"Número: {factura.NumeroComprobante}".PadLeft(50));
            sb.AppendLine($"Fecha: {factura.FechaComprobante:dd/MM/yyyy HH:mm}".PadLeft(50));
            sb.AppendLine();

            // Datos del cliente
            sb.AppendLine("DATOS DEL CLIENTE:");
            sb.AppendLine($"Nombre: {factura.Cliente.Nombres} {factura.Cliente.Apellidos}");
            sb.AppendLine($"Cédula/RUC: {factura.Cliente.Cedula}");
            sb.AppendLine($"Dirección: {factura.Cliente.Direccion}");
            sb.AppendLine($"Teléfono: {factura.Cliente.Telefono}");
            sb.AppendLine();

            // Detalle de productos
            sb.AppendLine("DETALLE DE PRODUCTOS:");
            sb.AppendLine();
            sb.AppendLine("".PadRight(70, '-'));
            sb.AppendLine("Descripción".PadRight(30) + "Cant.".PadLeft(8) + "P.Unit".PadLeft(12) + "Subtotal".PadLeft(15));
            sb.AppendLine("".PadRight(70, '-'));

            foreach (var detalle in factura.ListaVentaDetalle)
            {
                sb.AppendLine(
                    detalle.producto.NombreComercial.PadRight(30) +
                    detalle.Cantidad.ToString().PadLeft(8) +
                    detalle.PrecioVenta.ToString("0.00").PadLeft(12) +
                    detalle.Subtotal.ToString("0.00").PadLeft(15)
                );
            }

            sb.AppendLine("".PadRight(70, '-'));
            sb.AppendLine();

            // Totales
            double subtotal = factura.ListaVentaDetalle.Sum(d => d.Subtotal);
            double iva = subtotal * 0.12;
            double total = subtotal + iva;

            sb.AppendLine($"SUBTOTAL: {subtotal.ToString("0.00").PadLeft(55)}");
            sb.AppendLine($"IVA (12%): {iva.ToString("0.00").PadLeft(55)}");
            sb.AppendLine($"TOTAL: {total.ToString("0.00").PadLeft(55)}");
            sb.AppendLine();
            sb.AppendLine("GRACIAS POR SU COMPRA".PadLeft(50));

            // Mostrar en el RichTextBox
            rtb_Factura.Text = sb.ToString();
        }

        private void button_Buscar_Click(object sender, EventArgs e)
        {
            string codigoFactura = textBox_Factura.Text.Trim();

            if (string.IsNullOrEmpty(codigoFactura))
            {
                MessageBox.Show("Por favor ingrese un código de factura", "Advertencia",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Buscar la factura por código
                 var FacturaEncontrada = ObtenerFacturaCompleta(codigoFactura);

                if (FacturaEncontrada == null)
                {
                    MessageBox.Show($"No se encontró factura con código: {codigoFactura}", "Error",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Mostrar confirmación
                DialogResult resultado = MessageBox.Show($"¿Factura encontrada! ¿Desea generar esta factura?",
                                                       "Confirmación",
                                                       MessageBoxButtons.YesNo,
                                                       MessageBoxIcon.Question);

                if (resultado == DialogResult.Yes)
                {
                    MostrarFacturaEnRichTextBox(FacturaEncontrada); 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar factura: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_Guardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(rtb_Factura.Text))
            {
                MessageBox.Show("No hay contenido para guardar", "Advertencia",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Archivos PDF (*.pdf)|*.pdf";
            saveFileDialog.Title = "Guardar factura como PDF";
            saveFileDialog.FileName = $"Factura_{DateTime.Now:yyyyMMddHHmmss}.pdf";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Configuración del documento PDF
                    Document document = new Document(PageSize.A4, 20, 20, 30, 30);
                    PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(saveFileDialog.FileName, FileMode.Create));

                    document.Open();

                    // Configurar fuente (Courier New para mantener el formato)
                    BaseFont bf = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                    iTextSharp.text.Font font = new iTextSharp.text.Font(bf, 10);

                    // Dividir el texto por líneas
                    string[] lines = rtb_Factura.Text.Split(new[] { "\n" }, StringSplitOptions.None);

                    // Agregar contenido al PDF
                    foreach (string line in lines)
                    {
                        if (!string.IsNullOrEmpty(line.Trim()))
                        {
                            document.Add(new Paragraph(line, font));
                        }
                    }

                    document.Close();

                    MessageBox.Show("Factura guardada como PDF exitosamente", "Éxito",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al guardar PDF: {ex.Message}", "Error",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox_Apellido_TextChanged(object sender, EventArgs e)
        {
           // FiltrarPorApellido();
        }
        
    }
}
