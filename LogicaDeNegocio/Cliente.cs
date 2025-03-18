using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccesoDatos;

namespace LogicaDeNegocio
{
    public class Cliente
    {
        public int IdCliente { get; set; } 
        public string NombreCompleto { get; set; } // Nombre completo del cliente (persona o empresa)
        public int? DNI { get; set; } // DNI del cliente (nullable)
        public string CUIT { get; set; } // CUIT del cliente (nullable). No ponemos el signo "?" porque la propiedad
                                         // es de tipo string y si no se asigna un valor explícito a esa propiedad,
                                         // su valor por defecto será null.

        // Relación con la clase CondicionIVA
        public CondicionIVA CondicionIVA { get; set; } 

        public string Direccion { get; set; } // Dirección del cliente
        public string Localidad { get; set; } // Localidad del cliente
        public string CodigoPostal { get; set; } // Código postal del cliente
        public string Provincia { get; set; } // Provincia del cliente
        public string Pais { get; set; } // País del cliente

        // Método para obtener los datos del cliente

        public List<Cliente> ObtenerDatosClientes()

        {

            List<Cliente> listaClientes = new List<Cliente>();

            if (ConexionBD.EstablecerConexion())

            {

                try

                {

                    // Consulta SQL que obtiene todos los datos solicitados

                    string ObtenerDatos = @"

                SELECT C.IdCliente, C.NombreCompleto, C.Direccion, C.CUIT, C.DNI, CI.Descripcion AS CondicionIVA,

                       L.Descripcion AS Localidad, CP.NroCodigoPostal,

                       P.Descripcion AS Provincia, PA.Descripcion AS Pais

                FROM Cliente C

                INNER JOIN CondicionIVA CI ON C.IdCondicionIVA = CI.IdCondicionIVA

                INNER JOIN Localidad L ON C.IdLocalidad = L.IdLocalidad

                INNER JOIN CodigoPostal CP ON L.IdCodigoPostal = CP.IdCodigoPostal

                INNER JOIN Provincia P ON L.IdProvincia = P.IdProvincia

                INNER JOIN Pais PA ON L.IdPais = PA.IdPais";



                    // Ejecutamos el comando

                    using (SqlCommand comando = new SqlCommand(ObtenerDatos, ConexionBD.miConexion))

                    {

                        using (SqlDataReader lector = comando.ExecuteReader())

                        {

                            // Recorremos los resultados de la consulta

                            while (lector.Read())

                            {

                                // Crear una nueva instancia de Cliente

                                Cliente cliente1 = new Cliente();
                                // Crear una nueva instancia de CondicionIVA para este cliente
                                cliente1.CondicionIVA = new CondicionIVA();
                                // Definir las propiedades del objeto cliente1

                                cliente1.IdCliente = (int)lector["IdCliente"];

                                cliente1.NombreCompleto = (string)lector["NombreCompleto"];

                                cliente1.Direccion = (string)lector["Direccion"];

                                cliente1.CondicionIVA.Descripcion = (string)lector["CondicionIVA"];

                                cliente1.Localidad = (string)lector["Localidad"];

                                cliente1.CodigoPostal = (string)lector["NroCodigoPostal"];

                                cliente1.Provincia = (string)lector["Provincia"];

                                cliente1.Pais = (string)lector["Pais"];


                                // Asignar DNI si está presente

                                if (lector["DNI"] != DBNull.Value)

                                {

                                    cliente1.DNI = (int)lector["DNI"];

                                }

                                else

                                {

                                    cliente1.DNI = null; // o algún valor por defecto

                                }



                                // Asignar CUIT si está presente

                                if (lector["CUIT"] != DBNull.Value)

                                {

                                    cliente1.CUIT = (string)lector["CUIT"];

                                }

                                else

                                {

                                    cliente1.CUIT = null;

                                }


                                // Agregar el cliente a la lista

                                listaClientes.Add(cliente1);

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



            // Retornar la lista de clientes

            return listaClientes;

        }

    }

}
