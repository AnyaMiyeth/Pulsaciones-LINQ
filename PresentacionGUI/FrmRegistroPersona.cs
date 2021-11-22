using Entidad;
using Logica;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PresentacionGUI
{
    public partial class FrmRegistroPersona : Form,IReceptor
    {
        PersonaService personaService;
        


        public FrmRegistroPersona()
        {
            InitializeComponent();
            personaService = new PersonaService(ConfigConnectionString.ConnectionString);
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            Persona persona = MapearPersona();
           
            string mensaje = personaService.Guardar(persona);
            MessageBox.Show(mensaje, "Guardar Persona", MessageBoxButtons.OK, MessageBoxIcon.Information);


        }

        private Persona MapearPersona()
        {
            Persona persona = new Mujer();
            persona.Identificacion = txtIdentificacion.Text;
            persona.Nombre = txtNombre.Text;
            persona.Sexo = cmbSexo.Text;
            persona.Edad = int.Parse(txtEdad.Text);
            persona.CalcularPulsacion();
            txtPulsacion.Text = persona.Pulsacion.ToString();
            return persona;
        }

        private void BtnCalcular_Click(object sender, EventArgs e)
        {
            Persona persona = MapearPersona();
            btnGuardar.Enabled = true;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            txtIdentificacion.Text = "";
            txtNombre.Text = "";
            txtEdad.Text = "";
            cmbSexo.Text = "";
            txtPulsacion.Text = "";
            btnGuardar.Enabled = false;
        }

       

        private void button1_Click(object sender, EventArgs e)
        {
            FrmConsultaPersona frmConsulta = new FrmConsultaPersona(this);
            frmConsulta.Show();
        }

        

        private void FrmRegistroPersona_Load(object sender, EventArgs e)
        {

        }

        public void Recibir(Persona persona)
        {
            if (persona!=null)
            {
                txtIdentificacion.Text = persona.Identificacion;
                txtNombre.Text = persona.Nombre;
                txtEdad.Text = persona.Edad.ToString();
                cmbSexo.Text = persona.Sexo;
                txtPulsacion.Text = persona.Pulsacion.ToString();
            }
        }
    }
}
