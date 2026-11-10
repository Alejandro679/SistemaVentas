using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace SistemaVentas.Conexion
{
    public class ConexionBD
    {
        private static readonly string cadena =
             @"Data Source=(localdb)\MSSQLLocalDB;
              Database=SistemaVentas;
              Integrated Security=True;
              TrustServerCertificate=True;";
        public static SqlConnection ObtenerConexion()
        {
            return new SqlConnection(cadena);
        }

        // METODO CORREGIDO
        public static bool ProbarConexion(out string mensaje)
        {
            try
            {
                using (SqlConnection conexion = ObtenerConexion())
                {
                    conexion.Open();

                    mensaje = "Conexión exitosa con SQL Server.";

                    return true;
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error de conexión: " + ex.Message;

                return false;
            }
        }
    }
}
