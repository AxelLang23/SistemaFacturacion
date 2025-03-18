using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicaDeNegocio;
using AccesoDatos;

namespace InterfazDeUsuario
{
    public partial class frmAcceso : Form
    {
        public frmAcceso()
        {
            InitializeComponent();
        }

        Usuario usuario1 = new Usuario();

        private void Acceso_Load(object sender, EventArgs e)
        {

            // Obtener la lista de usuarios desde la base de datos
            List<string> listaUsuarios = Usuario.ObtenerUsuarios();

            // Agregar el texto predeterminado
            listaUsuarios.Insert(0, "Seleccione su usuario"); // Este es el item fijo

            // Asignar la lista de usuarios al ComboBox
            cboUsuario.DataSource = listaUsuarios;

            // Establecer el índice seleccionado al primer elemento
            cboUsuario.SelectedIndex = 0; // Esto asegura que se muestre "Seleccione su usuario"

        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            // Obtener el usuario seleccionado y la contraseña ingresada
            usuario1.NombreUsuario = cboUsuario.SelectedItem.ToString();
            usuario1.Contraseña = txtContraseña.Text;

            // Verificar si el usuario ha seleccionado uno de la lista
            if (usuario1.NombreUsuario == "Seleccione su usuario")
            {
                // Si no ha seleccionado un usuario válido, muestra un mensaje de error
                MessageBox.Show("Por favor, seleccione un usuario.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Salir del método sin continuar con la validación
            }

            // Validar usuario y contraseña
            if (Usuario.ValidarUsuario(usuario1))
            {
                
                // Si la validación es correcta, crea una nueva instancia del formulario Facturación
                frmFacturacion formFacturacion = new frmFacturacion();

                // Muestra el formulario Facturación
                formFacturacion.Show();

                this.Hide();


            }
            else
            {
                // Si la validación falla, muestra un mensaje de error
                MessageBox.Show("La contraseña es incorrecta", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

    }
}
