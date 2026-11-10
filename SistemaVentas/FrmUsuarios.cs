using Microsoft.Data.SqlClient;
using SistemaVentas.Conexion;
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
    public partial class FrmUsuarios : Form
    {
        private readonly int idUsuarioActual;

        private Panel panelDatos;
        private DataGridView dgvUsuarios;

        private TextBox txtUsuario;
        private TextBox txtPassword;
        private TextBox txtNombre;
        private TextBox txtBuscar;

        private ComboBox cmbRol;

        private Button btnGuardar;
        private Button btnActualizar;
        private Button btnEliminar;
        private Button btnLimpiar;
        private Button btnBuscar;

        private int idSeleccionado = 0;

        public FrmUsuarios(int idUsuarioActual)
        {
            this.idUsuarioActual = idUsuarioActual;

            ConstruirInterfaz();
            CargarUsuarios();
        }
        private void ConstruirInterfaz()
        {
            Text = "VENTAS PRO - Administración de Usuarios";
            Size = new Size(1200, 720);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(240, 244, 248);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            // ENCABEZADO
            Panel encabezado = new Panel();
            encabezado.Dock = DockStyle.Top;
            encabezado.Height = 75;
            encabezado.BackColor = Color.FromArgb(20, 45, 80);
            Controls.Add(encabezado);

            Label titulo = new Label();
            titulo.Text = "ADMINISTRACIÓN DE USUARIOS";
            titulo.Font = new Font("Segoe UI", 19, FontStyle.Bold);
            titulo.ForeColor = Color.White;
            titulo.Location = new Point(25, 18);
            titulo.AutoSize = true;
            encabezado.Controls.Add(titulo);

            // PANEL DE DATOS
            panelDatos = new Panel();
            panelDatos.Location = new Point(20, 100);
            panelDatos.Size = new Size(340, 535);
            panelDatos.BackColor = Color.White;
            panelDatos.BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(panelDatos);

            Label lblDatos = new Label();
            lblDatos.Text = "DATOS DEL USUARIO";
            lblDatos.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            lblDatos.ForeColor = Color.FromArgb(20, 45, 80);
            lblDatos.Location = new Point(20, 15);
            lblDatos.AutoSize = true;
            panelDatos.Controls.Add(lblDatos);

            CrearEtiqueta("Nombre completo", 65);
            txtNombre = CrearCampo(90);

            CrearEtiqueta("Usuario", 145);
            txtUsuario = CrearCampo(170);

            CrearEtiqueta("Contraseña", 225);
            txtPassword = CrearCampo(250);
            txtPassword.UseSystemPasswordChar = true;

            CrearEtiqueta("Rol", 305);

            cmbRol = new ComboBox();
            cmbRol.Location = new Point(20, 330);
            cmbRol.Size = new Size(290, 30);
            cmbRol.Font = new Font("Segoe UI", 10);
            cmbRol.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbRol.Items.Add("Administrador");
            cmbRol.Items.Add("Vendedor");
            cmbRol.SelectedIndex = 1;
            panelDatos.Controls.Add(cmbRol);

            // BOTONES
            btnGuardar = CrearBoton(
                "GUARDAR", 15, 385,
                Color.FromArgb(39, 174, 96));

            btnActualizar = CrearBoton(
                "ACTUALIZAR", 175, 385,
                Color.FromArgb(41, 128, 185));

            btnEliminar = CrearBoton(
                "ELIMINAR", 15, 440,
                Color.FromArgb(192, 57, 43));

            btnLimpiar = CrearBoton(
                "LIMPIAR", 175, 440,
                Color.FromArgb(127, 140, 141));

            btnGuardar.Click += BtnGuardar_Click;
            btnActualizar.Click += BtnActualizar_Click;
            btnEliminar.Click += BtnEliminar_Click;
            btnLimpiar.Click += BtnLimpiar_Click;

            // PANEL DE LISTADO
            Panel panelListado = new Panel();
            panelListado.Location = new Point(380, 100);
            panelListado.Size = new Size(790, 535);
            panelListado.BackColor = Color.White;
            panelListado.BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(panelListado);

            Label lblListado = new Label();
            lblListado.Text = "USUARIOS REGISTRADOS";
            lblListado.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            lblListado.ForeColor = Color.FromArgb(20, 45, 80);
            lblListado.Location = new Point(15, 15);
            lblListado.AutoSize = true;
            panelListado.Controls.Add(lblListado);

            txtBuscar = new TextBox();
            txtBuscar.Location = new Point(15, 60);
            txtBuscar.Size = new Size(550, 32);
            txtBuscar.Font = new Font("Segoe UI", 10);
            panelListado.Controls.Add(txtBuscar);

            btnBuscar = new Button();
            btnBuscar.Text = "BUSCAR";
            btnBuscar.Location = new Point(585, 57);
            btnBuscar.Size = new Size(175, 38);
            btnBuscar.BackColor = Color.FromArgb(41, 128, 185);
            btnBuscar.ForeColor = Color.White;
            btnBuscar.FlatStyle = FlatStyle.Flat;
            btnBuscar.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnBuscar.Click += BtnBuscar_Click;
            panelListado.Controls.Add(btnBuscar);

            dgvUsuarios = new DataGridView();
            dgvUsuarios.Location = new Point(15, 115);
            dgvUsuarios.Size = new Size(745, 395);

            dgvUsuarios.BackgroundColor = Color.White;
            dgvUsuarios.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
            dgvUsuarios.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dgvUsuarios.MultiSelect = false;
            dgvUsuarios.ReadOnly = true;
            dgvUsuarios.AllowUserToAddRows = false;
            dgvUsuarios.RowHeadersVisible = false;
            dgvUsuarios.EnableHeadersVisualStyles = false;

            dgvUsuarios.ColumnHeadersDefaultCellStyle.BackColor =
                Color.FromArgb(20, 45, 80);
            dgvUsuarios.ColumnHeadersDefaultCellStyle.ForeColor =
                Color.White;
            dgvUsuarios.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9, FontStyle.Bold);

            dgvUsuarios.DefaultCellStyle.Font =
                new Font("Segoe UI", 9);

            dgvUsuarios.CellClick += DgvUsuarios_CellClick;

            panelListado.Controls.Add(dgvUsuarios);

            // CERRAR
            Button btnCerrar = new Button();
            btnCerrar.Text = "CERRAR";
            btnCerrar.Location = new Point(1020, 650);
            btnCerrar.Size = new Size(150, 35);
            btnCerrar.BackColor = Color.FromArgb(20, 45, 80);
            btnCerrar.ForeColor = Color.White;
            btnCerrar.FlatStyle = FlatStyle.Flat;
            btnCerrar.Click += (s, e) => Close();
            Controls.Add(btnCerrar);
        }

        private void CrearEtiqueta(string texto, int y)
        {
            Label etiqueta = new Label();
            etiqueta.Text = texto;
            etiqueta.Location = new Point(20, y);
            etiqueta.AutoSize = true;
            etiqueta.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            etiqueta.ForeColor = Color.FromArgb(44, 62, 80);
            panelDatos.Controls.Add(etiqueta);
        }

        private TextBox CrearCampo(int y)
        {
            TextBox campo = new TextBox();
            campo.Location = new Point(20, y);
            campo.Size = new Size(290, 30);
            campo.Font = new Font("Segoe UI", 10);
            panelDatos.Controls.Add(campo);
            return campo;
        }

        private Button CrearBoton(
            string texto, int x, int y, Color color)
        {
            Button boton = new Button();
            boton.Text = texto;
            boton.Location = new Point(x, y);
            boton.Size = new Size(145, 42);
            boton.BackColor = color;
            boton.ForeColor = Color.White;
            boton.FlatStyle = FlatStyle.Flat;
            boton.FlatAppearance.BorderSize = 0;
            boton.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            boton.Cursor = Cursors.Hand;

            panelDatos.Controls.Add(boton);
            return boton;
        }

        // CARGAR USUARIOS
        private void CargarUsuarios(string filtro = "")
        {
            try
            {
                using (SqlConnection conexion =
                    ConexionBD.ObtenerConexion())
                {
                    conexion.Open();

                    string consulta = @"
                        SELECT IdUsuario, Usuario, Nombre, Rol
                        FROM dbo.Usuarios
                        WHERE Usuario LIKE @filtro
                           OR Nombre LIKE @filtro
                           OR Rol LIKE @filtro
                        ORDER BY IdUsuario DESC";

                    using (SqlCommand comando =
                        new SqlCommand(consulta, conexion))
                    {
                        comando.Parameters.AddWithValue(
                            "@filtro", "%" + filtro + "%");

                        using (SqlDataAdapter adaptador =
                            new SqlDataAdapter(comando))
                        {
                            DataTable tabla = new DataTable();
                            adaptador.Fill(tabla);
                            dgvUsuarios.DataSource = tabla;
                        }
                    }
                }

                dgvUsuarios.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al cargar usuarios:\n" + ex.Message);
            }
        }

        // VALIDAR DATOS
        private bool ValidarDatos(bool esNuevo)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtUsuario.Text) ||
                cmbRol.SelectedIndex == -1)
            {
                MessageBox.Show("Completa todos los campos.");
                return false;
            }

            if (esNuevo &&
                string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show(
                    "Debes ingresar una contraseña para el nuevo usuario.");
                txtPassword.Focus();
                return false;
            }

            return true;
        }

        // COMPROBAR USUARIO REPETIDO
        private bool UsuarioExiste(string usuario)
        {
            using (SqlConnection conexion =
                ConexionBD.ObtenerConexion())
            {
                conexion.Open();

                string consulta = @"
                    SELECT COUNT(*)
                    FROM dbo.Usuarios
                    WHERE Usuario = @Usuario
                      AND IdUsuario <> @IdUsuario";

                using (SqlCommand comando =
                    new SqlCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue(
                        "@Usuario", usuario);

                    comando.Parameters.AddWithValue(
                        "@IdUsuario", idSeleccionado);

                    return Convert.ToInt32(
                        comando.ExecuteScalar()) > 0;
                }
            }
        }

        // GUARDAR
        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarDatos(true)) return;

            try
            {
                if (UsuarioExiste(txtUsuario.Text.Trim()))
                {
                    MessageBox.Show(
                        "Ese nombre de usuario ya está registrado.");
                    return;
                }

                using (SqlConnection conexion =
                    ConexionBD.ObtenerConexion())
                {
                    conexion.Open();

                    string consulta = @"
                        INSERT INTO dbo.Usuarios
                        (Usuario, [Password], Nombre, Rol)
                        VALUES
                        (@Usuario, @Password, @Nombre, @Rol)";

                    using (SqlCommand comando =
                        new SqlCommand(consulta, conexion))
                    {
                        comando.Parameters.AddWithValue(
                            "@Usuario", txtUsuario.Text.Trim());

                        comando.Parameters.AddWithValue(
                            "@Password", txtPassword.Text);

                        comando.Parameters.AddWithValue(
                            "@Nombre", txtNombre.Text.Trim());

                        comando.Parameters.AddWithValue(
                            "@Rol", cmbRol.Text);

                        comando.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Usuario registrado correctamente.");

                LimpiarCampos();
                CargarUsuarios();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al guardar usuario:\n" + ex.Message);
            }
        }

        // ACTUALIZAR
        private void BtnActualizar_Click(object sender, EventArgs e)
        {
            if (idSeleccionado == 0)
            {
                MessageBox.Show("Selecciona un usuario de la tabla.");
                return;
            }

            if (!ValidarDatos(false)) return;

            try
            {
                if (UsuarioExiste(txtUsuario.Text.Trim()))
                {
                    MessageBox.Show(
                        "Ese nombre de usuario ya pertenece a otra cuenta.");
                    return;
                }

                using (SqlConnection conexion =
                    ConexionBD.ObtenerConexion())
                {
                    conexion.Open();

                    string consulta;

                    if (string.IsNullOrWhiteSpace(txtPassword.Text))
                    {
                        consulta = @"
                            UPDATE dbo.Usuarios
                            SET Usuario = @Usuario,
                                Nombre = @Nombre,
                                Rol = @Rol
                            WHERE IdUsuario = @IdUsuario";
                    }
                    else
                    {
                        consulta = @"
                            UPDATE dbo.Usuarios
                            SET Usuario = @Usuario,
                                [Password] = @Password,
                                Nombre = @Nombre,
                                Rol = @Rol
                            WHERE IdUsuario = @IdUsuario";
                    }

                    using (SqlCommand comando =
                        new SqlCommand(consulta, conexion))
                    {
                        comando.Parameters.AddWithValue(
                            "@Usuario", txtUsuario.Text.Trim());

                        comando.Parameters.AddWithValue(
                            "@Nombre", txtNombre.Text.Trim());

                        comando.Parameters.AddWithValue(
                            "@Rol", cmbRol.Text);

                        comando.Parameters.AddWithValue(
                            "@IdUsuario", idSeleccionado);

                        if (!string.IsNullOrWhiteSpace(txtPassword.Text))
                        {
                            comando.Parameters.AddWithValue(
                                "@Password", txtPassword.Text);
                        }

                        comando.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Usuario actualizado correctamente.");

                LimpiarCampos();
                CargarUsuarios();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al actualizar usuario:\n" + ex.Message);
            }
        }

        // ELIMINAR
        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (idSeleccionado == 0)
            {
                MessageBox.Show("Selecciona un usuario.");
                return;
            }

            if (idSeleccionado == idUsuarioActual)
            {
                MessageBox.Show(
                    "No puedes eliminar la cuenta con la que has iniciado sesión.");
                return;
            }

            DialogResult respuesta = MessageBox.Show(
                "¿Estás seguro de eliminar este usuario?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (respuesta != DialogResult.Yes) return;

            try
            {
                using (SqlConnection conexion =
                    ConexionBD.ObtenerConexion())
                {
                    conexion.Open();

                    string consulta = @"
                        DELETE FROM dbo.Usuarios
                        WHERE IdUsuario = @IdUsuario";

                    using (SqlCommand comando =
                        new SqlCommand(consulta, conexion))
                    {
                        comando.Parameters.AddWithValue(
                            "@IdUsuario", idSeleccionado);

                        comando.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Usuario eliminado correctamente.");

                LimpiarCampos();
                CargarUsuarios();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(
                    "No se pudo eliminar el usuario.\n" +
                    "Puede estar relacionado con ventas registradas.\n\n" +
                    ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al eliminar usuario:\n" + ex.Message);
            }
        }

        // SELECCIONAR USUARIO
        private void DgvUsuarios_CellClick(
            object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow fila =
                dgvUsuarios.Rows[e.RowIndex];

            idSeleccionado = Convert.ToInt32(
                fila.Cells["IdUsuario"].Value);

            txtUsuario.Text = Convert.ToString(
                fila.Cells["Usuario"].Value);

            txtNombre.Text = Convert.ToString(
                fila.Cells["Nombre"].Value);

            cmbRol.Text = Convert.ToString(
                fila.Cells["Rol"].Value);

            // No mostramos contraseñas en la tabla.
            // Dejar vacío conserva la contraseña al actualizar.
            txtPassword.Clear();
        }

        // BUSCAR
        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            CargarUsuarios(txtBuscar.Text.Trim());
        }

        // LIMPIAR
        private void BtnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private void LimpiarCampos()
        {
            txtNombre.Clear();
            txtUsuario.Clear();
            txtPassword.Clear();

            cmbRol.SelectedIndex = 1;

            idSeleccionado = 0;

            dgvUsuarios.ClearSelection();

            txtNombre.Focus();
        }
    }
}
