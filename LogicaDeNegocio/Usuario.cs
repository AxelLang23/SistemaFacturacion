using AccesoDatos;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaDeNegocio
{
    public class Usuario
    {
        public int IdUsuario { get; set; } // Identificador único del usuario
        public string NombreUsuario { get; set; } // Nombre del usuario
        public string Contraseña { get; set; } // Contraseña del usuario

        // Método para obtener todos los nombres de usuario desde la base de datos
        public static List<string> ObtenerUsuarios()
        {
            List<string> listaUsuarios = new List<string>();

            // Intentar establecer la conexión
            if (ConexionBD.EstablecerConexion())
            {
                string query = "SELECT NombreUsuario FROM Usuario";

                try
                {
                    // Usar using para asegurar que se cierre la conexión automáticamente
                    using (SqlCommand comando = new SqlCommand(query, ConexionBD.miConexion))
                    {
                        using (SqlDataReader lector = comando.ExecuteReader())
                        {
                            while (lector.Read())
                            {
                                listaUsuarios.Add(lector["NombreUsuario"].ToString());
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    // Capturar errores relacionados con SQL
                    Console.WriteLine("Error en la consulta SQL: " + ex.Message);
                }
            }
            else
            {
                // Si no se pudo abrir la conexión, mostrar un mensaje
                Console.WriteLine("No se pudo establecer la conexión a la base de datos.");
            }

            return listaUsuarios;
        }


        // Método para validar si un usuario y contraseña son correctos
        public static bool ValidarUsuario(Usuario usuario1)
        {
            bool esValido = false;

            if (ConexionBD.EstablecerConexion())
            {
                try
                {
                    using (SqlCommand comando = new SqlCommand("SELECT COUNT(1) FROM Usuario WHERE NombreUsuario = @NombreUsuario AND Contraseña = @Contraseña", ConexionBD.miConexion))
                    {
                        comando.Parameters.AddWithValue("@NombreUsuario", usuario1.NombreUsuario);
                        comando.Parameters.AddWithValue("@Contraseña", usuario1.Contraseña);

                        int resultado = (int)comando.ExecuteScalar();
                        esValido = resultado == 1;
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine("Error de SQL: " + ex.Message);
                }
            }
            else
            {
                Console.WriteLine("No se pudo establecer la conexión.");
            }

            return esValido;
        }

    }

}
