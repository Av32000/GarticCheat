using System;
using System.Net;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;

namespace GarticCheat
{
    public partial class Form1 : Form
    {
        List<Color> colors = new List<Color>();

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
    }
}
