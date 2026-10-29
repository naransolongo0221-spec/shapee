using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.Versioning;

namespace shapee
{
    // 1. Үндсэн абстракт класс (Base Class)
    public abstract class Shape
    {
        public Point Location { get; set; }
        public abstract void Draw(Graphics g);
    }

    // 2. Дэд класс: Дугуй (Circle)
    public class Circle : Shape
    {
        public int Radius { get; set; } = 20;

        public override void Draw(Graphics g)
        {
            using (Brush brush = new SolidBrush(Color.Red))
            using (Pen pen = new Pen(Color.Black, 2))
            {
                g.FillEllipse(brush, Location.X - Radius, Location.Y - Radius, Radius * 2, Radius * 2);
                g.DrawEllipse(pen, Location.X - Radius, Location.Y - Radius, Radius * 2, Radius * 2);
            }
        }
    }

    // 3. Дэд класс: Тэгш өнцөгт (Rectangle)
    public class MyRectangle : Shape
    {
        public int Width { get; set; } = 60;
        public int Height { get; set; } = 40;

        public override void Draw(Graphics g)
        {
            using (Brush brush = new SolidBrush(Color.Blue))
            using (Pen pen = new Pen(Color.Black, 2))
            {
                g.FillRectangle(brush, Location.X - Width / 2, Location.Y - Height / 2, Width, Height);
                g.DrawRectangle(pen, Location.X - Width / 2, Location.Y - Height / 2, Width, Height);
            }
        }
    }

    // 4. Дэд класс: Цэгүүдийг холбосон хар шугам болон улаан цэгүүд (Polyline)
    public class MyPolyLine : Shape
    {
        public List<Point> Points { get; set; } = new List<Point>();

        public override void Draw(Graphics g)
        {
            // 1. Цэгүүдийг хар өнгийн шугамаар холбож зурах
            if (Points.Count > 1)
            {
                using (Pen pen = new Pen(Color.Black, 2))
                {
                    g.DrawLines(pen, Points.ToArray());
                }
            }

            // 2. Цэг тус бүр дээр тод улаан өнгийн жижиг дугуй дүрслэх
            int pointRadius = 4;
            using (Brush redBrush = new SolidBrush(Color.Red))
            using (Pen blackPen = new Pen(Color.Black, 1))
            {
                foreach (var pt in Points)
                {
                    g.FillEllipse(redBrush, pt.X - pointRadius, pt.Y - pointRadius, pointRadius * 2, pointRadius * 2);
                    g.DrawEllipse(blackPen, pt.X - pointRadius, pt.Y - pointRadius, pointRadius * 2, pointRadius * 2);
                }
            }
        }
    }

    [SupportedOSPlatform("windows")]
    public partial class Form1 : Form
    {
        private List<Shape> shapes = new List<Shape>();
        private string currentShapeType = "Circle"; // "Circle", "Rectangle", "Line"

        // Шулуун горимд цуглуулах цэгүүдийн жагсаалт
        private MyPolyLine? currentPolyLine = null;

        private Button? btnCircle;
        private Button? btnRectangle;
        private Button? btnLine;
        private Button? btnClear;

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.Width = 850;
            this.Height = 550;

            CreateUIButtons();

            this.MouseClick += new MouseEventHandler(this.Form1_MouseClick);
            this.KeyDown += new KeyEventHandler(this.Form1_KeyDown);
            this.KeyPreview = true;
        }

        private void CreateUIButtons()
        {
            btnCircle = new Button();
            btnCircle.Text = "Дугуй";
            btnCircle.Location = new Point(15, 12);
            btnCircle.Size = new Size(90, 30);
            btnCircle.BackColor = Color.LightCoral;
            btnCircle.Click += (s, e) => {
                currentShapeType = "Circle";
                currentPolyLine = null;
            };
            this.Controls.Add(btnCircle);

            btnRectangle = new Button();
            btnRectangle.Text = "Тэгш өнцөгт";
            btnRectangle.Location = new Point(115, 12);
            btnRectangle.Size = new Size(100, 30);
            btnRectangle.BackColor = Color.LightSkyBlue;
            btnRectangle.Click += (s, e) => {
                currentShapeType = "Rectangle";
                currentPolyLine = null;
            };
            this.Controls.Add(btnRectangle);

            btnLine = new Button();
            btnLine.Text = "Шулуун";
            btnLine.Location = new Point(225, 12);
            btnLine.Size = new Size(90, 30);
            btnLine.BackColor = Color.LightGreen;
            btnLine.Click += (s, e) => {
                currentShapeType = "Line";
                currentPolyLine = null;
            };
            this.Controls.Add(btnLine);

            btnClear = new Button();
            btnClear.Text = "Цэвэрлэх";
            btnClear.Location = new Point(325, 12);
            btnClear.Size = new Size(90, 30);
            btnClear.BackColor = Color.LightGray;
            btnClear.Click += (s, e) => {
                shapes.Clear();
                currentPolyLine = null;
                this.Invalidate();
            };
            this.Controls.Add(btnClear);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "Shape Lab - Дүрс ба Цэгээр холбох шулуун";
        }

        private void Form1_MouseClick(object? sender, MouseEventArgs e)
        {
            // Хэрэв товчлуурууд байрлах дээд хэсэгт (Y < 55) дарвал зурахгүй
            if (e.Y < 55) return;

            if (currentShapeType == "Circle")
            {
                shapes.Add(new Circle { Location = e.Location });
            }
            else if (currentShapeType == "Rectangle")
            {
                shapes.Add(new MyRectangle { Location = e.Location });
            }
            else if (currentShapeType == "Line")
            {
                if (currentPolyLine == null)
                {
                    currentPolyLine = new MyPolyLine();
                    shapes.Add(currentPolyLine);
                }

                currentPolyLine.Points.Add(e.Location);
            }

            this.Invalidate();
        }

        private void Form1_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.C)
            {
                currentShapeType = "Circle";
                currentPolyLine = null;
            }
            else if (e.KeyCode == Keys.T || e.KeyCode == Keys.M)
            {
                currentShapeType = "Rectangle";
                currentPolyLine = null;
            }
            else if (e.KeyCode == Keys.L)
            {
                currentShapeType = "Line";
                currentPolyLine = null;
            }
            else if (e.KeyCode == Keys.R)
            {
                shapes.Clear();
                currentPolyLine = null;
                this.Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            foreach (var shape in shapes)
            {
                shape.Draw(e.Graphics);
            }
        }
    }
}