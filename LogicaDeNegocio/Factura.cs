using AccesoDatos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReporteFactura;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.ReportAppServer;



namespace LogicaDeNegocio
{

    
    public class Factura
    {
        public int IdFactura { get; set; }
        public int NroFactura { get; set; } // Número de la factura
        public char Tipo { get; set; } // Tipo de factura (A o B)
        public CondicionVenta CondicionVenta { get; set; } = new CondicionVenta(); // Relación con la condición de venta
        public DateTime Fecha { get; set; } // Fecha de la factura
        public Cliente Cliente { get; set; } = new Cliente(); // Cliente que emitió la factura
        public decimal? Subtotal { get; set; } // Subtotal de la factura
        public decimal? IVA { get; set; } // Valor del IVA
        public decimal TotalNeto { get; set; } // Total neto (Subtotal + IVA)

      


      

        public decimal CalcularSumaPreciosTotales(DataTable tablaArticulos)
        {
            decimal sumaTotal = 0;

            // Recorre todas las filas de la tabla
            foreach (DataRow row in tablaArticulos.Rows)
            {
                // Suma el valor de la columna "Precio Total"
                sumaTotal += (decimal)row["Precio Total ($)"];
            }

            return sumaTotal;
        }

        public decimal CalcularIVAInscripto(DataTable tablaArticulos)
        {
            decimal totalIVA = 0;

            // Agrupar las filas por el valor de la columna "Alicuota", verificando que sea decimal
            var gruposPorAlicuota = tablaArticulos.AsEnumerable()
            .Where(fila => decimal.TryParse(fila.Field<object>("Alícuota").ToString(), out decimal alicuota)) // Verificar que sea decimal
            .GroupBy(fila => Convert.ToDecimal(fila.Field<object>("Alícuota")));

            // Recorrer cada grupo (cada valor de alícuota distinto)
            foreach (var grupo in gruposPorAlicuota)
            {
                decimal alicuota = grupo.Key;

                // Sumar los valores de la columna "Precio Total" de las filas con la misma alícuota
                decimal sumaPreciosTotales = grupo.Sum(fila => fila.Field<decimal>("Precio Total ($)"));

                // Calcular el IVA para esa suma (solo el IVA, no el total con IVA)
                decimal iva = sumaPreciosTotales * alicuota / 100;

                // Sumar el IVA al total
                totalIVA += iva;
            }

            return totalIVA;
        }

