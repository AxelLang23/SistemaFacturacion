using AccesoDatos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicaDeNegocio;
using System.Net;
using System.Runtime.InteropServices;
using System.Diagnostics.Eventing.Reader;
using System.Net.Sockets;
using System.CodeDom;
using System.Collections;


namespace InterfazDeUsuario
{
    public partial class frmFacturacion : Form
    {

        public frmFacturacion()
        {
            InitializeComponent();
        }

        Cliente cliente1 = new Cliente();

        private DataTable tablaArticulos;

        private DataTable tablaFacturas;

        Articulo articulo1 = new Articulo();

        CondicionVenta condicionVenta1 = new CondicionVenta();

        Factura factura1 = new Factura();

        FacturaDetalle facturaDetalle1 = new FacturaDetalle();


        private void Facturacion_Load(object sender, EventArgs e)
        {

            // Cargar clientes con el primer ítem personalizado
            UtilidadesFormulario.CargarComboBox(cboCliente, cliente1.ObtenerDatosClientes(), "NombreCompleto", "Seleccionar Cliente");

            // Cargar artículos con su primer ítem personalizado
            UtilidadesFormulario.CargarComboBox(cboArticulo, articulo1.ObtenerDatosArticulos(), "Descripcion", "Seleccionar Artículo");

            // Cargar condiciones de venta con su ítem personalizado
            UtilidadesFormulario.CargarComboBox(cboCondicionVenta, condicionVenta1.ObtenerCondicionesVenta(), "Descripcion", "Seleccionar Condición de Venta");

            tablaArticulos = UtilidadesFormulario.CrearTablaArticulos();
            tablaFacturas = UtilidadesFormulario.CrearTablaFacturas();

        }


        private void cboCliente_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Cuando cambiamos el cliente,la grilla se limpiará para que no queden facturas del cliente previamente analizado.
            dtgFacturasCliente.DataSource = null;
            dtgFacturasCliente.Rows.Clear();
            dtgFacturasCliente.Visible = false;
            //Lo mismo con los productos agregados en caso de cambiar de cliente durante el proceso de completado.
            dtgDetallesVenta.DataSource = null;
            dtgDetallesVenta.Rows.Clear();
            dtgDetallesVenta.Visible = false;

            lblSubTotal.Visible = false;
            lblCantidadSubTotal.Visible = false;
            lblIVAInscripto.Visible = false;
            lblCantidadIVAInscripto.Visible = false ;
            lblTotalNeto.Visible = false;
            lblCantidadTotalNeto.Visible = false;
            nudCantidad.Value = 1;

            // Limpiar la tabla de artículos
            if (tablaArticulos != null) // Esto asegura que no sea nulo
            {
                tablaArticulos.Clear(); // Limpiar los datos de la tabla
            }

            // Verificar si los ComboBox tienen elementos antes de establecer el SelectedIndex
            if (cboArticulo.Items.Count > 0)
            {
                cboArticulo.SelectedIndex = 0;
            }

            if (cboCondicionVenta.Items.Count > 0)
            {
                cboCondicionVenta.SelectedIndex = 0;
            }

            if (cboCliente.SelectedIndex == 0)
            {
                btnAgregar.Enabled = false;
                nudCantidad.Enabled = false;
                cboArticulo.Enabled = false; // El ComboBox es visible pero no se puede desplegar
                cboCondicionVenta.Enabled = false;
                dtgDetallesVenta.Visible = false;
                dtgFacturasCliente.Visible = false;

                // Recorre todos los controles dentro de grpDatosCliente
                foreach (Control control in grpDatosCliente.Controls)
                {
                    // Verifica si el control es una etiqueta (Label) y no es lblCliente
                    if (control is Label && control.Name != "lblCliente")
                    {
                        control.Visible = false; // Oculta la etiqueta
                    }
                }

            }

