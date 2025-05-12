using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Farmacia_Entidades;
using System.Data.SqlClient;

namespace Farmacia_Datos_SQLServer
{
    public static class Cliente_Datos
    {
        private static string connectio nString = "Data Source=Daniel\\PUNTO_A;Initial Catalog=GestionFarmacia;Persist Security Info=True;User ID=sa;Password=admin123;TrustServerCertificate=True;";
            
        public static List<Cliente_Entidades> DevolverListadoClientes()
        {
            List<Cliente_Entidades> lista = new List<Cliente_Entidades>();
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM Clientes ORDER BY Id ASC;";
                using (var cmd = new SqlCommand(sql, connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Cliente_Entidades cliente = new Cliente_Entidades
                            {
                                Id = reader.GetInt32(0),
                                Apellidos = reader.GetString(1),
                                Nombres = reader.GetString(2),
                                Cedula = reader.GetString(3),
                                Direccion = reader.GetString(4),
                                Telefono = reader.GetString(5),
                                Correo = reader.GetString(6)
                            };
                            lista.Add(cliente);
                        }
                    }
                }
            }
            return lista;
        }

        public static Cliente_Entidades Nuevo(Cliente_Entidades cliente)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = @"INSERT INTO Clientes (Apellido, Nombre, Cedula, Direccion, Telefono, Correo)
                                   OUTPUT INSERTED.Id
                                   VALUES (@Apellido, @Nombre, @Cedula, @Direccion, @Telefono, @Correo);";
                    using (var cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@Apellido", cliente.Apellidos);
                        cmd.Parameters.AddWithValue("@Nombre", cliente.Nombres);
                        cmd.Parameters.AddWithValue("@Cedula", cliente.Cedula);
                        cmd.Parameters.AddWithValue("@Direccion", cliente.Direccion);
                        cmd.Parameters.AddWithValue("@Telefono", cliente.Telefono);
                        cmd.Parameters.AddWithValue("@Correo", cliente.Correo);
                        cliente.Id = (int)cmd.ExecuteScalar();
                    }
                }
                return cliente;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Cliente_Entidades Actualizar(Cliente_Entidades cliente)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = @"UPDATE Clientes
                                   SET Apellido = @Apellido,
                                       Nombre = @Nombre,
                                       Cedula = @Cedula,
                                       Direccion = @Direccion,
                                       Telefono = @Telefono,
                                       Correo = @Correo
                                   WHERE Id = @Id;";
                    using (var cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@Id", cliente.Id);
                        cmd.Parameters.AddWithValue("@Apellido", cliente.Apellidos);
                        cmd.Parameters.AddWithValue("@Nombre", cliente.Nombres);
                        cmd.Parameters.AddWithValue("@Cedula", cliente.Cedula);
                        cmd.Parameters.AddWithValue("@Direccion", cliente.Direccion);
                        cmd.Parameters.AddWithValue("@Telefono", cliente.Telefono);
                        cmd.Parameters.AddWithValue("@Correo", cliente.Correo);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            throw new Exception($"No se encontró el cliente con ID {cliente.Id}");
                        }
                    }
                }
                return cliente;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al actualizar cliente: " + ex.Message);
                throw;
            }
        }

        public static Cliente_Entidades DevolverClientePorId(int idCliente)
        {
            try
            {
                Cliente_Entidades cliente = new Cliente_Entidades();
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = @"SELECT Id, Apellido, Nombre, Cedula, Direccion, Telefono, Correo
                                   FROM Clientes
                                   WHERE Id = @Id;";
                    using (var cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@Id", idCliente);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                cliente.Id = reader.GetInt32(0);
                                cliente.Apellidos = reader.GetString(1);
                                cliente.Nombres = reader.GetString(2);
                                cliente.Cedula = reader.GetString(3);
                                cliente.Direccion = reader.GetString(4);
                                cliente.Telefono = reader.GetString(5);
                                cliente.Correo = reader.GetString(6);
                            }
                            else
                            {
                                throw new Exception($"No se encontró el cliente con Id {idCliente}");
                            }
                        }
                    }
                }
                return cliente;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

