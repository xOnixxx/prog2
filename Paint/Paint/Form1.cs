using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Paint
{
    public partial class Form1 : Form
    {
        private bool pressed = false;
        private Point? lastCoords = null;

        private Bitmap image;
        private Graphics graphics;
        private Pen pen;

        public Form1()
        {
            InitializeComponent();

            image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            graphics = Graphics.FromImage(image);
            graphics.FillRectangle(new SolidBrush(Color.White), 0, 0, pictureBox1.Width, pictureBox1.Height);
            pen = new Pen(Color.Black, 6)
            {
                StartCap = System.Drawing.Drawing2D.LineCap.Round,
                EndCap = System.Drawing.Drawing2D.LineCap.Round
            };

            pictureBox1.Image = image;
        }

        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            pressed = true;
            lastCoords = new Point(e.X, e.Y);
        }

        private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (pressed && lastCoords is Point last)
            {
                Point current = new Point(e.X, e.Y);
                graphics.DrawLine(pen, last, current);
                pictureBox1.Refresh();
                lastCoords = current;
            }
        }

        private void PictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            pressed = false;
            lastCoords = null;
        }

        private void PictureBox2_Click(object sender, EventArgs e)
        {
            ColorDialog dialog = new ColorDialog()
            {
                Color = pen.Color
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                pen.Color = dialog.Color;
                (sender as PictureBox).BackColor = dialog.Color;
            }
        }

        private void NumericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            pen.Width = (int)(sender as NumericUpDown).Value;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            graphics.Clear(Color.White);
            pictureBox1.Refresh();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog()
            {
                Filter = "PNG image|*.png",
                Title = "Save an image"
            };

            if (dialog.ShowDialog() == DialogResult.OK && dialog.FileName != "")
            {
                image.Save(dialog.FileName);
            }
        }
    }
}
