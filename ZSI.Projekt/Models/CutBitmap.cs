using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZSI.Projekt.Models.Extensions;

namespace ZSI.Projekt.Models
{
    public static class CutBitmap
    {
        public static byte[] SegmentLetterAndGetAsByteArray(Bitmap bitmap)
        {
            var sideLength = AppSettings.BitmapSideLength;

            var letters = Segment(bitmap, sideLength);

            var lettersAsByteArray = new List<byte[]>();

            foreach (var l in letters) {
                lettersAsByteArray.Add(GetBytesFromPixels(l));
            }

            return lettersAsByteArray[0];
        }

        public static Bitmap SegmentLetter(Bitmap bitmap)
        {
            var sideLength = AppSettings.BitmapSideLength;

            return Segment(bitmap, sideLength)[0];
        }

        public static List<byte[]> SegmentEntireBitmap(Bitmap bitmap)
        {
            var sideLength = AppSettings.BitmapSideLength;

            var letters = Segment(bitmap, sideLength);

            var lettersAsByteArray = new List<byte[]>();

            foreach (var l in letters) {
                lettersAsByteArray.Add(GetBytesFromPixels(l));
            }

            return lettersAsByteArray;
        }

        private static List<Bitmap> Segment(Bitmap bitmap, int sideLength)
        {
            bitmap.TransformToBlackAndWhite();

            var bitmapRows = GetRows(bitmap);

            var letters = new List<Bitmap>();

            foreach (var r in bitmapRows) {
                letters.AddRange(GetLetters(r));
            }

            var fixedLetters = new List<Bitmap>();

            foreach (var l in letters) {
                var letter = SetToSize(GetLetterWithoutWhiteSpaces(l), sideLength, sideLength);
                fixedLetters.Add(letter);
            }

            return fixedLetters;
        }

        private static byte[] GetBytesFromPixels(Bitmap bitmap)
        {
            var arraySize = bitmap.Size.Width * bitmap.Size.Height;
            var array = new byte[arraySize];

            int count = 0;
            for (int i = 0; i < bitmap.Size.Width; i++) {
                for (int j = 0; j < bitmap.Size.Height; j++) {
                    var pixel = bitmap.GetPixel(i, j);
                    array[count] = (byte)(pixel.IsNotWhite() ? 1 : 0); 
                    count++;
                }
            }

            return array;
        }

        private static Bitmap SetToSize(Bitmap bitmap, int width, int heigth)
        {
            var result = new Bitmap(width, heigth);
            var g = Graphics.FromImage(result);
            g.DrawImage(bitmap, 0, 0, 20, 20);

            return result;
        }

