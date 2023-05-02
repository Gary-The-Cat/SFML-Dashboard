using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;

namespace Shared.ExtensionMethods
{
    public static class ImageExtensions
    {
        public static System.Drawing.Bitmap ConvertToBitmap(this Image image)
        {
            var bitmap = new System.Drawing.Bitmap((int)image.Size.X, (int)image.Size.Y, PixelFormat.Format32bppArgb);
            var bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.WriteOnly, bitmap.PixelFormat);
            byte[] data = getPixelData(image);
            Marshal.Copy(data, 0, bitmapData.Scan0, data.Length);
            bitmap.UnlockBits(bitmapData);
            return bitmap;
        }

        private static byte[] getPixelData(Image image)
        {
            byte[] data = image.Pixels;

            // SFML is always RGBA, regardless of the endian-ness of the CPU. On a little-endian
            // CPU, System.Drawing.Bitmap expects BGRA.
            if (BitConverter.IsLittleEndian)
            {
                byte[] source = image.Pixels;
                data = new byte[source.Length];
                for (int i = 0; i < data.Length; i += 4)
                {
                    data[i] = source[i + 2];
                    data[i + 1] = source[i + 1];
                    data[i + 2] = source[i];
                    data[i + 3] = source[i + 3];
                }
            }
            return data;
        }
    }
}
