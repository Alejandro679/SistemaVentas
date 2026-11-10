using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SistemaVentas.Conexion;

namespace SistemaVentas
{
    public partial class FrmLogin : Form
    {
        private TextBox txtUsuario;
        private TextBox txtPassword;
        private Button btnIngresar;
        private Label lblEstado;
        private Panel panelDerecho;
        private Panel tarjeta;

        public FrmLogin()
        {
            ConstruirInterfaz();

            this.Load += FrmLogin_Load;
        }
        private void ConstruirInterfaz()
        {
            // CONFIGURACION GENERAL
            this.Text = "VENTAS PRO | Inicio de sesión";
            this.Size = new Size(1000, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 10);

            // PANEL IZQUIERDO
            Panel panelIzquierdo = new Panel();
            panelIzquierdo.Dock = DockStyle.Left;
            panelIzquierdo.Width = 290;
            panelIzquierdo.BackColor =
                Color.FromArgb(15, 32, 60);

            this.Controls.Add(panelIzquierdo);

            Label lblLogo = new Label();
            lblLogo.Text = "VENTAS PRO";
            lblLogo.Font = new Font(
                "Segoe UI", 27, FontStyle.Bold);
            lblLogo.ForeColor = Color.White;
            lblLogo.AutoSize = true;
            lblLogo.Location = new Point(45, 155);

            panelIzquierdo.Controls.Add(lblLogo);

            Label lblLinea = new Label();
            lblLinea.BackColor = Color.FromArgb(35, 135, 235);
            lblLinea.Size = new Size(75, 5);
            lblLinea.Location = new Point(48, 215);

            panelIzquierdo.Controls.Add(lblLinea);

            Label lblDescripcion = new Label();
            lblDescripcion.Text =
                "SISTEMA DE GESTIÓN\n\n" +
                "Controla tus productos,\n" +
                "ventas e inventario desde\n" +
                "un solo lugar.";
            lblDescripcion.Font =
                new Font("Segoe UI", 14);
            lblDescripcion.ForeColor =
                Color.FromArgb(220, 230, 245);
            lblDescripcion.AutoSize = true;
            lblDescripcion.Location =
                new Point(48, 250);

            panelIzquierdo.Controls.Add(lblDescripcion);

            Label lblVersion = new Label();
            lblVersion.Text = "VENTAS PRO  |  SISTEMA COMERCIAL";
            lblVersion.Font =
                new Font("Segoe UI", 8, FontStyle.Bold);
            lblVersion.ForeColor = Color.Silver;
            lblVersion.AutoSize = true;
            lblVersion.Location = new Point(35, 570);

            panelIzquierdo.Controls.Add(lblVersion);

            // PANEL DERECHO
            panelDerecho = new Panel();
            panelDerecho.Dock = DockStyle.Fill;
            panelDerecho.BackColor =
                Color.FromArgb(244, 247, 252);

            this.Controls.Add(panelDerecho);

            // TARJETA DE LOGIN
            tarjeta = new Panel();
            tarjeta.Size = new Size(650, 475);
            tarjeta.BackColor = Color.White;

            panelDerecho.Controls.Add(tarjeta);

            panelDerecho.Resize += (s, e) =>
            {
                CentrarTarjeta();
            };

            CentrarTarjeta();

            // TITULO
            Label lblTitulo = new Label();
            lblTitulo.Text = "¡Bienvenido!";
            lblTitulo.Font =
                new Font("Segoe UI", 25, FontStyle.Bold);
            lblTitulo.ForeColor =
                Color.FromArgb(25, 42, 70);
            lblTitulo.AutoSize = true;
            lblTitulo.Location = new Point(203, 35);

            tarjeta.Controls.Add(lblTitulo);

            Label lblSubtitulo = new Label();
            lblSubtitulo.Text =
                "Ingresa tus credenciales para continuar";
            lblSubtitulo.Font =
                new Font("Segoe UI", 10);
            lblSubtitulo.ForeColor = Color.Gray;
            lblSubtitulo.AutoSize = true;
            lblSubtitulo.Location = new Point(203, 88);

            tarjeta.Controls.Add(lblSubtitulo);

            // USUARIO
            Label lblUsuario = new Label();
            lblUsuario.Text = "USUARIO";
            lblUsuario.Font =
                new Font("Segoe UI", 9, FontStyle.Bold);
            lblUsuario.ForeColor =
                Color.FromArgb(45, 60, 85);
            lblUsuario.AutoSize = true;
            lblUsuario.Location = new Point(203, 145);

            tarjeta.Controls.Add(lblUsuario);

            txtUsuario = new TextBox();
            txtUsuario.Location = new Point(203, 175);
            txtUsuario.Size = new Size(360, 38);
            txtUsuario.Font = new Font("Segoe UI", 12);
            txtUsuario.PlaceholderText = "Escribe tu usuario";

            tarjeta.Controls.Add(txtUsuario);

            // CONTRASEÑA
            Label lblPassword = new Label();
            lblPassword.Text = "CONTRASEÑA";
            lblPassword.Font =
                new Font("Segoe UI", 9, FontStyle.Bold);
            lblPassword.ForeColor =
                Color.FromArgb(45, 60, 85);
            lblPassword.AutoSize = true;
            lblPassword.Location = new Point(203, 235);

            tarjeta.Controls.Add(lblPassword);

            txtPassword = new TextBox();
            txtPassword.Location = new Point(203, 265);
            txtPassword.Size = new Size(360, 38);
            txtPassword.Font = new Font("Segoe UI", 12);
            txtPassword.PasswordChar = '●';
            txtPassword.PlaceholderText =
                "Escribe tu contraseña";

            tarjeta.Controls.Add(txtPassword);

            // BOTON INGRESAR
            btnIngresar = new Button();
            btnIngresar.Text = "INICIAR SESIÓN";
            btnIngresar.Location = new Point(203, 335);
            btnIngresar.Size = new Size(360, 50);
            btnIngresar.BackColor =
                Color.FromArgb(30, 115, 210);
            btnIngresar.ForeColor = Color.White;
            btnIngresar.FlatStyle = FlatStyle.Flat;
            btnIngresar.FlatAppearance.BorderSize = 0;
            btnIngresar.Font =
                new Font("Segoe UI", 11, FontStyle.Bold);
            btnIngresar.Cursor = Cursors.Hand;
            btnIngresar.Click += BtnIngresar_Click;

            tarjeta.Controls.Add(btnIngresar);

            // ESTADO DE CONEXION
            lblEstado = new Label();
            lblEstado.Text = "Verificando conexión...";
            lblEstado.Font = new Font("Segoe UI", 9);
            lblEstado.ForeColor = Color.Gray;
            lblEstado.AutoSize = true;
            lblEstado.Location = new Point(203, 410);

            tarjeta.Controls.Add(lblEstado);

            // ENTER PARA INGRESAR
            txtPassword.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    BtnIngresar_Click(s, e);
                }
            };
        }

        private void CentrarTarjeta()
        {
            if (tarjeta == null || panelDerecho == null)
                return;

            tarjeta.Left =
                (panelDerecho.ClientSize.Width - tarjeta.Width) / 2;

            tarjeta.Top =
                (panelDerecho.ClientSize.Height - tarjeta.Height) / 2;
        }

        private void FrmLogin_Load(object sender, EventArgs e)
        {
            bool conectado = ConexionBD.ProbarConexion(out string mensaje);

            if (conectado)
            {
                lblEstado.Text = "● SQL Server conectado";
                lblEstado.ForeColor = Color.Green;
                btnIngresar.Enabled = true;
            }
            else
            {
                lblEstado.Text = "● Error de conexión SQL";
                lblEstado.ForeColor = Color.Red;
                btnIngresar.Enabled = false;

                MessageBox.Show(
                    mensaje,
                    "Error de conexión",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void BtnIngresar_Click(object sender, EventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(usuario) ||
                string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show(
                    "Debes ingresar usuario y contraseña.",
                    "Campos obligatorios",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            try
            {
                using (SqlConnection conexion =
                    ConexionBD.ObtenerConexion())
                {
                    conexion.Open();

                    string consulta = @"
                        SELECT TOP 1 IdUsuario, Nombre, Rol
                        FROM dbo.Usuarios
                        WHERE Usuario = @Usuario
                        AND [Password] = @Password";

                    using (SqlCommand comando =
                        new SqlCommand(consulta, conexion))
                    {
                        comando.Parameters.AddWithValue(
                            "@Usuario", usuario);

                        comando.Parameters.AddWithValue(
                            "@Password", password);

                        using (SqlDataReader lector =
                            comando.ExecuteReader())
                        {
                            if (lector.Read())
                            {
                                int idUsuario =
                                    Convert.ToInt32(
                                        lector["IdUsuario"]);

                                string nombre =
                                    lector["Nombre"].ToString();

                                string rol =
                                    lector["Rol"].ToString().Trim();

                                if (rol.Equals(
                                    "Administrador",
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    AbrirMenu(nombre, rol, idUsuario);
                                }
                                else if (rol.Equals(
                                    "Vendedor",
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    AbrirMenu(  nombre, rol, idUsuario);
                                }
                                else
                                {
                                    MessageBox.Show(
                                        "El rol de usuario no es válido.",
                                        "Acceso denegado");
                                }
                            }
                            else
                            {
                                MessageBox.Show(
                                    "Usuario o contraseña incorrectos.",
                                    "Acceso denegado",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);

                                txtPassword.Clear();
                                txtPassword.Focus();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al iniciar sesión:\n" + ex.Message,
                    "VENTAS PRO",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void AbrirMenu(
    string nombre, string rol, int idUsuario)
        {
            Form menu;

            if (rol.Equals(
                "Administrador",
                StringComparison.OrdinalIgnoreCase))
            {
                menu = new FrmMenuAdmin(nombre, idUsuario);
            }
            else if (rol.Equals(
                "Vendedor",
                StringComparison.OrdinalIgnoreCase))
            {
                menu = new FrmMenuVendedor(nombre, idUsuario);
            }
            else
            {
                MessageBox.Show("Rol no autorizado.");
                return;
            }

            this.Hide();

            using (menu)
            {
                menu.ShowDialog(this);
            }

            this.Show();

            txtPassword.Clear();
            txtUsuario.Clear();
            txtUsuario.Focus();
        }
    }
}

