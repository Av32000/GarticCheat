using System;
using System.Net;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace GarticCheat
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, UIntPtr dwExtraInfo);

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;

        List<Color> colors = new List<Color>();
        List<Point> colorPositions = new List<Point>();

        Point start = new Point(0,0);
        Point end = new Point(0, 0);

        Color formColor;

        Bitmap imageToDraw = null;
        public Form1()
        {
            InitializeComponent();
            colors.Add(Color.FromArgb(0, 0, 0));
            colors.Add(Color.FromArgb(102, 102, 102));
            colors.Add(Color.FromArgb(0, 80, 205));
            colors.Add(Color.FromArgb(255, 255, 255));
            colors.Add(Color.FromArgb(170, 170, 170));
            colors.Add(Color.FromArgb(38, 201, 255));
            colors.Add(Color.FromArgb(1, 116, 32));
            colors.Add(Color.FromArgb(153, 0, 0));
            colors.Add(Color.FromArgb(150, 65, 18));
            colors.Add(Color.FromArgb(17, 176, 60));
            colors.Add(Color.FromArgb(255, 0, 19));
            colors.Add(Color.FromArgb(255, 120, 41));
            colors.Add(Color.FromArgb(176, 112, 28));
            colors.Add(Color.FromArgb(153, 0, 78));
            colors.Add(Color.FromArgb(203, 90, 87));
            colors.Add(Color.FromArgb(255, 193, 38));
            colors.Add(Color.FromArgb(255, 0, 143));
            colors.Add(Color.FromArgb(254, 175, 168));

            formColor = BackColor;

            Log(colors.Count + " colors loaded !");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(imageURL.Text != "")
            {
                using (WebClient webClient = new WebClient())
                {
                    byte[] imageBytes = webClient.DownloadData(imageURL.Text);
                    using (MemoryStream ms = new MemoryStream(imageBytes))
                    {
                        Image image = Image.FromStream(ms);
                        imagePreview.Image = ProcessImage(CreateNonIndexedImage(image));

                        if(imageToDraw != null)
                        {
                            imageToDraw = ProcessImage((Bitmap)imagePreview.Image.GetThumbnailImage(end.X - start.X, end.Y - start.Y, null, IntPtr.Zero));
                        }

                        Log((image.Size.Width * image.Size.Height) + " pixels loaded (" + image.Size.Width + ";" + image.Size.Height + ")");
                    }
                }
            }
        }

        public Bitmap CreateNonIndexedImage(Image src)
        {
            Bitmap newBmp = new Bitmap(src.Width, src.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (Graphics gfx = Graphics.FromImage(newBmp))
            {
                gfx.DrawImage(src, 0, 0);
            }

            return newBmp;
        }

        public Bitmap ProcessImage(Bitmap image)
        {
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color pixelColor = image.GetPixel(x, y);
                    Color closestColor = GetClosestColor(pixelColor, colors);
                    image.SetPixel(x, y, closestColor);
                }
            }

            return image;
        }

        private Color GetClosestColor(Color pixelColor, List<Color> colors)
        {
            int minDistance = int.MaxValue;
            Color closestColor = Color.Black;
            foreach (Color color in colors)
            {
                int distance = GetColorDistance(pixelColor, color);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestColor = color;
                }
            }
            return closestColor;
        }

        private int GetColorDistance(Color a, Color b)
        {
            int deltaR = a.R - b.R;
            int deltaG = a.G - b.G;
            int deltaB = a.B - b.B;
            return (int)Math.Sqrt(deltaR * deltaR + deltaG * deltaG + deltaB * deltaB);
        }

        private void Log(string log)
        {
            logLabel.Text += "\n" + log;
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            if (start.X == 0 && start.Y == 0) start = Cursor.Position;
            else end = Cursor.Position;

            if(start != new Point(0,0) && end != new Point(0, 0))
            {
                positionLabel.Text = "Start : (" + start.X + ";" + start.Y + ") End : (" + end.X + ";" + end.Y + ")";
                Log("Start : (" + start.X + ";" + start.Y + ") End : (" + end.X + ";" + end.Y + ")");
                panel1.Visible = false;

                imageToDraw = ProcessImage((Bitmap)imagePreview.Image.GetThumbnailImage(end.X - start.X, end.Y - start.Y, null, IntPtr.Zero));
                Log("Image(" + imageToDraw.Size.Width + ";" + imageToDraw.Size.Height + ") ready !");

                panel1.Visible = false;
                FormBorderStyle = FormBorderStyle.Sizable;
                WindowState = FormWindowState.Normal;
                BackColor = formColor;
                this.Opacity = 1;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            start = new Point(0, 0);
            end = new Point(0, 0);

            panel1.Visible = true;
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;

            this.BackColor = Color.Black;
            this.Opacity = 0.5;

            panel1.Dock = DockStyle.Fill;
            panel1.BackColor = Color.FromArgb(100, 0, 0, 0);
        }

        private void panel2_Click(object sender, EventArgs e)
        {
            colorPositions.Add(Cursor.Position);

            if(colorPositions.Count == colors.Count)
            {
                colorsLabel.Text = "Colors Ready !";

                Log(colorPositions.Count + " positions loaded !");

                panel2.Visible = false;
                FormBorderStyle = FormBorderStyle.Sizable;
                WindowState = FormWindowState.Normal;
                BackColor = formColor;
                this.Opacity = 1;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            colorPositions.Clear();

            panel2.Visible = true;
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;

            this.BackColor = Color.Black;
            this.Opacity = 0.5;

            panel2.Dock = DockStyle.Fill;
            panel2.BackColor = Color.FromArgb(100, 0, 0, 0);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int cps = (int)Math.Round(1000 / numericUpDown1.Value, 0);
            Log("CPS : " + cps);
            int offset = 0;
            switch (trackBar1.Value)
            {
                case 1:
                    offset = 0;
                    break;
                case 2:
                    offset = 5;
                    break;
                case 3:
                    offset = 11;
                    break;
                case 4:
                    offset = 20;
                    break;
            }
            if(imageToDraw != null)
            {
                this.TopMost = true;

                List<List<Point>> points = new List<List<Point>>();
                foreach (Color color in colors)
                {
                    points.Add(new List<Point>());
                }

                for (int y = 0; y < imageToDraw.Height; y++)
                {
                    for (int x = 0; x < imageToDraw.Width; x++)
                    {
                        if(imageToDraw.GetPixel(x, y) != Color.FromArgb(255,255,255))
                        {
                            points[colors.IndexOf(imageToDraw.GetPixel(x, y))].Add(new Point(start.X + x, start.Y + y));
                            x += offset;
                        }
                    }
                    y += offset;
                }

                foreach (List<Point> currentList in points)
                {
                    GenerateClick(colorPositions[points.IndexOf(currentList)]);
                    System.Threading.Thread.Sleep(100);

                    foreach (Point point in currentList)
                    {
                        GenerateClick(point);
                        System.Threading.Thread.Sleep(cps);
                    }
                }
            }
        }

        private void GenerateClick(Point position)
        {
            Cursor.Position = position;
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)position.X, (uint)position.Y, 0, UIntPtr.Zero);
        }
    }
}
