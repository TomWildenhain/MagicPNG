using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Drawing.Imaging;
using System.Drawing;
using mColor = System.Windows.Media.Color;
using dColor = System.Drawing.Color;

namespace MagicPng
{
    static class ImageTransformer
    {
        public static void previewTransformOld(string file, string fileForSize, string output, mColor lightColor, mColor darkColor, bool invert, bool invertBg)
        {
            Bitmap inputImg = new Bitmap(file);
            int width = inputImg.Width;
            int height = inputImg.Height;
            if (fileForSize != "")
            {
                Bitmap sizeImg = new Bitmap(fileForSize);
                width = Math.Max(inputImg.Width, sizeImg.Width);
                height = Math.Max(inputImg.Height, sizeImg.Height);
                sizeImg.Dispose();
            }
            Bitmap outputImg = new Bitmap(width, height);
            dColor dLightColor = mColorToDColor(lightColor);
            dColor dDarkColor = mColorToDColor(darkColor);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    dColor pixel = getPixelFromBitmap(inputImg, x, y, width, height);
                    outputImg.SetPixel(x, y, previewGetColor(pixel, dLightColor, dDarkColor, invert, invertBg));
                }
            }
            outputImg.Save(output);
            inputImg.Dispose();
            outputImg.Dispose();
        }
        public static void previewTransform(string file, string fileForSize, string output, mColor lightColor, mColor darkColor, bool invert, bool invertBg)
        {
            Bitmap inputImg = new Bitmap(file);
            int width = inputImg.Width;
            int height = inputImg.Height;
            if (fileForSize != "")
            {
                Bitmap sizeImg = new Bitmap(fileForSize);
                width = Math.Max(inputImg.Width, sizeImg.Width);
                height = Math.Max(inputImg.Height, sizeImg.Height);
                sizeImg.Dispose();
            }
            Bitmap outputImg = new Bitmap(width, height);
            dColor dLightColor = mColorToDColor(lightColor);
            dColor dDarkColor = mColorToDColor(darkColor);
            dColor bgColor = dLightColor;
            if (invertBg != invert)
            {
                bgColor = dDarkColor;
            }

            //get a graphics object from the new image
            Graphics g = Graphics.FromImage(outputImg);
            // create the negative color matrix
            ColorMatrix colorMatrix = getPreviewColorMatrix(lightColor, darkColor, invert);
            // create some image attributes
            ImageAttributes attributes = new ImageAttributes();
            attributes.SetColorMatrix(colorMatrix);
            int leftMargin = (width - inputImg.Width) / 2;
            int topMargin = (height - inputImg.Height) / 2;
            g.FillRectangle(new SolidBrush(bgColor), new Rectangle(0, 0, width, height));
            g.DrawImage(inputImg, new Rectangle(leftMargin, topMargin, inputImg.Width, inputImg.Height),
                        0, 0, inputImg.Width, inputImg.Height, GraphicsUnit.Pixel, attributes);
            //dispose the Graphics object
            g.Dispose();

            outputImg.Save(output);
            inputImg.Dispose();
            outputImg.Dispose();
        }
        private static dColor getPixelFromBitmap(Bitmap img, int x, int y, int width, int height)
        {
            int leftMargin = (width - img.Width) / 2;
            int topMargin = (height - img.Height) / 2;
            int imgX = x - leftMargin;
            int imgY = y - topMargin;
            if (0 <= imgX && imgX < img.Width && 0 <= imgY && imgY < img.Height)
            {
                return img.GetPixel(imgX, imgY);
            }
            return dColor.FromArgb(0);
        }
        private static ColorMatrix getPreviewColorMatrix(mColor lightColor, mColor darkColor, bool invert)
        {
            ColorMatrix rawGrayScale = new ColorMatrix(
               new float[][]
               {
                  new float[] { 1f / 3, 1f / 3, 1f / 3, 0, 0},
                  new float[] { 1f / 3, 1f / 3, 1f / 3, 0, 0},
                  new float[] { 1f / 3, 1f / 3, 1f / 3, 0, 0},
                  new float[] { 0, 0, 0, 1, 0},
                  new float[] {0, 0, 0, 0, 1}
               });
            ColorMatrix invertMatrix = new ColorMatrix(
               new float[][]
               {
                  new float[] { 1, 0, 0, 0, 0},
                  new float[] { 0, 1, 0, 0, 0},
                  new float[] { 0, 0, 1, 0, 0},
                  new float[] { 0, 0, 0, 1, 0},
                  new float[] { 0, 0, 0, 0, 1}
               });
            if (invert)
            {
                invertMatrix = new ColorMatrix(
                   new float[][]
                   {
                      new float[] { -1, 0, 0, 0, 0},
                      new float[] { 0, -1, 0, 0, 0},
                      new float[] { 0, 0, -1, 0, 0},
                      new float[] { 0, 0, 0, 1, 0},
                      new float[] { 1, 1, 1, 0, 1}
                   });
            } 

            int R1 = darkColor.R;
            int G1 = darkColor.G;
            int B1 = darkColor.B;
            int RI = lightColor.R - R1;
            int GI = lightColor.G - G1;
            int BI = lightColor.B - B1;
            ColorMatrix lumToColor = new ColorMatrix(
               new float[][]
               {
                  new float[] { RI / 255f, 0, 0, 0, 0},
                  new float[] { 0, GI/255f, 0, 0, 0},
                  new float[] { 0, 0, BI / 255f, 0, 0},
                  new float[] { 0, 0, 0, 1, 0},
                  new float[] { R1 / 255f, G1 / 255f, B1 / 255f, 0, 1 }
               });
            return MatrixMultiply(MatrixMultiply(lumToColor, invertMatrix), rawGrayScale);
        }
        private static dColor previewGetColor(dColor pixel, dColor lightColor, dColor darkColor, bool invert, bool invertBg)
        {
            byte lum = getLum(pixel, invert, invertBg);
            if (invert)
            {
                lum = (byte)(255 - lum);
            }
            return weightedAverage(darkColor, lum, lightColor, (byte)(255 - lum));
        }
        private static byte weightedAverage(byte val1, byte weight1, byte val2, byte weight2)
        {
            if (weight1 + weight2 == 0)
            {
                return (byte)((val1 + val2) / 2);
            }
            return (byte)((val1 * weight1 + val2 * weight2) / (weight1 + weight2));
        }
        private static dColor weightedAverage(dColor val1, byte weight1, dColor val2, byte weight2)
        {
            if (weight1 + weight2 == 0)
            {
                return weightedAverage(val1, 127, val2, 127);
            }
            byte A = 255;
            byte R = weightedAverage(val1.R, weight1, val2.R, weight2);
            byte G = weightedAverage(val1.G, weight1, val2.G, weight2);
            byte B = weightedAverage(val1.B, weight1, val2.B, weight2);
            return dColor.FromArgb(A, R, G, B);
        }
        public static mColor weightedAverage(mColor val1, byte weight1, mColor val2, byte weight2)
        {
            return dColorToMColor(weightedAverage(mColorToDColor(val1), weight1, mColorToDColor(val2), weight2));
        }
        private static dColor mColorToDColor(mColor color)
        {
            return dColor.FromArgb(color.A, color.R, color.G, color.B);
        }
        private static mColor dColorToMColor(dColor color)
        {
            return mColor.FromArgb(color.A, color.R, color.G, color.B);
        }
        public static void mergeImages(string file1, string file2, string output, mColor lightColor, mColor darkColor, 
                                       bool invert1, bool invertBg1, bool invert2, bool invertBg2)
        {
            Bitmap inputImg1 = new Bitmap(file1);
            Bitmap inputImg2 = new Bitmap(file2);
            int width = Math.Max(inputImg1.Width, inputImg2.Width);
            int height = Math.Max(inputImg1.Height, inputImg2.Height);
            Bitmap outputImg = new Bitmap(width, height);
            dColor dLightColor = mColorToDColor(lightColor);
            dColor dDarkColor = mColorToDColor(darkColor);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    dColor pixel1 = getPixelFromBitmap(inputImg1, x, y, width, height);
                    dColor pixel2 = getPixelFromBitmap(inputImg2, x, y, width, height);
                    outputImg.SetPixel(x, y, mergeGetColor(pixel1, pixel2, dLightColor, dDarkColor, invert1, invertBg1, invert2, invertBg2));
                }
            }
            outputImg.Save(output);
            inputImg1.Dispose();
            inputImg2.Dispose();
            outputImg.Dispose();
        }
        private static dColor mergeGetColor(dColor pixel1, dColor pixel2, dColor lightColor, dColor darkColor, 
                                            bool invert1, bool invertBg1, bool invert2, bool invertBg2)
        {
            byte A1 = getLum(pixel1, invert1, invertBg1);
            byte A2 = getLum(pixel2, invert2, invertBg2);
            byte R = weightedAverage(darkColor.R, A1, lightColor.R, A2);
            byte G = weightedAverage(darkColor.G, A1, lightColor.G, A2);
            byte B = weightedAverage(darkColor.B, A1, lightColor.B, A2);
            byte A = (byte)((A1 + A2) / 2);
            return dColor.FromArgb(A, R, G, B);
        }
        private static byte getLum(dColor pixel, bool invert, bool invertBg)
        {
            byte lum = (byte)((pixel.R + pixel.G + pixel.B) / 3);
            byte bgLum = 255;
            if (invertBg != invert)
            {
                bgLum = 0;
            }
            if (invert)
            {
                lum = (byte)(255 - lum);
            }
            return weightedAverage(lum, pixel.A, bgLum, (byte)(255 - pixel.A));
        }
        private static ColorMatrix MatrixMultiply(ColorMatrix f1, ColorMatrix f2)
        {
            float[][] X = new float[5][];
            for (int d = 0; d < 5; d++)
                X[d] = new float[5];
            int size = 5;
            float[] column = new float[5];
            for (int j = 0; j < 5; j++)
            {
                for (int k = 0; k < 5; k++)
                {
                    column[k] = f1[k,j];
                }
                for (int i = 0; i < 5; i++)
                {
                    float s = 0;
                    for (int k = 0; k < size; k++)
                    {
                        s += f2[i,k] * column[k];
                    }
                    X[i][j] = s;
                }
            }
            return new ColorMatrix(X);
        }
    }
}
