using Farmacia_Entidades;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Farmacia_Datos_SQLServer
{
    public static class Producto_Datos
    {
        private static string connectionString = "Server=localhost\\SQLEXPRESS;Database=GestionFarmacia;Trusted_Connection=True;";

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
        public static void DescontarStock(int idProducto, int cantidad)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = "UPDATE Productos SET Stock = Stock - @Cantidad WHERE Id = @Id;";
                using (var cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@Cantidad", cantidad);
                    cmd.Parameters.AddWithValue("@Id", idProducto);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}

