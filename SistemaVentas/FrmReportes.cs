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
    public partial class FrmReportes : Form
    {
        private readonly string rolUsuario;

        private DateTimePicker dtpDesde;
        private DateTimePicker dtpHasta;

        private DataGridView dgvReportes;

        private Label lblCantidadVentas;
        private Label lblSubtotal;
        private Label lblIVA;
        private Label lblTotal;

        private Button btnConsultar;
        private Button btnExportar;
        private Button btnLimpiar;
        private Button btnCerrar;

        private DataTable datosReporte;

        public FrmReportes(string rol)
        {
            rolUsuario = (rol ?? "").Trim();

            if (!rolUsuario.Equals(
                "Administrador",
                StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException(
                    "No tienes permiso para acceder a los reportes.");
            }

            ConstruirInterfaz();
            ConsultarReportes();
        }
        private void ConstruirInterfaz()
        {
            Text = "VENTAS PRO - Reportes";
            Size = new Size(1250, 780);
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
            titulo.Text = "REPORTES Y ESTADÍSTICAS";
            titulo.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            titulo.ForeColor = Color.White;
            titulo.Location = new Point(25, 18);
            titulo.AutoSize = true;
            encabezado.Controls.Add(titulo);

            Label usuario = new Label();
            usuario.Text = "ADMINISTRADOR";
            usuario.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            usuario.ForeColor = Color.White;
            usuario.AutoSize = true;
            usuario.Location = new Point(1030, 28);
            encabezado.Controls.Add(usuario);

            // PANEL DE FILTROS
            Panel panelFiltros = new Panel();
            panelFiltros.Location = new Point(20, 95);
            panelFiltros.Size = new Size(1190, 100);
            panelFiltros.BackColor = Color.White;
            panelFiltros.BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(panelFiltros);

            Label lblDesde = new Label();
            lblDesde.Text = "Fecha desde:";
            lblDesde.Location = new Point(20, 15);
            lblDesde.AutoSize = true;
            lblDesde.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            panelFiltros.Controls.Add(lblDesde);

            dtpDesde = new DateTimePicker();
            dtpDesde.Location = new Point(20, 43);
            dtpDesde.Size = new Size(220, 30);
            dtpDesde.Font = new Font("Segoe UI", 10);
            dtpDesde.Format = DateTimePickerFormat.Short;
            dtpDesde.Value = new DateTime(
                DateTime.Today.Year, 1, 1);
            panelFiltros.Controls.Add(dtpDesde);

            Label lblHasta = new Label();
            lblHasta.Text = "Fecha hasta:";
            lblHasta.Location = new Point(270, 15);
            lblHasta.AutoSize = true;
            lblHasta.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            panelFiltros.Controls.Add(lblHasta);

            dtpHasta = new DateTimePicker();
            dtpHasta.Location = new Point(270, 43);
            dtpHasta.Size = new Size(220, 30);
            dtpHasta.Font = new Font("Segoe UI", 10);
            dtpHasta.Format = DateTimePickerFormat.Short;
            dtpHasta.Value = DateTime.Today;
            panelFiltros.Controls.Add(dtpHasta);

            btnConsultar = CrearBoton(
                "CONSULTAR", 530, 38, 180,
                Color.FromArgb(41, 128, 185));

            panelFiltros.Controls.Add(btnConsultar);
            btnConsultar.Click += BtnConsultar_Click;

            btnLimpiar = CrearBoton(
                "HOY", 730, 38, 130,
                Color.FromArgb(127, 140, 141));

            panelFiltros.Controls.Add(btnLimpiar);
            btnLimpiar.Click += BtnLimpiar_Click;

            // PANEL DE INDICADORES
            Panel panelIndicadores = new Panel();
            panelIndicadores.Location = new Point(20, 210);
            panelIndicadores.Size = new Size(1190, 125);
            panelIndicadores.BackColor = Color.Transparent;
            Controls.Add(panelIndicadores);

            CrearTarjeta(
                panelIndicadores,
                "VENTAS REALIZADAS",
                0,
                Color.FromArgb(41, 128, 185),
                out lblCantidadVentas);

            CrearTarjeta(
                panelIndicadores,
                "SUBTOTAL",
                300,
                Color.FromArgb(39, 174, 96),
                out lblSubtotal);

            CrearTarjeta(
                panelIndicadores,
                "IVA",
                600,
                Color.FromArgb(142, 68, 173),
                out lblIVA);

            CrearTarjeta(
                panelIndicadores,
                "INGRESOS TOTALES",
                900,
                Color.FromArgb(20, 45, 80),
                out lblTotal);

            // PANEL DE TABLA
            Panel panelTabla = new Panel();
            panelTabla.Location = new Point(20, 350);
            panelTabla.Size = new Size(1190, 310);
            panelTabla.BackColor = Color.White;
            panelTabla.BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(panelTabla);

            Label lblTabla = new Label();
            lblTabla.Text = "DETALLE DE VENTAS";
            lblTabla.Location = new Point(15, 15);
            lblTabla.AutoSize = true;
            lblTabla.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            lblTabla.ForeColor = Color.FromArgb(20, 45, 80);
            panelTabla.Controls.Add(lblTabla);

            dgvReportes = new DataGridView();
            dgvReportes.Location = new Point(15, 55);
            dgvReportes.Size = new Size(1155, 235);

            dgvReportes.BackgroundColor = Color.White;
            dgvReportes.BorderStyle = BorderStyle.None;
            dgvReportes.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
            dgvReportes.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dgvReportes.MultiSelect = false;
            dgvReportes.ReadOnly = true;
            dgvReportes.AllowUserToAddRows = false;
            dgvReportes.RowHeadersVisible = false;
            dgvReportes.EnableHeadersVisualStyles = false;

            dgvReportes.ColumnHeadersDefaultCellStyle.BackColor =
                Color.FromArgb(20, 45, 80);

            dgvReportes.ColumnHeadersDefaultCellStyle.ForeColor =
                Color.White;

            dgvReportes.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9, FontStyle.Bold);

            dgvReportes.DefaultCellStyle.Font =
                new Font("Segoe UI", 9);

            panelTabla.Controls.Add(dgvReportes);

            // BOTONES INFERIORES
            btnExportar = CrearBoton(
                "EXPORTAR A EXCEL (CSV)",
                750, 680, 230,
                Color.FromArgb(39, 174, 96));

            Controls.Add(btnExportar);
            btnExportar.Click += BtnExportar_Click;

            btnCerrar = CrearBoton(
                "CERRAR", 1030, 680, 180,
                Color.FromArgb(20, 45, 80));

            Controls.Add(btnCerrar);
            btnCerrar.Click += (s, e) => Close();
        }

        private void CrearTarjeta(
            Panel contenedor,
            string titulo,
            int x,
            Color color,
            out Label valor)
        {
            Panel tarjeta = new Panel();
            tarjeta.Location = new Point(x, 0);
            tarjeta.Size = new Size(280, 110);
            tarjeta.BackColor = color;
            contenedor.Controls.Add(tarjeta);

            Label lblTitulo = new Label();
            lblTitulo.Text = titulo;
            lblTitulo.Font = new Font(
                "Segoe UI", 10, FontStyle.Bold);
            lblTitulo.ForeColor = Color.White;
            lblTitulo.Location = new Point(15, 15);
            lblTitulo.AutoSize = true;
            tarjeta.Controls.Add(lblTitulo);

            valor = new Label();
            valor.Text = "0";
            valor.Font = new Font(
                "Segoe UI", 19, FontStyle.Bold);
            valor.ForeColor = Color.White;
            valor.Location = new Point(15, 48);
            valor.AutoSize = true;
            tarjeta.Controls.Add(valor);
        }

        private Button CrearBoton(
            string texto,
            int x,
            int y,
            int ancho,
            Color color)
        {
            Button boton = new Button();
            boton.Text = texto;
            boton.Location = new Point(x, y);
            boton.Size = new Size(ancho, 40);
            boton.BackColor = color;
            boton.ForeColor = Color.White;
            boton.FlatStyle = FlatStyle.Flat;
            boton.FlatAppearance.BorderSize = 0;
            boton.Font = new Font(
                "Segoe UI", 9, FontStyle.Bold);
            boton.Cursor = Cursors.Hand;

            return boton;
        }

        // CONSULTAR REPORTES
        private void ConsultarReportes()
        {
            if (dtpDesde.Value.Date > dtpHasta.Value.Date)
            {
                MessageBox.Show(
                    "La fecha inicial no puede ser mayor que la fecha final.");
                return;
            }

            try
            {
                using (SqlConnection conexion =
                    ConexionBD.ObtenerConexion())
                {
                    conexion.Open();

                    string consulta = @"
                        SELECT
                            V.IdVenta,
                            V.Fecha,
                            U.Nombre AS Usuario,
                            COUNT(D.IdDetalle) AS Productos,
                            ISNULL(SUM(D.Cantidad), 0) AS Unidades,
                            V.Subtotal,
                            V.IVA,
                            V.Total
                        FROM dbo.Ventas V
                        INNER JOIN dbo.Usuarios U
                            ON V.IdUsuario = U.IdUsuario
                        LEFT JOIN dbo.DetalleVenta D
                            ON V.IdVenta = D.IdVenta
                        WHERE V.Fecha >= @Desde
                          AND V.Fecha < @Hasta
                        GROUP BY
                            V.IdVenta,
                            V.Fecha,
                            U.Nombre,
                            V.Subtotal,
                            V.IVA,
                            V.Total
                        ORDER BY V.Fecha DESC";

                    using (SqlCommand comando =
                        new SqlCommand(consulta, conexion))
                    {
                        comando.Parameters.Add(
                            "@Desde", SqlDbType.DateTime
                        ).Value = dtpDesde.Value.Date;

                        comando.Parameters.Add(
                            "@Hasta", SqlDbType.DateTime
                        ).Value = dtpHasta.Value.Date.AddDays(1);

                        using (SqlDataAdapter adaptador =
                            new SqlDataAdapter(comando))
                        {
                            datosReporte = new DataTable();
                            adaptador.Fill(datosReporte);
                        }
                    }
                }

                dgvReportes.DataSource = datosReporte;

                FormatearColumnas();
                CalcularResumen();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al consultar reportes:\n" + ex.Message,
                    "VENTAS PRO",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // FORMATO DE COLUMNAS
        private void FormatearColumnas()
        {
            if (dgvReportes.Columns.Count == 0)
                return;

            dgvReportes.Columns["IdVenta"].HeaderText = "N° Venta";
            dgvReportes.Columns["Fecha"].HeaderText = "Fecha";
            dgvReportes.Columns["Usuario"].HeaderText = "Vendedor";
            dgvReportes.Columns["Productos"].HeaderText = "Productos";
            dgvReportes.Columns["Unidades"].HeaderText = "Unidades";
            dgvReportes.Columns["Subtotal"].HeaderText = "Subtotal";
            dgvReportes.Columns["IVA"].HeaderText = "IVA";
            dgvReportes.Columns["Total"].HeaderText = "Total";

            dgvReportes.Columns["Fecha"]
                .DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";

            dgvReportes.Columns["Subtotal"]
                .DefaultCellStyle.Format = "C2";

            dgvReportes.Columns["IVA"]
                .DefaultCellStyle.Format = "C2";

            dgvReportes.Columns["Total"]
                .DefaultCellStyle.Format = "C2";
        }

        // CALCULAR INDICADORES
        private void CalcularResumen()
        {
            int cantidadVentas = datosReporte.Rows.Count;

            decimal sumaSubtotal = 0;
            decimal sumaIVA = 0;
            decimal sumaTotal = 0;

            foreach (DataRow fila in datosReporte.Rows)
            {
                sumaSubtotal += Convert.ToDecimal(fila["Subtotal"]);
                sumaIVA += Convert.ToDecimal(fila["IVA"]);
                sumaTotal += Convert.ToDecimal(fila["Total"]);
            }

            lblCantidadVentas.Text =
                cantidadVentas.ToString();

            lblSubtotal.Text =
                "$" + sumaSubtotal.ToString("N2");

            lblIVA.Text =
                "$" + sumaIVA.ToString("N2");

            lblTotal.Text =
                "$" + sumaTotal.ToString("N2");
        }

        // BOTÓN CONSULTAR
        private void BtnConsultar_Click(
            object sender, EventArgs e)
        {
            ConsultarReportes();
        }

        // MOSTRAR VENTAS DE HOY
        private void BtnLimpiar_Click(
            object sender, EventArgs e)
        {
            dtpDesde.Value = DateTime.Today;
            dtpHasta.Value = DateTime.Today;

            ConsultarReportes();
        }

        // EXPORTAR CSV
        private void BtnExportar_Click(
            object sender, EventArgs e)
        {
            if (datosReporte == null ||
                datosReporte.Rows.Count == 0)
            {
                MessageBox.Show(
                    "No hay información para exportar.");
                return;
            }

            using (SaveFileDialog guardar =
                new SaveFileDialog())
            {
                guardar.Title = "Guardar reporte de ventas";
                guardar.Filter = "Archivo CSV (*.csv)|*.csv";
                guardar.FileName =
                    "Reporte_Ventas_" +
                    DateTime.Now.ToString("yyyyMMdd_HHmm") +
                    ".csv";

                if (guardar.ShowDialog() != DialogResult.OK)
                    return;

                try
                {
                    StringBuilder contenido = new StringBuilder();

                    // Encabezados
                    for (int i = 0; i < datosReporte.Columns.Count; i++)
                    {
                        contenido.Append(
                            EscaparCSV(datosReporte.Columns[i].ColumnName));

                        if (i < datosReporte.Columns.Count - 1)
                            contenido.Append(";");
                    }

                    contenido.AppendLine();

                    // Registros
                    foreach (DataRow fila in datosReporte.Rows)
                    {
                        for (int i = 0; i < datosReporte.Columns.Count; i++)
                        {
                            string valor = Convert.ToString(fila[i]);

                            contenido.Append(EscaparCSV(valor));

                            if (i < datosReporte.Columns.Count - 1)
                                contenido.Append(";");
                        }

                        contenido.AppendLine();
                    }

                    File.WriteAllText(
                        guardar.FileName,
                        contenido.ToString(),
                        Encoding.UTF8);

                    MessageBox.Show(
                        "Reporte exportado correctamente:\n" +
                        guardar.FileName,
                        "VENTAS PRO",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Error al exportar:\n" + ex.Message);
                }
            }
        }

        private string EscaparCSV(string valor)
        {
            return "\"" +
                (valor ?? "").Replace("\"", "\"\"") +
                "\"";
        }
    }
}
