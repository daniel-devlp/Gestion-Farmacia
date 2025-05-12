using Farmacia_Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmacia_Datos_LINQ
{
    using Farmacia_Entidades;
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;

    namespace Farmacia_Datos_LINQ
    {
        public static class Detalle_Factura_Datos
        {
            private static string conn = "Data Source=Daniel\\PUNTO_A;Initial Catalog=GestionFarmacia;Persist Security Info=True;User ID=sa;Password=admin123;TrustServerCertificate=True;";

            public static void GuardarVentaDetalle(int ventaId, List<VentaDetalle_Entidades> detalles, SqlConnection conn, SqlTransaction transaction)
            {
                foreach (var detalle in detalles)
                {
                    if (detalle.producto.Stock < detalle.Cantidad)
                    {
                        throw new InvalidOperationException($"Stock insuficiente para {detalle.producto.NombreComercial}");
                    }

                    string queryDetalle = @"INSERT INTO VentaDetalle 
                    (Id_Venta, Id_Producto, PrecioVenta, Cantidad, Subtotal, Iva)
                    VALUES (@Id_Venta, @Id_Producto, @PrecioVenta, @Cantidad, @Subtotal, @Iva)";

                    using (SqlCommand cmd = new SqlCommand(queryDetalle, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@Id_Venta", ventaId);
                        cmd.Parameters.AddWithValue("@Id_Producto", detalle.producto.Id);
                        cmd.Parameters.AddWithValue("@PrecioVenta", detalle.PrecioVenta);
                        cmd.Parameters.AddWithValue("@Cantidad", detalle.Cantidad);
                        cmd.Parameters.AddWithValue("@Subtotal", detalle.Subtotal);
                        cmd.Parameters.AddWithValue("@Iva", detalle.Iva);
                        cmd.ExecuteNonQuery();
                    }

                    string queryStock = @"UPDATE Productos SET Stock = Stock - @Cantidad WHERE Id = @Id_Producto";
                    using (SqlCommand cmd = new SqlCommand(queryStock, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@Cantidad", detalle.Cantidad);
                        cmd.Parameters.AddWithValue("@Id_Producto", detalle.producto.Id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }

}
