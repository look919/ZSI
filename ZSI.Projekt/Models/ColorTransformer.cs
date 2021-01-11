using System.Drawing;
using System.Drawing.Imaging;

namespace ZSI.Projekt.Models.Extensions
{
    public static class ColorExtensions
    {
        public static bool IsNotWhite(this Color color)
        {
            return color.R < 255 || color.G < 255 || color.B < 255;
        }

        public static bool IsNotBlack(this Color color)
        {
            return color.R > 0 || color.G > 0 || color.B > 0;
        }

        public static void TransformToBlackAndWhite(this Bitmap bitmap)
        {
            var newBitmap = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), PixelFormat.Format1bppIndexed);

            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    var pixel = bitmap.GetPixel(i, j);

                    if (pixel.IsNotWhite() && pixel.IsNotBlack())
                        bitmap.SetPixel(i, j, Color.White);
                }
            }
        }
    }
}
