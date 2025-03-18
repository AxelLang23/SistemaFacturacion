using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaDeNegocio
{
    public class FacturaDetalle
    {
        public Factura Factura { get; set; } = new Factura();
        public Articulo Articulo { get; set; } = new Articulo();
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal PrecioTotal { get; set; }

        // Método para calcular el precio total
        public decimal CalcularPrecioTotal(FacturaDetalle facturaDetalle1, Factura factura1)
        {
            decimal precioUnitario = 0; 

            // Verificar el tipo de la factura y asignar el precio unitario correspondiente
            if (factura1.Tipo == 'A')
            {
                precioUnitario = facturaDetalle1.Articulo.PrecioUnitarioSinIVA;
            }
            else if (factura1.Tipo == 'B')
            {
                precioUnitario = facturaDetalle1.PrecioUnitario;
            }

            // Calcular y devolver el precio total
            return Math.Round(precioUnitario * facturaDetalle1.Cantidad, 2);
        }


    }

}
