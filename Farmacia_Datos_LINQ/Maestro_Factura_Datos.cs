using Farmacia_Entidades;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Farmacia_Datos_LINQ
{
    public static class Maestro_Factura_Datos
    {
        private static string connString = "Data Source=Daniel\\PUNTO_A;Initial Catalog=GestionFarmacia;Persist Security Info=True;User ID=sa;Password=admin123;TrustServerCertificate=True;";

        public static int GuardarVentaCabecera(VentaCabecera_Entidades venta)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Validación de datos requeridos
                        if (venta.Cliente == null || venta.Cliente.Id == 0)
                            throw new ArgumentException("Debe especificar un cliente válido");

                        // Generar número de comprobante si no existe
                        if (string.IsNullOrEmpty(venta.NumeroComprobante))
                        {
                            venta.NumeroComprobante = GenerarNumeroComprobante();
                        }

                        // Establecer fecha si no está definida
                        if (venta.FechaComprobante == DateTime.MinValue)
                        {
                            venta.FechaComprobante = DateTime.Now;
                        }

                        string query = @"INSERT INTO VentaCabecera 
                                       (id_cliente, FechaComprobante, NumeroComprobante)
                                       VALUES (@id_cliente, @fechaComprobante, @numeroComprobante);
                                       SELECT SCOPE_IDENTITY();";

                        using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@id_cliente", venta.Cliente.Id);
                            cmd.Parameters.AddWithValue("@fechaComprobante", venta.FechaComprobante);
                            cmd.Parameters.AddWithValue("@numeroComprobante", venta.NumeroComprobante);

                            int idVenta = Convert.ToInt32(cmd.ExecuteScalar());
                            transaction.Commit();
                            return idVenta;
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("Error al guardar cabecera de venta: " + ex.Message);
                    }
                }
            }
        }

        public static void GuardarVentaDetalle(int ventaId, List<VentaDetalle_Entidades> detalles)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (var detalle in detalles)
                        {
                            // Validar producto y cantidad
                            if (detalle.producto == null || detalle.producto.Id == 0)
                                throw new ArgumentException("Producto no válido en detalle");

                            if (detalle.Cantidad <= 0)
                                throw new ArgumentException("La cantidad debe ser mayor que cero");

                            // 1. Primero verificamos el stock disponible
                            string queryVerificarStock = @"SELECT Stock FROM Productos WHERE Id = @Id_Producto";
                            int stockActual;

                            using (SqlCommand cmd = new SqlCommand(queryVerificarStock, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@Id_Producto", detalle.producto.Id);
                                stockActual = Convert.ToInt32(cmd.ExecuteScalar());
                            }

                            if (stockActual < detalle.Cantidad)
                            {
                                //throw new Exception($"Stock insuficiente para el producto ID {detalle.producto.Id}. Disponible: {stockActual}, Requerido: {detalle.Cantidad}");
                            }

                            // 2. Insertar detalle de venta
                            string queryInsertDetalle = @"INSERT INTO VentaDetalle 
                                                (Id_Venta, Id_Producto, Cantidad, PrecioVenta, Subtotal, Iva)
                                                VALUES (@Id_Venta, @Id_Producto, @Cantidad, @PrecioVenta, @Subtotal, @Iva)";

                            using (SqlCommand cmd = new SqlCommand(queryInsertDetalle, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@Id_Venta", ventaId);
                                cmd.Parameters.AddWithValue("@Id_Producto", detalle.producto.Id);
                                cmd.Parameters.AddWithValue("@Cantidad", detalle.Cantidad);
                                cmd.Parameters.AddWithValue("@PrecioVenta", detalle.PrecioVenta);
                                cmd.Parameters.AddWithValue("@Subtotal", detalle.Subtotal);
                                cmd.Parameters.AddWithValue("@Iva", detalle.Iva);
                                cmd.ExecuteNonQuery();
                            }

                            // 3. Actualizar el stock (descontar la cantidad vendida)
                            string queryActualizarStock = @"UPDATE Productos 
                                                 SET Stock = Stock - @Cantidad 
                                                 WHERE Id = @Id_Producto";

                            using (SqlCommand cmd = new SqlCommand(queryActualizarStock, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@Cantidad", detalle.Cantidad);
                                cmd.Parameters.AddWithValue("@Id_Producto", detalle.producto.Id);
                                int rowsAffected = cmd.ExecuteNonQuery();

                                if (rowsAffected == 0)
                                {
                                    throw new Exception($"No se pudo actualizar el stock para el producto ID {detalle.producto.Id}");
                                }
                            }
                        }
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("Error al guardar detalle de venta y actualizar stock: " + ex.Message);
                    }
                }
            }
        }
        

        private static string GenerarNumeroComprobante()
        {
            // Formato: FAC-AAAAMMDD-0001
            string fecha = DateTime.Now.ToString("yyyyMMdd");
            string ultimoNumero = ObtenerUltimoNumeroComprobante(fecha);
            int siguienteNumero = ultimoNumero == null ? 1 : int.Parse(ultimoNumero) + 1;
            return $"FAC-{fecha}-{siguienteNumero.ToString("0000")}";
        }

        private static string ObtenerUltimoNumeroComprobante(string fecha)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = @"SELECT MAX(SUBSTRING(NumeroComprobante, 13, 4))
                               FROM VentaCabecera
                               WHERE NumeroComprobante LIKE @patron";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@patron", $"FAC-{fecha}-%");
                    conn.Open();
                    return cmd.ExecuteScalar()?.ToString();
                }
            }
        }

        public static DateTime ObtenerFechaServidor()
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT GETDATE()", conn))
                {
                    return (DateTime)cmd.ExecuteScalar();
                }
            }
        }
    }
}