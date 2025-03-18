using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.ReportAppServer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReporteFactura
{
    public partial class FormularioReporte : Form
    {
        public FormularioReporte()
        {
            InitializeComponent();
          
        }

        public void CargarReporte(ReportDocument report)
        {
          

            crystalReportViewer1.ReportSource = report;

            crystalReportViewer1.Refresh();

        }

       
    }
}
