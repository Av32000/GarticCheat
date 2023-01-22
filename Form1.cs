using System;
using System.Net;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace GarticCheat
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
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
                        imagePreview.Image = image;
                    }
                }
            }
        }
    }
}
