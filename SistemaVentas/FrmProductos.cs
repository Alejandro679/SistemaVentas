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
    public partial class FrmProductos : Form
    {
        private readonly string rolUsuario;
        private readonly bool esAdministrador;

        private Panel panelDatos;
        private Panel panelSuperior;
        private DataGridView dgvProductos;

        private TextBox txtCodigo;
        private TextBox txtNombre;
        private TextBox txtCategoria;
        private TextBox txtPrecio;
        private TextBox txtStock;
        private TextBox txtBuscar;

        private Button btnGuardar;
        private Button btnActualizar;
        private Button btnEliminar;
        private Button btnLimpiar;
        private Button btnBuscar;

        private int idProductoSeleccionado = 0;

        public FrmProductos(string rol)
        {
            rolUsuario = (rol ?? "").Trim();
            esAdministrador = rolUsuario.Equals(
                "Administrador",
                StringComparison.OrdinalIgnoreCase);

            ConstruirInterfaz();
            CargarProductos();
        }



        private void ConstruirInterfaz()
        {
            Text = "VENTAS PRO - Gestión de Productos";
            Size = new Size(1200, 750);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(240, 244, 248);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            // ENCABEZADO
            panelSuperior = new Panel();
            panelSuperior.Dock = DockStyle.Top;
            panelSuperior.Height = 75;
            panelSuperior.BackColor = Color.FromArgb(20, 45, 80);
            Controls.Add(panelSuperior);

            Label lblTitulo = new Label();
            lblTitulo.Text = "GESTIÓN DE PRODUCTOS";
            lblTitulo.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            lblTitulo.ForeColor = Color.White;
            lblTitulo.Location = new Point(25, 17);
            lblTitulo.AutoSize = true;
            panelSuperior.Controls.Add(lblTitulo);

            Label lblRol = new Label();
            lblRol.Text = "Usuario: " + rolUsuario;
            lblRol.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblRol.ForeColor = Color.White;
            lblRol.AutoSize = true;
            lblRol.Location = new Point(950, 28);
            panelSuperior.Controls.Add(lblRol);

            // PANEL DE DATOS
            panelDatos = new Panel();
            panelDatos.Location = new Point(20, 100);
            panelDatos.Size = new Size(340, 570);
            panelDatos.BackColor = Color.White;
            panelDatos.BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(panelDatos);

            Label lblDatos = new Label();
            lblDatos.Text = "INFORMACIÓN DEL PRODUCTO";
            lblDatos.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblDatos.ForeColor = Color.FromArgb(20, 45, 80);
            lblDatos.Location = new Point(15, 15);
            lblDatos.AutoSize = true;
            panelDatos.Controls.Add(lblDatos);

            txtCodigo = CrearCampo("Código", 55);
            txtNombre = CrearCampo("Nombre", 135);
            txtCategoria = CrearCampo("Categoría", 215);
            txtPrecio = CrearCampo("Precio", 295);
            txtStock = CrearCampo("Stock", 375);

            // BOTONES SOLO PARA ADMINISTRADOR
            if (esAdministrador)
            {
                btnGuardar = CrearBoton(
                    "GUARDAR", 15, 455,
                    Color.FromArgb(39, 174, 96));

                btnActualizar = CrearBoton(
                    "ACTUALIZAR", 175, 455,
                    Color.FromArgb(41, 128, 185));

                btnEliminar = CrearBoton(
                    "ELIMINAR", 15, 510,
                    Color.FromArgb(192, 57, 43));

                btnLimpiar = CrearBoton(
                    "LIMPIAR", 175, 510,
                    Color.FromArgb(127, 140, 141));

                btnGuardar.Click += BtnGuardar_Click;
                btnActualizar.Click += BtnActualizar_Click;
                btnEliminar.Click += BtnEliminar_Click;
                btnLimpiar.Click += BtnLimpiar_Click;
            }
            else
            {
                Label lblSoloConsulta = new Label();
                lblSoloConsulta.Text = "MODO VENDEDOR\nSolo consulta de productos";
                lblSoloConsulta.Font = new Font(
                    "Segoe UI", 11, FontStyle.Bold);
                lblSoloConsulta.ForeColor = Color.DarkBlue;
                lblSoloConsulta.TextAlign = ContentAlignment.MiddleCenter;
                lblSoloConsulta.Location = new Point(20, 465);
                lblSoloConsulta.Size = new Size(290, 70);
                panelDatos.Controls.Add(lblSoloConsulta);
            }

            // PANEL DE LISTADO
            Panel panelListado = new Panel();
            panelListado.Location = new Point(380, 100);
            panelListado.Size = new Size(790, 570);
            panelListado.BackColor = Color.White;
            panelListado.BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(panelListado);

            Label lblListado = new Label();
            lblListado.Text = "LISTADO DE PRODUCTOS";
            lblListado.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblListado.ForeColor = Color.FromArgb(20, 45, 80);
            lblListado.Location = new Point(15, 15);
            lblListado.AutoSize = true;
            panelListado.Controls.Add(lblListado);

            txtBuscar = new TextBox();
            txtBuscar.Location = new Point(20, 60);
            txtBuscar.Size = new Size(550, 32);
            txtBuscar.Font = new Font("Segoe UI", 11);
            panelListado.Controls.Add(txtBuscar);

            btnBuscar = new Button();
            btnBuscar.Text = "BUSCAR";
            btnBuscar.Location = new Point(590, 57);
            btnBuscar.Size = new Size(150, 38);
            btnBuscar.BackColor = Color.FromArgb(41, 128, 185);
            btnBuscar.ForeColor = Color.White;
            btnBuscar.FlatStyle = FlatStyle.Flat;
            btnBuscar.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnBuscar.Click += BtnBuscar_Click;
            panelListado.Controls.Add(btnBuscar);

            dgvProductos = new DataGridView();
            dgvProductos.Location = new Point(15, 115);
            dgvProductos.Size = new Size(755, 430);
            dgvProductos.BackgroundColor = Color.White;
            dgvProductos.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
            dgvProductos.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dgvProductos.MultiSelect = false;
            dgvProductos.ReadOnly = true;
            dgvProductos.AllowUserToAddRows = false;
            dgvProductos.RowHeadersVisible = false;
            dgvProductos.EnableHeadersVisualStyles = false;

            dgvProductos.ColumnHeadersDefaultCellStyle.BackColor =
                Color.FromArgb(20, 45, 80);
            dgvProductos.ColumnHeadersDefaultCellStyle.ForeColor =
                Color.White;
            dgvProductos.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9, FontStyle.Bold);

            dgvProductos.DefaultCellStyle.Font =
                new Font("Segoe UI", 9);

            dgvProductos.CellClick += DgvProductos_CellClick;

            panelListado.Controls.Add(dgvProductos);

            // BOTÓN CERRAR
            Button btnCerrar = new Button();
            btnCerrar.Text = "CERRAR";
            btnCerrar.Location = new Point(1020, 680);
            btnCerrar.Size = new Size(150, 35);
            btnCerrar.BackColor = Color.FromArgb(20, 45, 80);
            btnCerrar.ForeColor = Color.White;
            btnCerrar.FlatStyle = FlatStyle.Flat;
            btnCerrar.Click += (s, e) => Close();
            Controls.Add(btnCerrar);
        }

        private TextBox CrearCampo(string titulo, int y)
        {
            Label etiqueta = new Label();
            etiqueta.Text = titulo;
            etiqueta.Location = new Point(20, y);
            etiqueta.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            etiqueta.ForeColor = Color.FromArgb(44, 62, 80);
            etiqueta.AutoSize = true;
            panelDatos.Controls.Add(etiqueta);

            TextBox campo = new TextBox();
            campo.Location = new Point(20, y + 25);
            campo.Size = new Size(290, 28);
            campo.Font = new Font("Segoe UI", 10);
            panelDatos.Controls.Add(campo);

            if (!esAdministrador)
                campo.ReadOnly = true;

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

            // IMPORTANTE: agregar al panel de datos
            panelDatos.Controls.Add(boton);

            return boton;
        }

        private void CargarProductos(string filtro = "")
        {
            try
            {
                using (SqlConnection conexion =
                    ConexionBD.ObtenerConexion())
                {
                    conexion.Open();

                    string consulta = @"
                        SELECT IdProducto, Codigo, Nombre,
                               Categoria, Precio, Stock
                        FROM dbo.Productos
                        WHERE Codigo LIKE @filtro
                           OR Nombre LIKE @filtro
                           OR Categoria LIKE @filtro
                        ORDER BY IdProducto DESC";

                    using (SqlCommand comando =
                        new SqlCommand(consulta, conexion))
                    {
                        comando.Parameters.Add(
                            "@filtro", SqlDbType.NVarChar, 150
                        ).Value = "%" + filtro + "%";

                        using (SqlDataAdapter adaptador =
                            new SqlDataAdapter(comando))
                        {
                            DataTable tabla = new DataTable();
                            adaptador.Fill(tabla);
                            dgvProductos.DataSource = tabla;
                        }
                    }
                }

                dgvProductos.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al cargar productos:\n" + ex.Message,
                    "VENTAS PRO",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private bool ValidarDatos()
        {
            if (string.IsNullOrWhiteSpace(txtCodigo.Text) ||
                string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtCategoria.Text) ||
                string.IsNullOrWhiteSpace(txtPrecio.Text) ||
                string.IsNullOrWhiteSpace(txtStock.Text))
            {
                MessageBox.Show(
                    "Completa todos los campos.",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            if (!decimal.TryParse(txtPrecio.Text.Trim(), out decimal precio)
                || precio <= 0)
            {
                MessageBox.Show(
                    "Ingresa un precio válido mayor que cero.");
                txtPrecio.Focus();
                return false;
            }

            if (!int.TryParse(txtStock.Text.Trim(), out int stock)
                || stock < 0)
            {
                MessageBox.Show(
                    "Ingresa una cantidad de stock válida.");
                txtStock.Focus();
                return false;
            }

            return true;
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            if (!esAdministrador) return;
            if (!ValidarDatos()) return;

            try
            {
                using (SqlConnection conexion =
                    ConexionBD.ObtenerConexion())
                {
                    conexion.Open();

                    string consulta = @"
                        INSERT INTO dbo.Productos
                        (Codigo, Nombre, Categoria, Precio, Stock)
                        VALUES
                        (@Codigo, @Nombre, @Categoria, @Precio, @Stock)";

                    using (SqlCommand comando =
                        new SqlCommand(consulta, conexion))
                    {
                        AgregarParametros(comando);
                        comando.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Producto guardado correctamente.");
                LimpiarCampos();
                CargarProductos();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo guardar el producto:\n" + ex.Message);
            }
        }

        private void BtnActualizar_Click(object sender, EventArgs e)
        {
            if (!esAdministrador) return;

            if (idProductoSeleccionado == 0)
            {
                MessageBox.Show("Selecciona un producto de la tabla.");
                return;
            }

            if (!ValidarDatos()) return;

            try
            {
                using (SqlConnection conexion =
                    ConexionBD.ObtenerConexion())
                {
                    conexion.Open();

                    string consulta = @"
                        UPDATE dbo.Productos
                        SET Codigo = @Codigo,
                            Nombre = @Nombre,
                            Categoria = @Categoria,
                            Precio = @Precio,
                            Stock = @Stock
                        WHERE IdProducto = @IdProducto";

                    using (SqlCommand comando =
                        new SqlCommand(consulta, conexion))
                    {
                        AgregarParametros(comando);

                        comando.Parameters.Add(
                            "@IdProducto", SqlDbType.Int
                        ).Value = idProductoSeleccionado;

                        comando.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Producto actualizado correctamente.");
                LimpiarCampos();
                CargarProductos();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo actualizar:\n" + ex.Message);
            }
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (!esAdministrador) return;

            if (idProductoSeleccionado == 0)
            {
                MessageBox.Show("Selecciona un producto de la tabla.");
                return;
            }

            DialogResult respuesta = MessageBox.Show(
                "¿Estás seguro de eliminar este producto?",
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
                        DELETE FROM dbo.Productos
                        WHERE IdProducto = @IdProducto";

                    using (SqlCommand comando =
                        new SqlCommand(consulta, conexion))
                    {
                        comando.Parameters.Add(
                            "@IdProducto", SqlDbType.Int
                        ).Value = idProductoSeleccionado;

                        comando.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Producto eliminado correctamente.");
                LimpiarCampos();
                CargarProductos();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo eliminar el producto:\n" + ex.Message);
            }
        }

        private void BtnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            CargarProductos(txtBuscar.Text.Trim());
        }

        private void DgvProductos_CellClick(
            object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow fila = dgvProductos.Rows[e.RowIndex];

            idProductoSeleccionado =
                Convert.ToInt32(fila.Cells["IdProducto"].Value);

            txtCodigo.Text =
                Convert.ToString(fila.Cells["Codigo"].Value);

            txtNombre.Text =
                Convert.ToString(fila.Cells["Nombre"].Value);

            txtCategoria.Text =
                Convert.ToString(fila.Cells["Categoria"].Value);

            txtPrecio.Text =
                Convert.ToString(fila.Cells["Precio"].Value);

            txtStock.Text =
                Convert.ToString(fila.Cells["Stock"].Value);
        }

        private void AgregarParametros(SqlCommand comando)
        {
            comando.Parameters.Add(
                "@Codigo", SqlDbType.NVarChar, 50
            ).Value = txtCodigo.Text.Trim();

            comando.Parameters.Add(
                "@Nombre", SqlDbType.NVarChar, 150
            ).Value = txtNombre.Text.Trim();

            comando.Parameters.Add(
                "@Categoria", SqlDbType.NVarChar, 100
            ).Value = txtCategoria.Text.Trim();

            comando.Parameters.Add(
                "@Precio", SqlDbType.Decimal
            ).Value = decimal.Parse(txtPrecio.Text.Trim());

            comando.Parameters["@Precio"].Precision = 18;
            comando.Parameters["@Precio"].Scale = 2;

            comando.Parameters.Add(
                "@Stock", SqlDbType.Int
            ).Value = int.Parse(txtStock.Text.Trim());
        }

        private void LimpiarCampos()
        {
            txtCodigo.Clear();
            txtNombre.Clear();
            txtCategoria.Clear();
            txtPrecio.Clear();
            txtStock.Clear();

            idProductoSeleccionado = 0;
            dgvProductos.ClearSelection();
            txtCodigo.Focus();
        }
    }
}
