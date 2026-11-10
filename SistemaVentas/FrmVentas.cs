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
    public partial class FrmVentas : Form
    {
        private readonly int idUsuario;
        private readonly string rolUsuario;

        // IVA de El Salvador: 13%
        private const decimal PORCENTAJE_IVA = 0.13m;

        private DataGridView dgvProductos;
        private DataGridView dgvCarrito;

        private TextBox txtBuscar;
        private TextBox txtCantidad;

        private Label lblSubtotal;
        private Label lblIVA;
        private Label lblTotal;

        private Button btnBuscar;
        private Button btnAgregar;
        private Button btnQuitar;
        private Button btnLimpiar;
        private Button btnGuardar;

        private DataTable carrito;

        private decimal subtotal = 0;
        private decimal iva = 0;
        private decimal total = 0;

        public FrmVentas(string rol, int idUsuario)
        {
            rolUsuario = (rol ?? "").Trim();
            this.idUsuario = idUsuario;

            if (!rolUsuario.Equals(
                "Administrador",
                StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException(
                    "Solamente el administrador puede registrar ventas.");
            }
            InicializarInterfaz();
            CrearCarrito();

            CargarProductos();
            CalcularTotales();
        }
        private void InicializarInterfaz()
        {
            Text = "VENTAS PRO - Registro de Ventas";
            Size = new Size(1250, 790);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(240, 244, 248);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            // ENCABEZADO
            Panel encabezado = new Panel();
            encabezado.Dock = DockStyle.Top;
            encabezado.Height = 70;
            encabezado.BackColor = Color.FromArgb(20, 45, 80);
            Controls.Add(encabezado);

            Label titulo = new Label();
            titulo.Text = "REGISTRO DE VENTAS";
            titulo.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            titulo.ForeColor = Color.White;
            titulo.Location = new Point(25, 15);
            titulo.AutoSize = true;
            encabezado.Controls.Add(titulo);

            Label usuario = new Label();
            usuario.Text = "Administrador";
            usuario.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            usuario.ForeColor = Color.White;
            usuario.AutoSize = true;
            usuario.Location = new Point(1050, 25);
            encabezado.Controls.Add(usuario);

            // PANEL PRODUCTOS
            Panel panelProductos = new Panel();
            panelProductos.Location = new Point(20, 90);
            panelProductos.Size = new Size(575, 490);
            panelProductos.BackColor = Color.White;
            panelProductos.BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(panelProductos);

            Label tituloProductos = new Label();
            tituloProductos.Text = "PRODUCTOS DISPONIBLES";
            tituloProductos.Font = new Font(
                "Segoe UI", 13, FontStyle.Bold);
            tituloProductos.ForeColor = Color.FromArgb(20, 45, 80);
            tituloProductos.Location = new Point(15, 15);
            tituloProductos.AutoSize = true;
            panelProductos.Controls.Add(tituloProductos);

            txtBuscar = new TextBox();
            txtBuscar.Location = new Point(15, 55);
            txtBuscar.Size = new Size(390, 30);
            txtBuscar.Font = new Font("Segoe UI", 10);
            panelProductos.Controls.Add(txtBuscar);

            btnBuscar = CrearBoton(
                "BUSCAR", 420, 52, 130,
                Color.FromArgb(41, 128, 185));
            panelProductos.Controls.Add(btnBuscar);
            btnBuscar.Click += BtnBuscar_Click;

            dgvProductos = new DataGridView();
            dgvProductos.Location = new Point(15, 105);
            dgvProductos.Size = new Size(535, 300);
            ConfigurarTabla(dgvProductos);
            panelProductos.Controls.Add(dgvProductos);

            Label lblCantidad = new Label();
            lblCantidad.Text = "Cantidad:";
            lblCantidad.Location = new Point(15, 425);
            lblCantidad.AutoSize = true;
            lblCantidad.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            panelProductos.Controls.Add(lblCantidad);

            txtCantidad = new TextBox();
            txtCantidad.Location = new Point(100, 420);
            txtCantidad.Size = new Size(100, 30);
            txtCantidad.Font = new Font("Segoe UI", 11);
            txtCantidad.Text = "1";
            panelProductos.Controls.Add(txtCantidad);

            btnAgregar = CrearBoton(
                "AGREGAR AL CARRITO", 220, 415, 330,
                Color.FromArgb(39, 174, 96));
            panelProductos.Controls.Add(btnAgregar);
            btnAgregar.Click += BtnAgregar_Click;

            // PANEL CARRITO
            Panel panelCarrito = new Panel();
            panelCarrito.Location = new Point(615, 90);
            panelCarrito.Size = new Size(605, 490);
            panelCarrito.BackColor = Color.White;
            panelCarrito.BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(panelCarrito);

            Label tituloCarrito = new Label();
            tituloCarrito.Text = "DETALLE DE LA VENTA";
            tituloCarrito.Font = new Font(
                "Segoe UI", 13, FontStyle.Bold);
            tituloCarrito.ForeColor = Color.FromArgb(20, 45, 80);
            tituloCarrito.Location = new Point(15, 15);
            tituloCarrito.AutoSize = true;
            panelCarrito.Controls.Add(tituloCarrito);

            dgvCarrito = new DataGridView();
            dgvCarrito.Location = new Point(15, 55);
            dgvCarrito.Size = new Size(570, 350);
            ConfigurarTabla(dgvCarrito);
            panelCarrito.Controls.Add(dgvCarrito);

            btnQuitar = CrearBoton(
                "QUITAR PRODUCTO", 15, 425, 260,
                Color.FromArgb(192, 57, 43));
            panelCarrito.Controls.Add(btnQuitar);
            btnQuitar.Click += BtnQuitar_Click;

            btnLimpiar = CrearBoton(
                "LIMPIAR CARRITO", 310, 425, 275,
                Color.FromArgb(127, 140, 141));
            panelCarrito.Controls.Add(btnLimpiar);
            btnLimpiar.Click += BtnLimpiar_Click;

            // PANEL TOTALES
            Panel panelTotales = new Panel();
            panelTotales.Location = new Point(615, 595);
            panelTotales.Size = new Size(605, 115);
            panelTotales.BackColor = Color.White;
            panelTotales.BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(panelTotales);

            lblSubtotal = CrearEtiquetaTotal(
                "Subtotal: $0.00", 15, 10);
            panelTotales.Controls.Add(lblSubtotal);

            lblIVA = CrearEtiquetaTotal(
                "IVA (13%): $0.00", 15, 42);
            panelTotales.Controls.Add(lblIVA);

            lblTotal = CrearEtiquetaTotal(
                "TOTAL: $0.00", 15, 74);
            lblTotal.ForeColor = Color.FromArgb(39, 174, 96);
            lblTotal.Font = new Font(
                "Segoe UI", 14, FontStyle.Bold);
            panelTotales.Controls.Add(lblTotal);

            // BOTÓN GUARDAR VENTA
            btnGuardar = CrearBoton(
                "GUARDAR VENTA", 20, 620, 575,
                Color.FromArgb(20, 45, 80));
            btnGuardar.Height = 65;
            btnGuardar.Font = new Font(
                "Segoe UI", 14, FontStyle.Bold);
            Controls.Add(btnGuardar);
            btnGuardar.Click += BtnGuardar_Click;

            Button btnCerrar = CrearBoton(
                "CERRAR", 20, 710, 150,
                Color.FromArgb(127, 140, 141));
            Controls.Add(btnCerrar);
            btnCerrar.Click += (s, e) => Close();
        }

        private void CrearCarrito()
        {
            carrito = new DataTable();

            carrito.Columns.Add("IdProducto", typeof(int));
            carrito.Columns.Add("Codigo", typeof(string));
            carrito.Columns.Add("Nombre", typeof(string));
            carrito.Columns.Add("Cantidad", typeof(int));
            carrito.Columns.Add("Precio", typeof(decimal));
            carrito.Columns.Add("Subtotal", typeof(decimal));

            dgvCarrito.DataSource = carrito;

            dgvCarrito.Columns["IdProducto"].Visible = false;
            dgvCarrito.Columns["Codigo"].HeaderText = "Código";
            dgvCarrito.Columns["Nombre"].HeaderText = "Producto";
            dgvCarrito.Columns["Cantidad"].HeaderText = "Cantidad";
            dgvCarrito.Columns["Precio"].HeaderText = "Precio";
            dgvCarrito.Columns["Subtotal"].HeaderText = "Subtotal";

            dgvCarrito.Columns["Precio"].DefaultCellStyle.Format = "C2";
            dgvCarrito.Columns["Subtotal"].DefaultCellStyle.Format = "C2";
        }

        private void ConfigurarTabla(DataGridView tabla)
        {
            tabla.BackgroundColor = Color.White;
            tabla.BorderStyle = BorderStyle.None;
            tabla.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
            tabla.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            tabla.MultiSelect = false;
            tabla.ReadOnly = true;
            tabla.AllowUserToAddRows = false;
            tabla.RowHeadersVisible = false;
            tabla.EnableHeadersVisualStyles = false;

            tabla.ColumnHeadersDefaultCellStyle.BackColor =
                Color.FromArgb(20, 45, 80);
            tabla.ColumnHeadersDefaultCellStyle.ForeColor =
                Color.White;
            tabla.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9, FontStyle.Bold);

            tabla.DefaultCellStyle.Font =
                new Font("Segoe UI", 9);
        }

        private Button CrearBoton(
            string texto, int x, int y, int ancho, Color color)
        {
            Button boton = new Button();
            boton.Text = texto;
            boton.Location = new Point(x, y);
            boton.Size = new Size(ancho, 38);
            boton.BackColor = color;
            boton.ForeColor = Color.White;
            boton.FlatStyle = FlatStyle.Flat;
            boton.FlatAppearance.BorderSize = 0;
            boton.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            boton.Cursor = Cursors.Hand;

            return boton;
        }

        private Label CrearEtiquetaTotal(
            string texto, int x, int y)
        {
            Label etiqueta = new Label();
            etiqueta.Text = texto;
            etiqueta.Location = new Point(x, y);
            etiqueta.AutoSize = true;
            etiqueta.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            etiqueta.ForeColor = Color.FromArgb(44, 62, 80);

            return etiqueta;
        }

        // CARGAR PRODUCTOS
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
                        WHERE Stock > 0
                          AND (Codigo LIKE @filtro
                           OR Nombre LIKE @filtro
                           OR Categoria LIKE @filtro)
                        ORDER BY Nombre";

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

                if (dgvProductos.Columns.Contains("Precio"))
                    dgvProductos.Columns["Precio"]
                        .DefaultCellStyle.Format = "C2";
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

        // BUSCAR PRODUCTOS
        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            CargarProductos(txtBuscar.Text.Trim());
        }

        // AGREGAR PRODUCTO AL CARRITO
        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            if (dgvProductos.CurrentRow == null)
            {
                MessageBox.Show("Selecciona un producto.");
                return;
            }

            if (!int.TryParse(txtCantidad.Text.Trim(), out int cantidad)
                || cantidad <= 0)
            {
                MessageBox.Show("Ingresa una cantidad válida.");
                txtCantidad.Focus();
                return;
            }

            DataGridViewRow fila = dgvProductos.CurrentRow;

            int idProducto = Convert.ToInt32(
                fila.Cells["IdProducto"].Value);

            string codigo = Convert.ToString(
                fila.Cells["Codigo"].Value);

            string nombre = Convert.ToString(
                fila.Cells["Nombre"].Value);

            decimal precio = Convert.ToDecimal(
                fila.Cells["Precio"].Value);

            int stock = Convert.ToInt32(
                fila.Cells["Stock"].Value);

            if (cantidad > stock)
            {
                MessageBox.Show(
                    "No hay suficiente existencia.\n" +
                    "Stock disponible: " + stock);
                return;
            }

            foreach (DataRow producto in carrito.Rows)
            {
                if (Convert.ToInt32(producto["IdProducto"])
                    == idProducto)
                {
                    int nuevaCantidad =
                        Convert.ToInt32(producto["Cantidad"])
                        + cantidad;

                    if (nuevaCantidad > stock)
                    {
                        MessageBox.Show(
                            "La cantidad supera el stock disponible.");
                        return;
                    }

                    producto["Cantidad"] = nuevaCantidad;
                    producto["Subtotal"] = nuevaCantidad * precio;

                    CalcularTotales();
                    txtCantidad.Text = "1";
                    return;
                }
            }

            decimal importe = cantidad * precio;

            carrito.Rows.Add(
                idProducto,
                codigo,
                nombre,
                cantidad,
                precio,
                importe);

            CalcularTotales();
            txtCantidad.Text = "1";
        }

        // QUITAR PRODUCTO
        private void BtnQuitar_Click(object sender, EventArgs e)
        {
            if (dgvCarrito.CurrentRow == null ||
                dgvCarrito.CurrentRow.IsNewRow)
            {
                MessageBox.Show("Selecciona un producto del carrito.");
                return;
            }

            int indice = dgvCarrito.CurrentRow.Index;

            if (indice >= 0 && indice < carrito.Rows.Count)
            {
                carrito.Rows.RemoveAt(indice);
                CalcularTotales();
            }
        }

        // LIMPIAR CARRITO
        private void BtnLimpiar_Click(object sender, EventArgs e)
        {
            if (carrito.Rows.Count == 0)
                return;

            DialogResult respuesta = MessageBox.Show(
                "¿Deseas quitar todos los productos?",
                "Confirmar",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (respuesta == DialogResult.Yes)
            {
                carrito.Clear();
                CalcularTotales();
            }
        }

        // CALCULAR SUBTOTAL, IVA Y TOTAL
        private void CalcularTotales()
        {
            subtotal = 0;

            foreach (DataRow fila in carrito.Rows)
            {
                subtotal += Convert.ToDecimal(fila["Subtotal"]);
            }

            iva = Math.Round(
                subtotal * PORCENTAJE_IVA, 2);

            total = subtotal + iva;

            lblSubtotal.Text =
                "Subtotal: $" + subtotal.ToString("N2");

            lblIVA.Text =
                "IVA (13%): $" + iva.ToString("N2");

            lblTotal.Text =
                "TOTAL: $" + total.ToString("N2");
        }

        // GUARDAR VENTA Y DETALLES EN UNA TRANSACCIÓN
        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            if (carrito.Rows.Count == 0)
            {
                MessageBox.Show(
                    "Agrega al menos un producto a la venta.");
                return;
            }

            DialogResult respuesta = MessageBox.Show(
                "¿Deseas guardar esta venta?\n\n" +
                "Total: $" + total.ToString("N2"),
                "Confirmar venta",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (respuesta != DialogResult.Yes)
                return;

            using (SqlConnection conexion =
                ConexionBD.ObtenerConexion())
            {
                conexion.Open();

                using (SqlTransaction transaccion =
                    conexion.BeginTransaction())
                {
                    try
                    {
                        string sqlVenta = @"
                            INSERT INTO dbo.Ventas
                            (Fecha, IdUsuario, Subtotal, IVA, Total)
                            OUTPUT INSERTED.IdVenta
                            VALUES
                            (@Fecha, @IdUsuario, @Subtotal, @IVA, @Total)";

                        int idVenta;

                        using (SqlCommand comando =
                            new SqlCommand(
                                sqlVenta, conexion, transaccion))
                        {
                            comando.Parameters.Add(
                                "@Fecha", SqlDbType.DateTime
                            ).Value = DateTime.Now;

                            comando.Parameters.Add(
                                "@IdUsuario", SqlDbType.Int
                            ).Value = idUsuario;

                            comando.Parameters.Add(
                                "@Subtotal", SqlDbType.Decimal
                            ).Value = subtotal;

                            comando.Parameters.Add(
                                "@IVA", SqlDbType.Decimal
                            ).Value = iva;

                            comando.Parameters.Add(
                                "@Total", SqlDbType.Decimal
                            ).Value = total;

                            comando.Parameters["@Subtotal"].Precision = 10;
                            comando.Parameters["@Subtotal"].Scale = 2;

                            comando.Parameters["@IVA"].Precision = 10;
                            comando.Parameters["@IVA"].Scale = 2;

                            comando.Parameters["@Total"].Precision = 10;
                            comando.Parameters["@Total"].Scale = 2;

                            idVenta = Convert.ToInt32(
                                comando.ExecuteScalar());
                        }

                        foreach (DataRow producto in carrito.Rows)
                        {
                            int idProducto = Convert.ToInt32(
                                producto["IdProducto"]);

                            int cantidad = Convert.ToInt32(
                                producto["Cantidad"]);

                            decimal precio = Convert.ToDecimal(
                                producto["Precio"]);

                            decimal importe = Convert.ToDecimal(
                                producto["Subtotal"]);

                            // Verificar y descontar inventario
                            string sqlStock = @"
                                UPDATE dbo.Productos
                                SET Stock = Stock - @Cantidad
                                WHERE IdProducto = @IdProducto
                                  AND Stock >= @Cantidad";

                            using (SqlCommand comandoStock =
                                new SqlCommand(
                                    sqlStock, conexion, transaccion))
                            {
                                comandoStock.Parameters.Add(
                                    "@Cantidad", SqlDbType.Int
                                ).Value = cantidad;

                                comandoStock.Parameters.Add(
                                    "@IdProducto", SqlDbType.Int
                                ).Value = idProducto;

                                int filas = comandoStock.ExecuteNonQuery();

                                if (filas != 1)
                                {
                                    throw new Exception(
                                        "No hay stock suficiente para el producto: "
                                        + producto["Nombre"]);
                                }
                            }

                            // Guardar detalle de venta
                            string sqlDetalle = @"
                                INSERT INTO dbo.DetalleVenta
                                (IdVenta, IdProducto, Cantidad,
                                 Precio, Subtotal)
                                VALUES
                                (@IdVenta, @IdProducto, @Cantidad,
                                 @Precio, @Subtotal)";

                            using (SqlCommand comandoDetalle =
                                new SqlCommand(
                                    sqlDetalle, conexion, transaccion))
                            {
                                comandoDetalle.Parameters.Add(
                                    "@IdVenta", SqlDbType.Int
                                ).Value = idVenta;

                                comandoDetalle.Parameters.Add(
                                    "@IdProducto", SqlDbType.Int
                                ).Value = idProducto;

                                comandoDetalle.Parameters.Add(
                                    "@Cantidad", SqlDbType.Int
                                ).Value = cantidad;

                                comandoDetalle.Parameters.Add(
                                    "@Precio", SqlDbType.Decimal
                                ).Value = precio;

                                comandoDetalle.Parameters.Add(
                                    "@Subtotal", SqlDbType.Decimal
                                ).Value = importe;

                                comandoDetalle.Parameters["@Precio"].Precision = 10;
                                comandoDetalle.Parameters["@Precio"].Scale = 2;

                                comandoDetalle.Parameters["@Subtotal"].Precision = 10;
                                comandoDetalle.Parameters["@Subtotal"].Scale = 2;

                                comandoDetalle.ExecuteNonQuery();
                            }
                        }

                        transaccion.Commit();

                        MessageBox.Show(
                            "¡Venta registrada correctamente!\n\n" +
                            "Número de venta: " + idVenta +
                            "\nTotal: $" + total.ToString("N2"),
                            "VENTAS PRO",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                        carrito.Clear();
                        CalcularTotales();
                        CargarProductos();
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            transaccion.Rollback();
                        }
                        catch
                        {
                            // La transacción pudo haberse revertido
                            // automáticamente o estar cerrada.
                        }

                        MessageBox.Show(
                            "No se pudo registrar la venta.\n\n" +
                            ex.Message,
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
