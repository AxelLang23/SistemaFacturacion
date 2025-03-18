using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace LogicaDeNegocio
{
    public class UtilidadesFormulario
    {

        // Procedimiento para traer los datos a cualquier combobox
        //<T> indica que es un método genérico, lo que significa que acepta una lista de elementos de cualquier tipo 
        public static void CargarComboBox<T>(ComboBox comboBox, List<T> items, string displayMember, string itemInicial)  
        {
            // Limpiar el ComboBox
            comboBox.Items.Clear();

            // Agregar el primer ítem personalizado
            comboBox.Items.Add(itemInicial);

            // Configurar el DisplayMember si se usa con objetos complejos
            comboBox.DisplayMember = displayMember;

            // Cargar el resto de los elementos
            foreach (var item in items)
            {
                comboBox.Items.Add(item);
            }

            // Establecer el ítem inicial como seleccionado
            comboBox.SelectedIndex = 0;
        }


        public static DataTable CrearTablaArticulos()
        {
            // Crear una nueva instancia de DataTable
            DataTable tablaArticulos = new DataTable("Articulos");

            // Agregar columnas con sus respectivos tipos de datos
            tablaArticulos.Columns.Add("Descripción", typeof(string));
            tablaArticulos.Columns.Add("Marca", typeof(string));
            tablaArticulos.Columns.Add("Precio Unitario ($)", typeof(decimal));
            tablaArticulos.Columns.Add("Alícuota", typeof(int));
            tablaArticulos.Columns.Add("Cantidad", typeof(int));
            tablaArticulos.Columns.Add("Precio Total ($)", typeof(decimal));
            tablaArticulos.Columns.Add("IdArtículo", typeof(int));

            return tablaArticulos; // Devolver la tabla creada
        }

        public static void AgregarFilas(DataTable tablaArticulos, FacturaDetalle facturaDetalle1, Factura factura1)
        {
            // Crear una nueva fila en la tabla de artículos
            DataRow nuevaFila = tablaArticulos.NewRow();

            // Asignar valores a cada columna de la fila
            nuevaFila["IdArtículo"] = facturaDetalle1.Articulo.IdArticulo;
            nuevaFila["Descripción"] = facturaDetalle1.Articulo.Descripcion;
            nuevaFila["Marca"] = facturaDetalle1.Articulo.Marca;

            // Asignar el Precio Unitario basado en el tipo de factura
            if (factura1.Tipo == 'A')
            {
                nuevaFila["Precio Unitario ($)"] = facturaDetalle1.Articulo.PrecioUnitarioSinIVA;
            }
            else if (factura1.Tipo == 'B')
            {
                nuevaFila["Precio Unitario ($)"] = facturaDetalle1.PrecioUnitario;
            }

            nuevaFila["Alícuota"] = facturaDetalle1.Articulo.Alicuota;
            nuevaFila["Cantidad"] = facturaDetalle1.Cantidad;
            nuevaFila["Precio Total ($)"] = facturaDetalle1.PrecioTotal;

            // Agregar la fila a la tabla
            tablaArticulos.Rows.Add(nuevaFila);
        }

        public static DataTable CrearTablaFacturas()
        {
            // Crear una nueva instancia de DataTable
            DataTable tablaFacturas = new DataTable("Facturas");

            // Agregar columnas con sus respectivos tipos de datos
            tablaFacturas.Columns.Add("NroFactura", typeof(int));
            tablaFacturas.Columns.Add("Tipo", typeof(char));
            tablaFacturas.Columns.Add("CondicionVenta", typeof(string)); // Descripción desde CondicionVenta
            tablaFacturas.Columns.Add("Fecha", typeof(DateTime));
            tablaFacturas.Columns.Add("Subtotal", typeof(decimal)); // Solo si es Responsable Inscripto
            tablaFacturas.Columns.Add("IVA", typeof(decimal));      // Solo si es Responsable Inscripto
            tablaFacturas.Columns.Add("TotalNeto", typeof(decimal));

            return tablaFacturas; // Devolver la tabla creada
        }


        public static void AgregarFilasFacturas(DataTable tablaFacturas, List<Factura> listaFacturas)
        {
            foreach (Factura factura in listaFacturas)
            {
                // Crear una nueva fila en la tabla de facturas
                DataRow nuevaFila = tablaFacturas.NewRow();

                // Asignar valores a cada columna de la fila
                nuevaFila["NroFactura"] = factura.NroFactura;
                nuevaFila["Tipo"] = factura.Tipo;
                nuevaFila["CondicionVenta"] = factura.CondicionVenta.Descripcion;
                nuevaFila["Fecha"] = factura.Fecha;
                nuevaFila["TotalNeto"] = factura.TotalNeto;

                // Asignar Subtotal e IVA solo si existen
                if (factura.Subtotal.HasValue)
                {
                    nuevaFila["Subtotal"] = factura.Subtotal.Value;
                }

                if (factura.IVA.HasValue)
                {
                    nuevaFila["IVA"] = factura.IVA.Value;
                }

                // Agregar la fila al DataTable
                tablaFacturas.Rows.Add(nuevaFila);
            }
        }


        public static void LimpiarFormulario(
            Form frmFacturacion,
            DataTable tablaArticulos,
            DataGridView dtgDetallesVenta,
            Factura factura1,
            DataTable tablaFacturas,
            DataGridView dtgFacturasCliente,
            ComboBox cboArticulo,
            NumericUpDown nudCantidad,
            ComboBox cboCliente,
            Label lblCliente,
            GroupBox grpDatosCliente,
            Button btnEliminar,
            Button btnAgregar,
            params Label[] etiquetas) // Usamos params para permitir un número variable de etiquetas
        {
            // Restablecer el ComboBox del cliente
            cboCliente.SelectedIndex = 0;


            factura1.Subtotal = null;
            factura1.IVA = null;    

            // Deshabilitar todos los ComboBox en el formulario
            foreach (Control ctrl in frmFacturacion.Controls)
            {
                if (ctrl is ComboBox cbo)
                {
                    cbo.SelectedIndex = 0; // Restablecer al primer ítem
                    cbo.Enabled = false;   // Deshabilitar todos los ComboBox
                }
            }

            // Ocultar todas las etiquetas en grpDatosCliente menos lblCliente
            foreach (Control ctrl in grpDatosCliente.Controls)
            {
                if (ctrl is Label lbl && lbl != lblCliente)
                {
                    lbl.Visible = false;
                }
            }

            

            // Limpiar todas las filas del DataTable
            tablaArticulos.Clear();
            dtgDetallesVenta.DataSource = null;
            dtgDetallesVenta.Rows.Clear();
            dtgDetallesVenta.Visible = false;


            tablaFacturas.Clear();
            dtgFacturasCliente.DataSource = null;
            dtgFacturasCliente.Rows.Clear();
            dtgFacturasCliente.Visible = false;


            // Deshabilitar los botones btnEliminar y btnAgregar
            btnEliminar.Enabled = false;
            btnAgregar.Enabled = false;

            // Reiniciar el valor del NumericUpDown a 1
            nudCantidad.Value = 1;

            // Ocultar etiquetas adicionales pasadas como parámetros
            foreach (var etiqueta in etiquetas)
            {
                etiqueta.Visible = false;
            }
        }


    }
}
