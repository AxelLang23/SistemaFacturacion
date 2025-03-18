using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaDeNegocio
{
    public class Empresa
    {
        public string NombreEmpresa { get; set; } // Nombre de la empresa
        public CondicionIVA CondicionIVA { get; set; } // Condición IVA de la empresa
        public string Telefono { get; set; } // Teléfono de contacto
        public string CUIT { get; set; }
        public string Email { get; set; } // Correo electrónico
        public string Direccion { get; set; } // Dirección física
        public string Localidad { get; set; } // Localidad de la empresa
        public string CodigoPostal { get; set; } // Código postal
        public string Provincia { get; set; } // Provincia
        public string Pais { get; set; } // País
        
    }

}
