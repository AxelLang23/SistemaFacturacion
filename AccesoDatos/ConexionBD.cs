using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;


namespace AccesoDatos
{
    public static class ConexionBD
    {
        //public Accesible desde cualquier proyecto
        
        // "internal" indica que este miembro (la variable 'miConexion') será accesible 
        // solo dentro del mismo ensamblado (es decir, dentro del mismo proyecto compilado).
        // Otros proyectos que referencien este ensamblado no podrán acceder directamente a 'miConexion'.

        // "static" significa que 'miConexion' es un miembro estático de la clase. 
        // Al ser estática, pertenece a la clase 'ConexionBD' en su totalidad y no a una instancia particular de la clase.
        // Esto permite que se comparta la misma conexión de base de datos entre todas las instancias
        // de la clase, o incluso cuando no existe ninguna instancia de la clase (se accede directamente desde la clase).

        // "SqlConnection" es el tipo de datos de 'miConexion', que representa una conexión a una base de datos SQL Server.
        // 'SqlConnection' es una clase del espacio de nombres 'System.Data.SqlClient', utilizada para gestionar conexiones 
        // a bases de datos Microsoft SQL Server.

        // "miConexion" es el nombre de la variable que almacenará la instancia de 'SqlConnection'.
        // Se usa para mantener una referencia a la conexión de base de datos activa.

        // "= null" inicializa la variable 'miConexion' con un valor nulo.
        // Esto significa que, al inicio, no existe ninguna conexión abierta hasta que se llame a un método que la configure.
        // Asignar 'null' aquí es importante porque indica explícitamente que no hay conexión activa inicialmente,
        // y se verificará antes de intentar operar con la base de datos para evitar errores de referencia nula.
        

        public static SqlConnection miConexion = null;

        // Método estático para establecer una conexión con la base de datos.
        // Retorna un valor booleano: true si la conexión se establece exitosamente,
        // false si ocurre algún error durante el proceso de conexión.
        public static bool EstablecerConexion()
        {
            try
            {
                // Definición de la cadena de conexión que contiene los parámetros necesarios
                // para establecer la conexión con SQL Server. 
                // Server: Nombre del servidor y la instancia de SQL Server.
                // Database: Nombre de la base de datos objetivo.
                // Integrated Security=True: Utiliza la autenticación integrada de Windows.
                string strCadenaConexion = "Server=AXEL\\SQLEXPRESS;Database=SistemaFacturacion;Integrated Security=True;";

                // Inicialización de la instancia de SqlConnection con la cadena de conexión proporcionada.
                // La instancia 'miConexion' es responsable de manejar la conexión física con la base de datos.
                miConexion = new SqlConnection(strCadenaConexion);

                // Se invoca el método Open() para establecer la conexión con la base de datos.
                // Este método inicia una conexión física con el servidor SQL, utilizando la cadena de conexión
                // especificada. Si la conexión es exitosa, no se lanza ninguna excepción.
                miConexion.Open();

                // Si no se lanza ninguna excepción, se devuelve 'true', indicando que la conexión se estableció correctamente.
                return true;
            }
            catch
            {
                // En caso de que ocurra cualquier excepción durante el proceso de apertura de la conexión
                // (por ejemplo, si el servidor no es accesible, la cadena de conexión es incorrecta, etc.),
                // se captura la excepción y se retorna 'false', indicando que la conexión falló.
                return false;
            }
        }

        
        public static void EjecutarInserccion(SqlCommand comando)
        {
            if (EstablecerConexion()) // Intentar establecer conexión
            {
                using (SqlTransaction transaction = miConexion.BeginTransaction())
                {
                    try
                    {
                        // Asignar la conexión y la transacción al comando
                        comando.Connection = miConexion;
                        comando.Transaction = transaction;

                        // Ejecutar el comando
                        comando.ExecuteNonQuery();

                        // Confirmar la transacción
                        transaction.Commit();
                        Console.WriteLine("Transacción realizada con éxito.");
                    }
                    catch (Exception ex)
                    {
                        // Revertir la transacción en caso de error
                        transaction.Rollback();
                        Console.WriteLine("Error en la transacción: " + ex.Message);
                    }
                }
            }
            else
            {
                // Si no se pudo abrir la conexión, mostrar un mensaje
                Console.WriteLine("No se pudo establecer la conexión a la base de datos.");
            }
        }



    }

}

