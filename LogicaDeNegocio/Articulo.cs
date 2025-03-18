using AccesoDatos;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaDeNegocio
{
    public class Articulo
    {
        public int IdArticulo { get; set; }
        public string Descripcion { get; set; } // Descripción del artículo
        public string Marca { get; set; } // Marca del artículo
        public decimal PrecioUnitarioSinIVA { get; set; } // Precio unitario del artículo
        public int CantidadEnStock { get; set; } // Cantidad disponible en el inventario
        public decimal Alicuota { get; set; }

        public List<Articulo> ObtenerDatosArticulos()

        {

            List<Articulo> listaArticulos = new List<Articulo>();

            if (ConexionBD.EstablecerConexion())

            {

                try

                {

                    string ObtenerDatos = @"

                SELECT A.IdArticulo, A.Descripcion, M.Descripcion AS Marca, A.PrecioUnitarioSinIVA, A.CantidadEnStock, AL.Porcentaje AS Alicuota

                FROM Articulo A
                INNER JOIN Marca M ON A.IdMarca = M.IdMarca
                INNER JOIN Alicuota AL ON A.IdAlicuota = AL.IdAlicuota

            ";



                    // Ejecutamos el comando

                    using (SqlCommand comando = new SqlCommand(ObtenerDatos, ConexionBD.miConexion))

                    {

                        using (SqlDataReader lector = comando.ExecuteReader())

                        {

                            // Recorremos los resultados de la consulta

                            while (lector.Read())

                            {

                                // Crear una nueva instancia de Articulos

                                Articulo articulo1 = new Articulo();



                                // Definir las propiedades del objeto articulo1

                                articulo1.IdArticulo = (int)lector["IdArticulo"];

                                articulo1.Descripcion = (string)lector["Descripcion"];

                                articulo1.Marca = (string)lector["Marca"];

                                articulo1.PrecioUnitarioSinIVA = (decimal)lector["PrecioUnitarioSinIVA"];

                                articulo1.CantidadEnStock = (int)lector["CantidadEnStock"];

                                articulo1.Alicuota = (decimal)lector["Alicuota"];



                                // Agregar el artículo a la lista

                                listaArticulos.Add(articulo1);

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



            // Retornar la lista de artículos

            return listaArticulos;

        }
        public decimal CalcularPrecioUnitarioConIVA(Articulo articulo1)
        {
            decimal resultado = articulo1.PrecioUnitarioSinIVA +
                                (articulo1.PrecioUnitarioSinIVA * articulo1.Alicuota / 100);
            return Math.Round(resultado, 2);
        }




    }

}
