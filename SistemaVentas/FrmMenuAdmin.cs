using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SistemaVentas
{
    public partial class FrmMenuAdmin : Form
    {
        private Panel panelContenido;
        private Label lblTitulo;
        private Label lblUsuario;

        private readonly Color azulOscuro =
            Color.FromArgb(15, 32, 60);

        private readonly Color azul =
            Color.FromArgb(35, 115, 210);

        private readonly Color fondo =
            Color.FromArgb(244, 247, 252);
        private readonly int _idUsuario;

        public FrmMenuAdmin(string nombre, int idUsuario)
        {
            ConstruirMenu(nombre, idUsuario);
            _idUsuario = idUsuario;
        }
        private void ConstruirMenu(string nombre, int idUsuario)
        {
            Text = "VENTAS PRO | Administrador";
            Size = new Size(1150, 720);
            MinimumSize = new Size(1000, 650);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = fondo;
            Font = new Font("Segoe UI", 10);

            // MENU LATERAL
            Panel lateral = new Panel();
            lateral.Dock = DockStyle.Left;
            lateral.Width = 235;
            lateral.BackColor = azulOscuro;

            Controls.Add(lateral);

            Label logo = new Label();
            logo.Text = "VENTAS PRO";
            logo.Font = new Font("Segoe UI", 21, FontStyle.Bold);
            logo.ForeColor = Color.White;
            logo.Location = new Point(22, 30);
            logo.AutoSize = true;
            lateral.Controls.Add(logo);

            Label subtitulo = new Label();
            subtitulo.Text = "PANEL ADMINISTRATIVO";
            subtitulo.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            subtitulo.ForeColor = Color.LightBlue;
            subtitulo.Location = new Point(25, 75);
            subtitulo.AutoSize = true;
            lateral.Controls.Add(subtitulo);

            CrearBoton(lateral, "▦   Dashboard", 125,
                () => MostrarDashboard());

            CrearBoton(lateral, "▣   Productos", 180, () =>
            {
                using (FrmProductos formulario = new FrmProductos("Administrador"))
                {
                    formulario.ShowDialog(this);
                }
            });

            CrearBoton(lateral, "🛒  Ventas", 235,
            () =>
            {
                using (FrmVentas formulario = new FrmVentas("Administrador", _idUsuario))
                {
                    formulario.ShowDialog(this);
                }
            });

            CrearBoton(lateral, "♙   Usuarios", 290,
                () =>
                {
                    using (FrmUsuarios formulario =
                        new FrmUsuarios(_idUsuario))
                    {
                        formulario.ShowDialog(this);
                    }
                });

            CrearBoton(lateral, "▤   Reportes", 345,
                () => {
                    using (FrmReportes formulario =
                        new FrmReportes("Administrador"))
                    {
                        formulario.ShowDialog(this);
                    }
                });

            Button btnCerrar = new Button();
            btnCerrar.Text = "↪   Cerrar sesión";
            btnCerrar.Location = new Point(15, 590);
            btnCerrar.Size = new Size(205, 45);
            btnCerrar.BackColor = Color.FromArgb(190, 55, 65);
            btnCerrar.ForeColor = Color.White;
            btnCerrar.FlatStyle = FlatStyle.Flat;
            btnCerrar.FlatAppearance.BorderSize = 0;
            btnCerrar.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnCerrar.Cursor = Cursors.Hand;
            btnCerrar.Click += (s, e) => Close();

            lateral.Controls.Add(btnCerrar);

            // CONTENIDO PRINCIPAL
            panelContenido = new Panel();
            panelContenido.Dock = DockStyle.Fill;
            panelContenido.BackColor = fondo;

            Controls.Add(panelContenido);

            // BARRA SUPERIOR
            Panel superior = new Panel();
            superior.Dock = DockStyle.Top;
            superior.Height = 85;
            superior.BackColor = Color.White;

            panelContenido.Controls.Add(superior);

            lblTitulo = new Label();
            lblTitulo.Text = "Dashboard";
            lblTitulo.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            lblTitulo.ForeColor = azulOscuro;
            lblTitulo.Location = new Point(30, 12);
            lblTitulo.AutoSize = true;

            superior.Controls.Add(lblTitulo);

            lblUsuario = new Label();
            lblUsuario.Text = "Administrador: " + nombre;
            lblUsuario.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblUsuario.ForeColor = azul;
            lblUsuario.AutoSize = true;
            lblUsuario.Location = new Point(30, 52);

            superior.Controls.Add(lblUsuario);

            MostrarDashboard();
        }
         

        private void CrearBoton(
            Panel panel, string texto, int posicion,
            Action accion)
        {
            Button boton = new Button();

            boton.Text = texto;
            boton.Location = new Point(15, posicion);
            boton.Size = new Size(205, 45);
            boton.BackColor = azulOscuro;
            boton.ForeColor = Color.White;
            boton.FlatStyle = FlatStyle.Flat;
            boton.FlatAppearance.BorderSize = 0;
            boton.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            boton.TextAlign = ContentAlignment.MiddleLeft;
            boton.Padding = new Padding(12, 0, 0, 0);
            boton.Cursor = Cursors.Hand;

            boton.MouseEnter += (s, e) =>
                boton.BackColor = azul;

            boton.MouseLeave += (s, e) =>
                boton.BackColor = azulOscuro;

            boton.Click += (s, e) => accion();

            panel.Controls.Add(boton);
        }

        private void MostrarDashboard()
        {
            panelContenido.Controls.Clear();

            lblTitulo.Text = "Dashboard";

            Panel superior = new Panel();
            superior.Dock = DockStyle.Top;
            superior.Height = 85;
            superior.BackColor = Color.White;

            panelContenido.Controls.Add(superior);

            lblTitulo.Location = new Point(30, 12);
            lblUsuario.Location = new Point(30, 52);

            superior.Controls.Add(lblTitulo);
            superior.Controls.Add(lblUsuario);

            Label bienvenida = new Label();
            bienvenida.Text = "Bienvenido al sistema de ventas";
            bienvenida.Font = new Font(
                "Segoe UI", 18, FontStyle.Bold);
            bienvenida.ForeColor = azulOscuro;
            bienvenida.Location = new Point(30, 115);
            bienvenida.AutoSize = true;

            panelContenido.Controls.Add(bienvenida);

            Label descripcion = new Label();
            descripcion.Text =
                "Administra los recursos y operaciones de tu negocio.";
            descripcion.ForeColor = Color.Gray;
            descripcion.Location = new Point(33, 155);
            descripcion.AutoSize = true;

            panelContenido.Controls.Add(descripcion);

            CrearTarjeta(
                "PRODUCTOS",
                "Inventario",
                Color.FromArgb(40, 120, 210),
                30, 220);

            CrearTarjeta(
                "VENTAS",
                "Operaciones",
                Color.FromArgb(20, 155, 120),
                275, 220);

            CrearTarjeta(
                "USUARIOS",
                "Administración",
                Color.FromArgb(135, 90, 190),
                520, 220);

            CrearTarjeta(
                "REPORTES",
                "Estadísticas",
                Color.FromArgb(220, 145, 45),
                30, 390);

            Label nota = new Label();
            nota.Text = "VENTAS PRO | Panel de administración";
            nota.ForeColor = Color.Gray;
            nota.AutoSize = true;
            nota.Location = new Point(35, 560);

            panelContenido.Controls.Add(nota);
        }

        private void CrearTarjeta(
            string titulo, string texto,
            Color color, int x, int y)
        {
            Panel tarjeta = new Panel();
            tarjeta.Size = new Size(215, 125);
            tarjeta.Location = new Point(x, y);
            tarjeta.BackColor = Color.White;

            panelContenido.Controls.Add(tarjeta);

            Panel franja = new Panel();
            franja.Dock = DockStyle.Left;
            franja.Width = 7;
            franja.BackColor = color;

            tarjeta.Controls.Add(franja);

            Label lblTitulo = new Label();
            lblTitulo.Text = titulo;
            lblTitulo.Font = new Font(
                "Segoe UI", 12, FontStyle.Bold);
            lblTitulo.ForeColor = color;
            lblTitulo.Location = new Point(22, 25);
            lblTitulo.AutoSize = true;

            tarjeta.Controls.Add(lblTitulo);

            Label lblTexto = new Label();
            lblTexto.Text = texto;
            lblTexto.Font = new Font("Segoe UI", 10);
            lblTexto.ForeColor = Color.Gray;
            lblTexto.Location = new Point(22, 65);
            lblTexto.AutoSize = true;

            tarjeta.Controls.Add(lblTexto);
        }

        private void MostrarMensaje(string modulo)
        {
            MessageBox.Show(
                "El módulo " + modulo +
                " lo construiremos en el siguiente paso.",
                "VENTAS PRO",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }
}
