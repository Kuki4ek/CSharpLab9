using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab9
{
    public partial class Form1 : Form
    {
        private static Bitmap image;
        private Kernel kernel;
        public Form1()
        {
            InitializeComponent();
            image = new Bitmap(typeof(Form1).Assembly.GetManifestResourceStream("lab9.image.png"));
            pictureBox1.Image = image;
        }
        public Bitmap CombineBitmap(Bitmap up, Bitmap down)
        {
            int width = Math.Max(up.Width, down.Width);
            int height = up.Height + down.Height;
            Bitmap new_bitmap = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(new_bitmap))
            {
                g.Clear(Color.White);
                g.DrawImage(up, 0, 0);
                g.DrawImage(down, 0, up.Height);
            }
            return new_bitmap;
        }
        public void ConvolutionStreams(double[][] kernel_cells, double summ)
        {
            if (image == null) return;

            Stopwatch sw = new Stopwatch();
            sw.Start();
            pictureBox2.Image = null;
            kernel.Summ = summ;
            kernel.Cells = kernel_cells;

            int imageWidth, imageHeight;
            lock (image)
            {
                imageWidth = image.Width;
                imageHeight = image.Height;
            }

            int streams = trackBar1.Value;
            int rect_height = imageHeight / streams;
            int last_rect_height = imageHeight - (streams - 1) * rect_height;
            int padding = kernel.Cells.Length / 2; // Размер отступа для свертки

            Bitmap[] results = new Bitmap[streams];
            Thread[] threads = new Thread[streams];

            for (int i = 0; i < streams; i++)
            {
                Rectangle rect;
                Rectangle cloneRect;

                if (i != streams - 1)
                {
                    rect = new Rectangle(0, i * rect_height, imageWidth, rect_height);
                }
                else
                {
                    rect = new Rectangle(0, i * rect_height, imageWidth, last_rect_height);
                }

                int cloneY = Math.Max(0, rect.Y - padding);
                int cloneHeight = rect.Height + padding * 2;
                if (cloneY + cloneHeight > imageHeight)
                    cloneHeight = imageHeight - cloneY;

                cloneRect = new Rectangle(0, cloneY, imageWidth, cloneHeight);

                int index = i;
                Rectangle currentRect = rect;
                Rectangle currentCloneRect = cloneRect;

                threads[index] = new Thread(() =>
                {
                    Bitmap image_copy;
                    lock (image)
                    {
                        image_copy = image.Clone(currentCloneRect, image.PixelFormat);
                    }

                    Rectangle processRect = new Rectangle(0, currentRect.Y - currentCloneRect.Y,
                                                          currentRect.Width, currentRect.Height);

                    Bitmap result = ImageCore.Convolution(image_copy, kernel, processRect);

                    results[index] = result;

                    image_copy.Dispose();
                });

                threads[index].Start();
            }

            foreach (Thread thread in threads)
            {
                thread.Join();
            }

            Bitmap new_image = results[0];
            for (int i = 1; i < results.Length; i++)
            {
                Bitmap combined = CombineBitmap(new_image, results[i]);
                if (i > 1) new_image.Dispose();
                new_image = combined;
            }

            pictureBox2.Image = new_image;
            sw.Stop();
            label1.Text = $"Время выполнения: {sw.ElapsedMilliseconds}мс";
        }
        private void button1_Click(object sender, EventArgs e)
        {
            double[][] kernel_cells = new double[][]
            {
                new double[] { 1, 1, 1},
                new double[] { 1, 1, 1},
                new double[] { 1, 1, 1}
            };
            ConvolutionStreams(kernel_cells, 9);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            double[][] kernel_cells = new double[][]
            {
                new double[] { 1, 1, 1, 1, 1},
                new double[] { 1, 1, 1, 1, 1},
                new double[] { 1, 1, 1, 1, 1},
                new double[] { 1, 1, 1, 1, 1},
                new double[] { 1, 1, 1, 1, 1}
            };
            ConvolutionStreams(kernel_cells, 25);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            double[][] kernel_cells = new double[][]
            {
                new double[] { 1, 2, 1},
                new double[] { 2, 4, 2},
                new double[] { 1, 2, 1}
            };
            ConvolutionStreams(kernel_cells, 16);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            double[][] kernel_cells = new double[][]
            {
                new double[] { 1,  4,  7,  4, 1 },
                new double[] { 4, 16, 26, 16, 4 },
                new double[] { 7, 26, 41, 26, 7 },
                new double[] { 4, 16, 26, 16, 4 },
                new double[] { 1,  4,  7,  4, 1 }
            };
            ConvolutionStreams(kernel_cells, 273);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            double[][] kernel_cells = new double[][]
            {
                new double[] {  0, -1,  0 },
                new double[] { -1,  5, -1 },
                new double[] {  0, -1,  0 }
            };
            ConvolutionStreams(kernel_cells, 1);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            double[][] kernel_cells = new double[][]
            {
                new double[] { -1, -1, -1, -1, -1 },
                new double[] { -1,  2,  2,  2, -1 },
                new double[] { -1,  2,  8,  2, -1 },
                new double[] { -1,  2,  2,  2, -1 },
                new double[] { -1, -1, -1, -1, -1 }
            };
            ConvolutionStreams(kernel_cells, 1);
        }
    }
}
