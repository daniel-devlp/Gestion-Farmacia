using Farmacia_Entidades;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Farmacia_Datos_SQLServer
{
    public static class Producto_Datos
    {
        private static string connectionString = "Data Source=Daniel\\PUNTO_A;Initial Catalog=GestionFarmacia;Persist Security Info=True;User ID=sa;Password=admin123;TrustServerCertificate=True;";

        public static List<Producto_Entidades> DevolverListadoProductos()
        {
            try
            {
                List<Producto_Entidades> lista = new List<Producto_Entidades>();

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = @"SELECT Id, NombreComercial, NombreGenerico, Presentacion, Precio, Stock 
                                   FROM Productos
                                    where stock >0
                                   ORDER BY Id ASC;";

                    using (var cmd = new SqlCommand(sql, connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int id = reader.GetInt32(0);
                                string nombreComercial = reader.GetString(1);
                                string nombreGenerico = reader.GetString(2);
                                string presentacion = reader.GetString(3);
                                double precio = Convert.ToDouble(reader.GetDecimal(4)); // Asegura conversión de decimal a double
                                int stock = reader.GetInt32(5);

                                Producto_Entidades producto = new Producto_Entidades(id, nombreComercial, nombreGenerico, presentacion, precio, stock);
                                lista.Add(producto);
                            }
                        }
                    }
                }

                return lista;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener productos: " + ex.Message);
                throw;
            }
        }
        public static Producto_Entidades ObtenerProductoPorId(int id)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = @"SELECT Id, NombreComercial, NombreGenerico, Presentacion, Precio, Stock 
                             FROM Productos
                             WHERE Id = @Id";

                    using (var cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Producto_Entidades(
                                    reader.GetInt32(0),
                                    reader.GetString(1),
                                    reader.GetString(2),
                                    reader.GetString(3),
                                    Convert.ToDouble(reader.GetDecimal(4)),
                                    reader.GetInt32(5)
                                );
                            }
                            return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener producto: {ex.Message}");
            }
        }
        public static void ActualizarStock(int idProducto, int cantidadCambio)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Verificar stock actual si estamos descontando
                        if (cantidadCambio < 0)
                        {
                            string sqlVerificar = "SELECT Stock FROM Productos WHERE Id = @Id";
                            int stockActual;

                            using (var cmd = new SqlCommand(sqlVerificar, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@Id", idProducto);
                                stockActual = Convert.ToInt32(cmd.ExecuteScalar());

                                if (stockActual < Math.Abs(cantidadCambio))
                                {
                                    throw new Exception($"Stock insuficiente. Disponible: {stockActual}, Se intentó descontar: {Math.Abs(cantidadCambio)}");
                                }
                            }
                        }

                        // Actualizar stock
                        string sqlActualizar = "UPDATE Productos SET Stock = Stock + @Cambio WHERE Id = @Id";
                        using (var cmd = new SqlCommand(sqlActualizar, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@Cambio", cantidadCambio);
                            cmd.Parameters.AddWithValue("@Id", idProducto);

                            int affectedRows = cmd.ExecuteNonQuery();
                            if (affectedRows == 0)
                            {
                                throw new Exception($"No se encontró el producto con ID: {idProducto}");
                            }
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        public static void DescontarStock(int idProducto, int cantidad)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Usar transacción para asegurar integridad
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Primero verificar stock actual
                        string sqlVerificar = "SELECT Stock FROM Productos WHERE Id = @Id";
                        int stockActual;

                        using (var cmd = new SqlCommand(sqlVerificar, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@Id", idProducto);
                            stockActual = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        if (stockActual < cantidad)
                        {
                            throw new Exception($"Stock insuficiente. Disponible: {stockActual}");
                        }

                        // Actualizar stock
                        string sqlActualizar = "UPDATE Productos SET Stock = Stock - @Cantidad WHERE Id = @Id";
                        using (var cmd = new SqlCommand(sqlActualizar, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@Cantidad", cantidad);
                            cmd.Parameters.AddWithValue("@Id", idProducto);
                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}