        public decimal CalcularTotalNeto(decimal subtotal, decimal IVA)
        {
            decimal resultado = subtotal + IVA;


            // Retornar la suma de Subtotal e IVA
            return resultado;
        }

       
        public List<Factura> ObtenerFacturasPorCliente(Cliente cliente1)
        {
            List<Factura> listaFacturas = new List<Factura>();

            // Verificamos si la conexión fue establecida
            if (ConexionBD.EstablecerConexion())
            {
                try
                {
                    // Consulta SQL dependiendo de la condición IVA
                    string query;

                    if (cliente1.CondicionIVA.Descripcion == "Responsable Inscripto")
                    {
                        query = @"
            SELECT 
                f.NroFactura, f.Tipo, 
                cv.Descripcion AS CondicionVenta, 
                f.Fecha, f.Subtotal, f.IVA, f.TotalNeto
            FROM Factura f
            INNER JOIN CondicionVenta cv ON f.IdCondicionVenta = cv.IdCondicionVenta
            WHERE f.IdCliente = @IdCliente";
                    }
                    else
                    {
                        query = @"
            SELECT 
                f.NroFactura, f.Tipo, 
                cv.Descripcion AS CondicionVenta, 
                f.Fecha, f.TotalNeto
            FROM Factura f
            INNER JOIN CondicionVenta cv ON f.IdCondicionVenta = cv.IdCondicionVenta
            WHERE f.IdCliente = @IdCliente";
                    }

                    // Ejecutamos el comando SQL
                    using (SqlCommand comando = new SqlCommand(query, ConexionBD.miConexion))
                    {
                        comando.Parameters.AddWithValue("@IdCliente", cliente1.IdCliente);

                        using (SqlDataReader lector = comando.ExecuteReader())
                        {
                            // Recorremos los resultados y construimos la lista de facturas
                            while (lector.Read())
                            {
                                // Crear instancia de Factura
                                Factura factura = new Factura();

                                // Asignamos los valores a las propiedades de la factura
                                factura.NroFactura = (int)lector["NroFactura"];

                                // En C#, el método lector["Tipo"] devuelve un valor de tipo object. 
                                // Aunque el valor real es un carácter, el compilador de C# necesita que indiques explícitamente 
                                // cómo convertir ese objeto al tipo esperado, ya que no puede asumir el tipo por sí mismo sin una conversión explícita.

                                // En otros casos, como int o string, la conversión es más directa:

                                // Enteros (int): Si el valor en la base de datos es numérico, C# puede hacer un boxing/unboxing automático entre object y int.
                                // Cadenas (string): Como una cadena es una referencia y ya es compatible con object, no requiere conversión explícita.
                                factura.Tipo = Convert.ToChar(lector["Tipo"]);
                                factura.CondicionVenta.Descripcion = (string)lector["CondicionVenta"];
                                factura.Fecha = (DateTime)lector["Fecha"];
                                factura.TotalNeto = (decimal)lector["TotalNeto"];

                                // Asignamos Subtotal e IVA solo si aplica (Responsable Inscripto)
                                if (cliente1.CondicionIVA.Descripcion == "Responsable Inscripto")
                                {
                                    factura.Subtotal = lector["Subtotal"] != DBNull.Value ? (decimal)lector["Subtotal"] : (decimal?)null;
                                    factura.IVA = lector["IVA"] != DBNull.Value ? (decimal)lector["IVA"] : (decimal?)null;
                                }

                                // Agregar la factura a la lista
                                listaFacturas.Add(factura);
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

            // Retornamos la lista de facturas del cliente
            return listaFacturas;
        }


        public SqlCommand InserccionFacturaYDetalle(Factura factura1, DataTable tablaArticulos)
        {
            SqlCommand comando = new SqlCommand();
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(@"
    INSERT INTO Factura (Tipo, NroFactura, IdCondicionVenta, Fecha, IdCliente, Subtotal, IVA, TotalNeto)
    VALUES (@Tipo, 0, @IdCondicionVenta, @Fecha, @IdCliente, @Subtotal, @IVA, @TotalNeto);
    DECLARE @IdFactura INT = SCOPE_IDENTITY();");

            comando.Parameters.AddWithValue("@Tipo", factura1.Tipo);
            comando.Parameters.AddWithValue("@IdCondicionVenta", factura1.CondicionVenta.IdCondicionVenta);
            comando.Parameters.AddWithValue("@Fecha", factura1.Fecha);
            comando.Parameters.AddWithValue("@IdCliente", factura1.Cliente.IdCliente);
            comando.Parameters.AddWithValue("@Subtotal", factura1.Subtotal ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@IVA", factura1.IVA ?? (object)DBNull.Value);
            comando.Parameters.AddWithValue("@TotalNeto", factura1.TotalNeto);

            int counter = 0;

            /*Cuando intentamos ejecutar una consulta SQL que tiene parámetros (como @IdArticulo, @Cantidad, etc.), el objeto SqlCommand espera
            que cada parámetro tenga un nombre único. Esto es necesario porque, al crear varios parámetros con el mismo nombre en un
            mismo comando, el SqlCommand no sabría cuál utilizar en cada lugar, generando un conflicto.

            Al utilizar un bucle foreach para recorrer cada artículo en la tabla tablaArticulos y al agregar parámetros con los
            nombres(@IdArticulo, @Cantidad, @PrecioUnitario, @PrecioTotal) se da la situación que hace que el SqlCommand intente agregar
            múltiples valores a los mismos parámetros, lo que resulta en un error porque SqlCommand solo permite un valor por nombre de 
            parámetro dentro de un mismo comando,

            Para solucionar esto, la idea es asegurarse de que cada parámetro en el SqlCommand tenga un nombre único.
            Esto se logra al usar un contador(counter) que agrega un sufijo único a cada nombre de parámetro en cada iteración del bucle.
            Así, en lugar de tener varios parámetros llamados @IdArticulo, tendremos @IdArticulo0, @IdArticulo1, @IdArticulo2, etc.,
            según el valor de counter en cada iteración.*/

            foreach (DataRow row in tablaArticulos.Rows)
            {
                sb.AppendLine($@"
        INSERT INTO FacturaDetalle (IdFactura, IdArticulo, Cantidad, PrecioUnitario, PrecioTotal)
        VALUES (@IdFactura, @IdArticulo{counter}, @Cantidad{counter}, @PrecioUnitario{counter}, @PrecioTotal{counter});");

                comando.Parameters.AddWithValue($"@IdArticulo{counter}", row["IdArtículo"]);
                comando.Parameters.AddWithValue($"@Cantidad{counter}", row["Cantidad"]);
                comando.Parameters.AddWithValue($"@PrecioUnitario{counter}", row["Precio Unitario ($)"]);
                comando.Parameters.AddWithValue($"@PrecioTotal{counter}", row["Precio Total ($)"]);

                counter++;
            }

            comando.CommandText = sb.ToString();

            return comando;
        }


      
        public void ImprimirFacturaSeleccionada(int NroFactura, char tipoFactura)

        {


            // Crear una instancia del DataSet específico
            var dsFactura = new dsFactura();

            // Crear una instancia del TableAdapter
            var adapter = new ReporteFactura.dsFacturaTableAdapters.DataTable1TableAdapter();

            dsFactura.EnforceConstraints = false;

            // Llenar el DataSet usando el TableAdapter y el número de factura
            adapter.FillFactura(dsFactura.DataTable1, NroFactura);

            // Cargar el reporte correspondiente usando el número de factura
            ReportDocument report = DeterminarReportePorTipoFactura(Tipo);

            // Asignar el DataSet al reporte
            report.SetDataSource(dsFactura);

            // Crear y mostrar el formulario de vista de reporte
            FormularioReporte vistaFactura = new FormularioReporte();
            vistaFactura.CargarReporte(report);
            vistaFactura.ShowDialog();

        }


        // Método para determinar el tipo de reporte 

        private ReportDocument DeterminarReportePorTipoFactura(char tipoFactura)

        {

            ReportDocument report = new ReportDocument();

            if (tipoFactura == 'A')

            {
                report.Load(@"C:\Users\54349\Desktop\TRABAJO FINAL\ReporteFactura\Reportes\FacturaA.rpt");
            }

            else if (tipoFactura == 'B')

            {
                report.Load(@"C:\Users\54349\Desktop\TRABAJO FINAL\ReporteFactura\Reportes\FacturaB.rpt");
            }

            return report;

        }


        // Método para imprimir la última factura insertada

        public void ImprimirUltimaFactura()

        {

            // Crear una instancia del DataSet específico
            var dsFactura = new dsFactura();

            // Crear una instancia del TableAdapter
            var adapter = new ReporteFactura.dsFacturaTableAdapters.DataTable1TableAdapter();

            dsFactura.EnforceConstraints = false;

            // Llenar el DataSet sin pasar un NroFactura para obtener la última factura
            adapter.FillFactura(dsFactura.DataTable1,-1);

            // Obtener el tipo de factura del DataSet como char
            char tipoFactura = Convert.ToChar(dsFactura.DataTable1.Rows[0]["Tipo"]);


            // Determinar el tipo de reporte a usar (reporte A o reporte B) basado en el tipo de factura

            ReportDocument report = DeterminarReportePorTipoFactura(tipoFactura);



            // Asignar el DataSet al reporte

            report.SetDataSource(dsFactura);



            // Crear y mostrar el formulario de vista de reporte

            FormularioReporte vistaFactura = new FormularioReporte();

            vistaFactura.CargarReporte(report);

            vistaFactura.ShowDialog();

        }

    }

}
