using Farmacia_Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Farmacia_Datos_LINQ
{
    public static class Maestro_Factura_Datos
    {
        private static string connectionString = "Server=localhost\\SQLEXPRESS;Database=GestionFarmacia;Trusted_Connection=True;";

        public static int GuardarVentaCabecera(VentaCabecera_Entidades venta)
        {
            // Validaciones iniciales
            if (venta == null) throw new ArgumentNullException(nameof(venta));
            if (venta.Cliente == null) throw new ArgumentException("Cliente no puede ser nulo");
            if (venta.ListaVentaDetalle == null || !venta.ListaVentaDetalle.Any())
                throw new ArgumentException("Debe haber al menos un detalle de venta");

            int ventaId = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Calcular totales
                        double subtotalTotal = venta.ListaVentaDetalle.Sum(d => d.Subtotal);
                        double iva = subtotalTotal * 0.12;
                        double totalVenta = subtotalTotal + iva;

                        // Query para insertar cabecera
                        string queryCabecera = @"INSERT INTO VentaCabecera 
                                                (id_cliente, fechaVenta, totalVenta)
                                                VALUES (@id_cliente, @fechaVenta, @totalVenta);
                                                SELECT CAST(SCOPE_IDENTITY() AS INT);";

                        using (SqlCommand cmd = new SqlCommand(queryCabecera, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@id_cliente", venta.Cliente.Id);
                            cmd.Parameters.AddWithValue("@fechaVenta", venta.FechaComprobante);
                            cmd.Parameters.AddWithValue("@totalVenta", totalVenta);

                            ventaId = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        // Guardar detalles
                        GuardarVentaDetalle(ventaId, venta.ListaVentaDetalle, conn, transaction);

                        transaction.Commit();
                        return ventaId;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("Error al guardar la venta: " + ex.Message, ex);
                    }
                }
            }
        }

        public static void GuardarVentaDetalle(int ventaId, List<VentaDetalle_Entidades> detalles, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool ownConnection = false;

            try
            {
                // Si no nos pasaron conexión, creamos una nueva
                if (conn == null)
                {
                    conn = new SqlConnection(connectionString);
                    conn.Open();
                    ownConnection = true;
                    transaction = conn.BeginTransaction();
                }

                foreach (var detalle in detalles)
                {
                    // Validar stock disponible
                    if (detalle.producto.Stock < detalle.Cantidad)
                    {
                        throw new InvalidOperationException($"Stock insuficiente para el producto {detalle.producto.NombreComercial}");
                    }

                    // Calcular valores para el detalle
                    double detalleIva = detalle.Subtotal * 0.12;
                    double detalleTotal = detalle.Subtotal + detalleIva;

                    // Insertar detalle
                    string detalleQuery = @"INSERT INTO VentaDetalle 
                                            (Id_Venta, Id_Producto, PrecioVenta, Cantidad, Subtotal, Iva, Total)
                                            VALUES (@Id_Venta, @Id_Producto, @PrecioVenta, @Cantidad, @Subtotal, @Iva, @Total)";

                    using (SqlCommand cmd = new SqlCommand(detalleQuery, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@Id_Venta", ventaId);
                        cmd.Parameters.AddWithValue("@Id_Producto", detalle.producto.Id);
                        cmd.Parameters.AddWithValue("@PrecioVenta", detalle.PrecioVenta);
                        cmd.Parameters.AddWithValue("@Cantidad", detalle.Cantidad);
                        cmd.Parameters.AddWithValue("@Subtotal", detalle.Subtotal);
                        cmd.Parameters.AddWithValue("@Iva", detalleIva);
                        cmd.Parameters.AddWithValue("@Total", detalleTotal);
                        cmd.ExecuteNonQuery();
                    }

                    // Actualizar stock
                    string updateStock = @"UPDATE Productos SET stock = stock - @cantidad 
                                          WHERE id = @id_producto";

                    using (SqlCommand cmd = new SqlCommand(updateStock, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@id_producto", detalle.producto.Id);
                        cmd.Parameters.AddWithValue("@cantidad", detalle.Cantidad);
                        int affectedRows = cmd.ExecuteNonQuery();

                        if (affectedRows == 0)
                        {
                            throw new InvalidOperationException($"No se pudo actualizar el stock para el producto {detalle.producto.NombreComercial}");
                        }
                    }
                }

                if (ownConnection)
                {
                    transaction.Commit();
                }
            }
            catch
            {
                if (ownConnection && transaction != null)
                {
                    transaction.Rollback();
                }
                throw;
            }
            finally
            {
                if (ownConnection && conn != null)
                {
                    conn.Close();
                }
            }
        }

        public static int RegistrarVentaCompleta(VentaCabecera_Entidades venta)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Guardar cabecera
                        int ventaId = GuardarVentaCabecera(venta);

                        // Guardar detalles y actualizar stock
                        GuardarVentaDetalle(ventaId, venta.ListaVentaDetalle, conn, transaction);

                        transaction.Commit();
                        return ventaId;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("Error al registrar la venta completa: " + ex.Message, ex);
                    }
                }
            }
        }
    }
}