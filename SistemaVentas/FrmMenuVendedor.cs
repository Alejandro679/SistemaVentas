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
    public partial class FrmMenuVendedor : Form
    {
        private readonly Color azulOscuro =
            Color.FromArgb(15, 32, 60);

        private Panel contenido;

        public FrmMenuVendedor(string nombre, int idUsuario)
        {
            ConstruirMenu(nombre, idUsuario);
            
        }
        private void ConstruirMenu(string nombre, int idUsuario)
        {
            Text = "VENTAS PRO | Vendedor";
            Size = new Size(1100, 680);
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(950, 600);
            BackColor = Color.FromArgb(244, 247, 252);

            // MENU LATERAL
            Panel lateral = new Panel();
            lateral.Dock = DockStyle.Left;
            lateral.Width = 235;
            lateral.BackColor = azulOscuro;

            Controls.Add(lateral);

            Label logo = new Label();
            logo.Text = "VENTAS PRO";
            logo.Font = new Font(
                "Segoe UI", 21, FontStyle.Bold);
            logo.ForeColor = Color.White;
            logo.Location = new Point(20, 30);
            logo.AutoSize = true;

            lateral.Controls.Add(logo);

            Label rol = new Label();
            rol.Text = "PANEL DEL VENDEDOR";
            rol.ForeColor = Color.LightBlue;
            rol.Font = new Font(
                "Segoe UI", 8, FontStyle.Bold);
            rol.Location = new Point(23, 75);
            rol.AutoSize = true;

            lateral.Controls.Add(rol);

            Button btnProductos = new Button();
            btnProductos.Text = "▣   Consultar productos";
            btnProductos.Location = new Point(15, 140);
            btnProductos.Size = new Size(205, 50);
            btnProductos.BackColor = Color.FromArgb(35, 115, 210);
            btnProductos.ForeColor = Color.White;
            btnProductos.FlatStyle = FlatStyle.Flat;
            btnProductos.FlatAppearance.BorderSize = 0;
            btnProductos.Font = new Font(
                "Segoe UI", 10, FontStyle.Bold);
            btnProductos.TextAlign = ContentAlignment.MiddleLeft;
            btnProductos.Padding = new Padding(10, 0, 0, 0);
            btnProductos.Cursor = Cursors.Hand;

            btnProductos.Click += (s, e) =>
                AbrirProductos();

            lateral.Controls.Add(btnProductos);

            Button btnCerrar = new Button();
            btnCerrar.Text = "↪   Cerrar sesión";
            btnCerrar.Location = new Point(15, 560);
            btnCerrar.Size = new Size(205, 45);
            btnCerrar.BackColor = Color.FromArgb(190, 55, 65);
            btnCerrar.ForeColor = Color.White;
            btnCerrar.FlatStyle = FlatStyle.Flat;
            btnCerrar.FlatAppearance.BorderSize = 0;
            btnCerrar.Font = new Font(
                "Segoe UI", 10, FontStyle.Bold);
            btnCerrar.Cursor = Cursors.Hand;

            btnCerrar.Click += (s, e) => Close();

            lateral.Controls.Add(btnCerrar);

            // CONTENIDO
            contenido = new Panel();
            contenido.Dock = DockStyle.Fill;
            contenido.BackColor =
                Color.FromArgb(244, 247, 252);

            Controls.Add(contenido);

            Panel superior = new Panel();
            superior.Dock = DockStyle.Top;
            superior.Height = 85;
            superior.BackColor = Color.White;

            contenido.Controls.Add(superior);

            Label titulo = new Label();
            titulo.Text = "Consulta de inventario";
            titulo.Font = new Font(
                "Segoe UI", 20, FontStyle.Bold);
            titulo.ForeColor = azulOscuro;
            titulo.Location = new Point(30, 12);
            titulo.AutoSize = true;

            superior.Controls.Add(titulo);

            Label usuario = new Label();
            usuario.Text = "Vendedor: " + nombre;
            usuario.Font = new Font(
                "Segoe UI", 10, FontStyle.Bold);
            usuario.ForeColor =
                Color.FromArgb(35, 115, 210);
            usuario.Location = new Point(30, 52);
            usuario.AutoSize = true;

            superior.Controls.Add(usuario);

            Label bienvenida = new Label();
            bienvenida.Text = "¡Bienvenido!";
            bienvenida.Font = new Font(
                "Segoe UI", 24, FontStyle.Bold);
            bienvenida.ForeColor = azulOscuro;
            bienvenida.Location = new Point(45, 150);
            bienvenida.AutoSize = true;

            contenido.Controls.Add(bienvenida);

            Label descripcion = new Label();
            descripcion.Text =
                "Desde aquí puedes consultar los productos\n" +
                "disponibles y verificar sus existencias.";
            descripcion.Font = new Font("Segoe UI", 12);
            descripcion.ForeColor = Color.Gray;
            descripcion.Location = new Point(48, 205);
            descripcion.AutoSize = true;

            contenido.Controls.Add(descripcion);

            Panel tarjeta = new Panel();
            tarjeta.Location = new Point(45, 300);
            tarjeta.Size = new Size(400, 155);
            tarjeta.BackColor = Color.White;

            contenido.Controls.Add(tarjeta);

            Label icono = new Label();
            icono.Text = "📦";
            icono.Font = new Font("Segoe UI Emoji", 35);
            icono.Location = new Point(25, 35);
            icono.AutoSize = true;

            tarjeta.Controls.Add(icono);

            Label productoTitulo = new Label();
            productoTitulo.Text = "CATÁLOGO DE PRODUCTOS";
            productoTitulo.Font = new Font(
                "Segoe UI", 13, FontStyle.Bold);
            productoTitulo.ForeColor = azulOscuro;
            productoTitulo.Location = new Point(110, 35);
            productoTitulo.AutoSize = true;

            tarjeta.Controls.Add(productoTitulo);

            Label productoTexto = new Label();
            productoTexto.Text = "Consulta precios y existencias.";
            productoTexto.ForeColor = Color.Gray;
            productoTexto.Location = new Point(110, 75);
            productoTexto.AutoSize = true;

            tarjeta.Controls.Add(productoTexto);

            Button consultar = new Button();
            consultar.Text = "VER PRODUCTOS";
            consultar.Location = new Point(110, 105);
            consultar.Size = new Size(175, 35);
            consultar.BackColor =
                Color.FromArgb(35, 115, 210);
            consultar.ForeColor = Color.White;
            consultar.FlatStyle = FlatStyle.Flat;
            consultar.FlatAppearance.BorderSize = 0;
            consultar.Click += (s, e) => AbrirProductos();

            tarjeta.Controls.Add(consultar);
        }

        private void AbrirProductos()
        {
            using (FrmProductos formulario = new FrmProductos("Vendedor"))
            {
                formulario.ShowDialog(this);
            }
        }
    }
}