            else
            {

                cliente1 = (Cliente)cboCliente.SelectedItem;

                // Muestra todas las etiquetas en grpDatosCliente
                foreach (Control control in grpDatosCliente.Controls)
                {
                    if (control is Label)
                    {
                        control.Visible = true; // Restablece la visibilidad de la etiqueta
                    }
                }

                cboArticulo.Enabled = true;
                cboCondicionVenta.Enabled = true;

                factura1.Cliente = cliente1;

                if (cliente1.CondicionIVA.Descripcion == "Responsable Inscripto")

                {
                    factura1.Tipo = 'A';
                }
                else
                {
                    factura1.Tipo = 'B';
                }
                lblDireccion.Text = cliente1.Direccion;

                lblLocalidad.Text = cliente1.Localidad;

                lblCodigoPostal.Text = cliente1.CodigoPostal;

                lblProvincia.Text = cliente1.Provincia;

                lblPais.Text = cliente1.Pais;

                lblCondicionIVA.Text = cliente1.CondicionIVA.Descripcion;

                lblNroCUIT.Text = cliente1.CUIT;


                if (cliente1.DNI.HasValue)
                {
                    lblNroDNI.Text = cliente1.DNI.Value.ToString(); // Si DNI tiene un valor, lo convertimos a texto
                }
                else
                {
                    lblNroDNI.Text = null; // Si DNI es null, asignamos null al Text del Label
                }

                if (cliente1.DNI == null)
                {
                    lblDNI.Visible = false;
                    lblNroDNI.Visible = false;
                }

                if (cliente1.CUIT == null)
                {
                    lblNroCUIT.Visible = false;
                    lblCUIT.Visible = false;
                }
            }
           
        }
        

