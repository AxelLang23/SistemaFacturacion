using AccesoDatos;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaDeNegocio
{
    public class CondicionVenta
    {
        public int IdCondicionVenta { get; set; } // Descripción de la condición de venta (ej: Contado, Tarjeta de Crédito)
        public string Descripcion { get; set; } // Descripción de la condición de venta (ej: Contado, Tarjeta de Crédito)

       

        public List<CondicionVenta> ObtenerCondicionesVenta()

        {

            List<CondicionVenta> listaCondicionVenta = new List<CondicionVenta>();

            if (ConexionBD.EstablecerConexion())

            {

                try

                {

                    string ObtenerDatos = @"SELECT IdCondicionVenta, Descripcion FROM CondicionVenta";



                    // Ejecutamos el comando

                    using (SqlCommand comando = new SqlCommand(ObtenerDatos, ConexionBD.miConexion))

                    {

                        using (SqlDataReader lector = comando.ExecuteReader())

                        {

                            // Recorremos los resultados de la consulta

                            while (lector.Read())

                            {

                                // Crear una nueva instancia de CondicionVenta

                                CondicionVenta condicionVenta1 = new CondicionVenta();

                                condicionVenta1.IdCondicionVenta = (int)lector["IdCondicionVenta"];

                                condicionVenta1.Descripcion = (string)lector["Descripcion"];



                                // Agregar la CondicionVenta a la lista

                                listaCondicionVenta.Add(condicionVenta1);

                            }

                        }

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



            // Retornar la lista de condiciones de venta

            return listaCondicionVenta;

        }
    }

}