        private static Bitmap GetLetterWithoutWhiteSpaces(Bitmap letter)
        {
            var first = GetPositionWithoutWhiteSpaces(letter);

            var firstBitmap = letter.Clone(first, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            return firstBitmap;
        }

        private static Rectangle GetPositionWithoutWhiteSpaces(Bitmap letter)
        {
            int offsetY = 0;

            while (IsPixel(letter, offsetY)) {
                offsetY++;
            }

            return new Rectangle {
                X = 0,
                Y = 0,
                Width = letter.Width,
                Height = offsetY
            };
        }

        private static bool IsPixel(Bitmap letter, int offsetY = 0)
        {
            for (int i = 0; i < letter.Width; i++) {
                for (int j = offsetY; j < letter.Height; j++) {
                    if (letter.GetPixel(i, j).IsNotWhite())
                        return true;
                }
            }

            return false;
        }

        private static IEnumerable<Bitmap> GetRows(Bitmap bitmap)
        {
            var rows = new List<Bitmap>();

            foreach (var p in GetRowsPositions(bitmap)) {
                rows.Add(bitmap.Clone(p, System.Drawing.Imaging.PixelFormat.Format32bppArgb));
            }

            return rows;
        }

        private static IEnumerable<Bitmap> GetLetters(Bitmap row)
        {
            var letters = new List<Bitmap>();

            var positions = GetLettersPositions(row);

            foreach (var p in positions) {
                letters.Add(row.Clone(p, System.Drawing.Imaging.PixelFormat.Format32bppArgb));
            }

            return letters;
        }

        private static IEnumerable<Rectangle> GetLettersPositions(Bitmap row)
        {
            var positions = new List<Rectangle>();

            int offsetY = 0;
            int offsetX = 0;

            int height;
            int width;

            while (IsLetterInRow(row, offsetX)) {
                offsetY = GetOffsetY(row, offsetY, offsetX);
                offsetX = GetOffsetX(row, offsetX, offsetY);

                height = row.Height;
                width = GetWidth(row, offsetX, offsetY);

                positions.Add(new Rectangle {
                    X = offsetX,
                    Y = 0,
                    Width = width,
                    Height = height
                });

                offsetX += width;
            }

            return positions;
        }

        private static IEnumerable<Rectangle> GetRowsPositions(Bitmap bitmap)
        {
            var positions = new List<Rectangle>();

            int offsetY = 0;
            int height;

            while (IsRow(bitmap, 0, offsetY)) {
                offsetY = GetOffsetY(bitmap, offsetY);
                height = GetHeight(bitmap, offsetY);

                positions.Add(new Rectangle {
                    X = 0,
                    Y = offsetY,
                    Width = bitmap.Width,
                    Height = height
                });

                offsetY += height;
            }

            return positions;
        }

        private static int GetOffsetY(Bitmap bitmap, int prevOffsetY = 0, int prevOffsetX = 0)
        {
            for (int i = prevOffsetY; i < bitmap.Height; i++) {
                for (int j = prevOffsetX; j < bitmap.Width; j++) {
                    if (bitmap.GetPixel(j, i).IsNotWhite())
                        return i;
                }
            }

            return 0;
        }

        private static int GetOffsetX(Bitmap bitmap, int prevOffsetX = 0, int prevOffsetY = 0)
        {
            for (int i = prevOffsetX; i < bitmap.Width; i++) {
                for (int j = prevOffsetY; j < bitmap.Height; j++) {
                    if (bitmap.GetPixel(i, j).IsNotWhite())
                        return i;
                }
            }

            return 0;
        }

        private static int GetHeight(Bitmap bitmap, int offsetY = 0, int offsetX = 0)
        {
            for (int i = offsetY; i < bitmap.Height; i++) {
                int countNotWhite = 0;
                for (int j = offsetX; j < bitmap.Width; j++) {
                    if (bitmap.GetPixel(j, i).IsNotWhite())
                        countNotWhite++;
                    if (j == bitmap.Width - 1 && countNotWhite == 0)
                        return i - offsetY;
                }
            }

            return bitmap.Height;
        }

        private static int GetWidth(Bitmap bitmap, int offsetX = 0, int offsetY = 0)
        {
            for (int i = offsetX; i < bitmap.Width; i++) {
                int countNotWhite = 0;
                for (int j = offsetY; j < bitmap.Height; j++) {
                    if (bitmap.GetPixel(i, j).IsNotWhite())
                        countNotWhite++;
                    if (j == bitmap.Height - 1 && countNotWhite == 0)
                        return i - offsetX;
                }
            }

            return bitmap.Width;
        }

        private static bool IsLetterInRow(Bitmap row, int offsetX = 0)
        {
            for (int i = offsetX; i < row.Width; i++) {
                for (int j = 0; j < row.Height; j++) {
                    if (row.GetPixel(i, j).IsNotWhite())
                        return true;
                }
            }

            return false;
        }

        private static bool IsRow(Bitmap bitmap, int offsetX = 0, int offsetY = 0)
        {
            for (int i = offsetX; i < bitmap.Width; i++) {
                for (int j = offsetY; j < bitmap.Height; j++) {
                    if (bitmap.GetPixel(i, j).IsNotWhite())
                        return true;
                }
            }

            return false;
        }
    }
}
