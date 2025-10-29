using System;
using System.Collections.Generic;
using System.Drawing;

namespace lab9
{
    public static class ImageCore
    {
        public static Bitmap Convolution(BufferedBitmap buff_image, Kernel kernel)
        {
            Rectangle rect = new Rectangle(0, 0, buff_image.Width, buff_image.Height);
            return Convolution(buff_image, kernel, rect);
        }
        public static Bitmap Convolution(BufferedBitmap buff_image, Kernel kernel, Rectangle rect)
        {
            // Создаем результирующее изображение размером с обрабатываемую область
            Bitmap new_bitmap = new Bitmap(rect.Width, rect.Height);
            double[][] cells = kernel.Cells;
            List<(int, int)> pixels = new List<(int, int)>();

            for (int i = -(cells.Length / 2); i <= (cells.Length / 2); i++)
            {
                for (int j = -(cells.Length / 2); j <= (cells.Length / 2); j++)
                {
                    pixels.Add((i, j));
                }
            }

            for (int j = rect.Y; j < rect.Y + rect.Height; j++)
            {
                for (int i = rect.X; i < rect.X + rect.Width; i++)
                {
                    int R = 0;
                    int G = 0;
                    int B = 0;
                    foreach (var k in pixels)
                    {
                        Color pixel_color;
                        int ii = i + k.Item1;
                        int jj = j + k.Item2;

                        // Правильная обработка границ с отражением
                        if (ii < 0) ii = -ii;
                        if (ii >= buff_image.Width) ii = 2 * buff_image.Width - ii - 2;
                        if (jj < 0) jj = -jj;
                        if (jj >= buff_image.Height) jj = 2 * buff_image.Height - jj - 2;

                        pixel_color = buff_image.GetPixel(ii, jj);
                        int coeff = kernel.Cells.Length / 2;
                        R += (int)(pixel_color.R * kernel.Cells[k.Item1 + coeff][k.Item2 + coeff]);
                        G += (int)(pixel_color.G * kernel.Cells[k.Item1 + coeff][k.Item2 + coeff]);
                        B += (int)(pixel_color.B * kernel.Cells[k.Item1 + coeff][k.Item2 + coeff]);
                    }
                    R = (int)((double)R / kernel.Summ);
                    G = (int)((double)G / kernel.Summ);
                    B = (int)((double)B / kernel.Summ);
                    R = Math.Min(255, Math.Max(0, R));
                    G = Math.Min(255, Math.Max(0, G));
                    B = Math.Min(255, Math.Max(0, B));
                    Color new_pixel_color = Color.FromArgb(R, G, B);

                    // Устанавливаем пиксель относительно области rect
                    new_bitmap.SetPixel(i - rect.X, j - rect.Y, new_pixel_color);
                }
            }

            return new_bitmap;
        }
    }
}