        private void cboCondicionVenta_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboCondicionVenta.SelectedIndex != 0) // Verifica que no sea el primer ítem
            {
                condicionVenta1 = (CondicionVenta)cboCondicionVenta.SelectedItem;

                factura1.CondicionVenta = condicionVenta1;
            }
        }
    

        private void cboArticulo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboArticulo.SelectedIndex == 0)
            { 
                nudCantidad.Enabled = false;
                btnAgregar.Enabled = false;
            }
            else
            {
                btnAgregar.Enabled = true;
                nudCantidad.Enabled = true;
                
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            dtgDetallesVenta.Visible = true;

            // Recuperar el artículo seleccionado del ComboBox
            articulo1 = (Articulo)cboArticulo.SelectedItem;

            // Asignar el artículo y cantidad al detalle de factura
            facturaDetalle1.Articulo = articulo1;

            facturaDetalle1.Cantidad = (int)nudCantidad.Value;

            if (cliente1.CondicionIVA.Descripcion != "Responsable Inscripto")
            {
                facturaDetalle1.PrecioUnitario = articulo1.CalcularPrecioUnitarioConIVA(articulo1);
            }


            facturaDetalle1.PrecioTotal = facturaDetalle1.CalcularPrecioTotal(facturaDetalle1,factura1);

            bool ElArticuloExiste = false;

            // Recorre las filas del DataTable
            foreach (DataRow row in tablaArticulos.Rows)
            {
                // Verifica si el nombre del producto en la fila actual coincide con el seleccionado
                if (row["Descripción"].ToString() == articulo1.Descripcion)
                {
                    // Obtiene el valor actual de cantidad y el valor del NumericUpDown
                    int cantidadActual = Convert.ToInt32(row["Cantidad"]);
                    int cantidadAumentar = Convert.ToInt32(nudCantidad.Value);

                    // Suma la nueva cantidad al valor actual
                    cantidadActual += cantidadAumentar;

                    // Actualiza el campo Cantidad en la fila
                    row["Cantidad"] = cantidadActual;

                    facturaDetalle1.Cantidad = cantidadActual;

                    // Calcula el nuevo total (cantidad * precio unitario)
                    decimal nuevoTotal = facturaDetalle1.CalcularPrecioTotal(facturaDetalle1,factura1);

                    // Actualiza el total en la fila
                    row["Precio Total ($)"] = nuevoTotal;

                    ElArticuloExiste = true;
                    break; // Sale del bucle una vez que encuentra y actualiza el artículo
                }

            }

            if (!ElArticuloExiste)
            {
                UtilidadesFormulario.AgregarFilas(tablaArticulos, facturaDetalle1,factura1);

                dtgDetallesVenta.DataSource = tablaArticulos;
                dtgDetallesVenta.Columns["Alícuota"].Visible = false;
                dtgDetallesVenta.Columns["IdArtículo"].Visible = false;
            }

            dtgDetallesVenta.Refresh();

            if (cliente1.CondicionIVA.Descripcion == "Responsable Inscripto")
            {
                // Hacemos visible las 6 etiquetas relacionadas con el cálculo de IVA
                lblSubTotal.Visible = true;
                lblCantidadSubTotal.Visible = true;
                lblIVAInscripto.Visible = true;
                lblCantidadIVAInscripto.Visible = true;
                lblTotalNeto.Visible = true;
                lblCantidadTotalNeto.Visible = true;

                factura1.Subtotal = factura1.CalcularSumaPreciosTotales(tablaArticulos);
                lblCantidadSubTotal.Text = factura1.Subtotal.Value.ToString("C");

                factura1.IVA = factura1.CalcularIVAInscripto(tablaArticulos);
                lblCantidadIVAInscripto.Text = factura1.IVA.Value.ToString("C");

                // Usamos `.Value` en `factura1.Subtotal` y `factura1.IVA` porque son nullable (decimal?).
                // `.Value` extrae el valor decimal subyacente, asumiendo que no son `null` en este momento.
                factura1.TotalNeto = factura1.CalcularTotalNeto(factura1.Subtotal.Value, factura1.IVA.Value);
                lblCantidadTotalNeto.Text = factura1.TotalNeto.ToString("C");
            }
            else
            {
                // Hacemos visible solo las etiquetas de Total Neto, ocultamos SubTotal e IVA
                lblTotalNeto.Visible = true;
                lblCantidadTotalNeto.Visible = true;
                lblSubTotal.Visible = false;
                lblCantidadSubTotal.Visible = false;
                lblIVAInscripto.Visible = false;
                lblCantidadIVAInscripto.Visible = false;

                factura1.TotalNeto = factura1.CalcularSumaPreciosTotales(tablaArticulos);
                lblCantidadTotalNeto.Text = factura1.TotalNeto.ToString("C");
            }
            // Reiniciar el valor del NumericUpDown a 1
            nudCantidad.Value = 1;
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dtgDetallesVenta.SelectedRows.Count == 0)
            {

                MessageBox.Show("Seleccione una fila");
                return;

            }

            // Obtener la fila seleccionada
            DataGridViewRow filaSeleccionada = dtgDetallesVenta.SelectedRows[0];

            int cantidadActual = (int)filaSeleccionada.Cells["Cantidad"].Value;
            int cantidadAEliminar = (int)nudCantidad.Value;

            // Verificar si la cantidad a eliminar es mayor a la cantidad actual
            if (cantidadAEliminar > cantidadActual)
            {
                MessageBox.Show("Hay menos productos agregados que los que desea eliminar");
                return;
            }

            // Restar la cantidad
            int nuevaCantidad = cantidadActual - cantidadAEliminar;
            facturaDetalle1.Cantidad = nuevaCantidad;

            if (nuevaCantidad == 0)
            {
                // Si la cantidad llega a 0, eliminar la fila
                tablaArticulos.Rows.RemoveAt(filaSeleccionada.Index);
            }
            else
            {
                // Actualizar la cantidad en la fila seleccionada
                filaSeleccionada.Cells["Cantidad"].Value = nuevaCantidad;

                // Actualizar el precio total para la fila
                
                facturaDetalle1.PrecioUnitario = (decimal)filaSeleccionada.Cells["Precio Unitario ($)"].Value;
                decimal nuevoPrecioTotal = facturaDetalle1.CalcularPrecioTotal(facturaDetalle1, factura1);
                filaSeleccionada.Cells["Precio Total ($)"].Value = nuevoPrecioTotal;
            }

            // Verificar si el cliente es Responsable Inscripto
            if (cliente1.CondicionIVA.Descripcion == "Responsable Inscripto")
            {
                // Actualizar los valores de las etiquetas
                factura1.Subtotal = factura1.CalcularSumaPreciosTotales(tablaArticulos);  
                lblCantidadSubTotal.Text = factura1.Subtotal.Value.ToString("C");
                

                factura1.IVA = factura1.CalcularIVAInscripto(tablaArticulos);
                lblCantidadIVAInscripto.Text = factura1.IVA.Value.ToString("C");
                

                factura1.TotalNeto = factura1.CalcularTotalNeto(factura1.Subtotal.Value, factura1.IVA.Value);
                lblCantidadTotalNeto.Text = factura1.TotalNeto.ToString("C");
            }
            else
            {
                // Actualizar el valor de Total Neto
                factura1.TotalNeto = factura1.CalcularSumaPreciosTotales(tablaArticulos);
                lblCantidadTotalNeto.Text = factura1.TotalNeto.ToString("C");
            }

            // Refrescar la grilla
            dtgDetallesVenta.Refresh();

            // Reiniciar el valor del NumericUpDown a 1
            nudCantidad.Value = 1;

        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (cboCliente.SelectedIndex == 0 ||
                cboArticulo.SelectedIndex == 0 ||
                cboCondicionVenta.SelectedIndex == 0 ||
                dtgDetallesVenta.Rows.Count == 0 ||
                (dtgDetallesVenta.Rows.Count == 1 && dtgDetallesVenta.Rows[0].IsNewRow))
            {
                MessageBox.Show("Complete correctamente lo solicitado");
            }

            else
            {
                facturaDetalle1.Factura = factura1;
                factura1.Fecha = DateTime.Now;
                SqlCommand comando = factura1.InserccionFacturaYDetalle(factura1, tablaArticulos);
                ConexionBD.EjecutarInserccion(comando);
                MessageBox.Show("Factura guardada con éxito");
            }
          
        }


        private void dtgDetallesVenta_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            // Verifica si la grilla está vacía o si solo contiene la fila para añadir
            if (dtgDetallesVenta.Rows.Count == 0 ||
                (dtgDetallesVenta.Rows.Count == 1 && dtgDetallesVenta.Rows[0].IsNewRow))
            {
                btnEliminar.Enabled = false;  // Deshabilita el botón
            }
            else
            {
                btnEliminar.Enabled = true;   // Habilita el botón
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            // Llama al método LimpiarFormulario con los parámetros necesarios
            UtilidadesFormulario.LimpiarFormulario(
                this,
                tablaArticulos,
                dtgDetallesVenta,
                factura1,
                tablaFacturas,
                dtgFacturasCliente,
                cboArticulo,
                nudCantidad,
                cboCliente,
                lblCliente,
                grpDatosCliente,
                btnEliminar,
                btnAgregar,
                lblSubTotal,              
                lblCantidadSubTotal,     
                lblIVAInscripto,          
                lblCantidadIVAInscripto,  
                lblTotalNeto,             
                lblCantidadTotalNeto       
                                           
            );

        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            DialogResult respuesta;
            respuesta = MessageBox.Show("¿Está seguro que desea salir?", "Confirme la operación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (respuesta == DialogResult.Yes)
            {
                // Cerrar el formulario llamado Facturacion
                Close(); // Si estás ejecutando esto dentro del formulario llamado Facturacion.
            }
        }

        private void btnVerFactura_Click(object sender, EventArgs e)
        {

          if (!(cboCliente.SelectedIndex == 0))
          { 
            dtgFacturasCliente.Visible = true;
            List<Factura> listaFacturas = factura1.ObtenerFacturasPorCliente(cliente1);
            //La tabla se limpia para que no se repitan las facturas de los clientes en la grilla.
            tablaFacturas.Clear();
            UtilidadesFormulario.AgregarFilasFacturas(tablaFacturas, listaFacturas);
            dtgFacturasCliente.DataSource = tablaFacturas;

            if (!(cliente1.CondicionIVA.Descripcion == "Responsable Inscripto"))
            {
                dtgFacturasCliente.Columns["IVA"].Visible = false;
                dtgFacturasCliente.Columns["Subtotal"].Visible = false;
            }
          }
            
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
 

            if (dtgFacturasCliente.SelectedRows.Count > 0)
            {
                // Obtener el valor de la columna NroFactura de la fila seleccionada
                int NroFactura = (int)dtgFacturasCliente.SelectedRows[0].Cells["NroFactura"].Value;

                char tipoFactura = Convert.ToChar(dtgFacturasCliente.SelectedRows[0].Cells["Tipo"].Value);

                factura1.ImprimirFacturaSeleccionada(NroFactura,tipoFactura);
            }
            else
            {
                // Si no hay ninguna fila seleccionada, imprimir la última factura insertada
                factura1.ImprimirUltimaFactura();
            }


        }


    }
  
}
