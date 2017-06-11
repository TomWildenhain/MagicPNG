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
        /// <summary>
        /// Saves a preview of the result of merging two image files with MagicPNG when over one background
        /// </summary>
        /// <param name="file">Path to the image to preview</param>
        /// <param name="fileForSize">Path to another image, whose dimensions will be used to ensure 
        /// the preview has the dimensions of the larger of the two images.  Can be "" to signify no second file should be used.</param>
        /// <param name="output">The path the preview image should be saved to</param>
        /// <param name="lightColor">The first background color</param>
        /// <param name="darkColor">The second background color</param>
        /// <param name="invert">Determines if the image should be inverted</param>
        /// <param name="invertBg">Determines if the image should use the darkColor as the background</param>
        public static void previewTransform(string file, string fileForSize, string output, mColor lightColor, 
            mColor darkColor, bool invert, bool invertBg)
        {

            // Load the first image
            Bitmap inputImg = new Bitmap(file);
            int width = inputImg.Width;
            int height = inputImg.Height;
            // If a second image was provided...
            if (fileForSize != "")
            {
                // Load the second image
                Bitmap sizeImg = new Bitmap(fileForSize);
                // Use it to adjust the dimensions
                width = Math.Max(inputImg.Width, sizeImg.Width);
                height = Math.Max(inputImg.Height, sizeImg.Height);
                // Free the second image
                sizeImg.Dispose();
            }
            // Create an output image with the new dimensions
            Bitmap outputImg = new Bitmap(width, height);

            // Convert System.Windows.Media colors to System.Drawing colors
            dColor dLightColor = mColorToDColor(lightColor);
            dColor dDarkColor = mColorToDColor(darkColor);
            dColor bgColor = dLightColor;

            // Select correct background
            if (invertBg != invert)
            {
                bgColor = dDarkColor;
            }

            // Get a graphics object from the new image
            Graphics g = Graphics.FromImage(outputImg);
            // Create the matrix represnting the transformation
            ColorMatrix colorMatrix = getPreviewColorMatrix(lightColor, darkColor, invert);
            // Create some image attributes
            ImageAttributes attributes = new ImageAttributes();
            // Apply the matrix
            attributes.SetColorMatrix(colorMatrix);
            // Computes the border caused by the size differance between to two input image files
            int leftMargin = (width - inputImg.Width) / 2;
            int topMargin = (height - inputImg.Height) / 2;
            // Fills the output image with the background color
            g.FillRectangle(new SolidBrush(bgColor), new Rectangle(0, 0, width, height));
            // Places the preview at the center of the output
            g.DrawImage(inputImg, new Rectangle(leftMargin, topMargin, inputImg.Width, inputImg.Height),
                        0, 0, inputImg.Width, inputImg.Height, GraphicsUnit.Pixel, attributes);
            // Dispose the Graphics object
            g.Dispose();

            // Saves the result
            outputImg.Save(output);
            // Closes the files
            inputImg.Dispose();
            outputImg.Dispose();
        }
        /// <summary>
        /// Very slowly gets a pixel's color form an image.  The image is assumed to be centered on an output image with the specified
        /// width and height
        /// </summary>
        /// <param name="img">The image to get the pixel from</param>
        /// <param name="x">The x coord</param>
        /// <param name="y">The y coord</param>
        /// <param name="width">The width of the output image</param>
        /// <param name="height">The height of the output image</param>
        /// <returns>The color of the pixel</returns>
        private static dColor getPixelFromBitmap(Bitmap img, int x, int y, int width, int height)
        {
            // Compute the margins around the image
            int leftMargin = (width - img.Width) / 2;
            int topMargin = (height - img.Height) / 2;
            // Accound for this offset
            int imgX = x - leftMargin;
            int imgY = y - topMargin;
            // If the specified point is in the image...
            if (0 <= imgX && imgX < img.Width && 0 <= imgY && imgY < img.Height)
            {
                // Return the correct pixel
                return img.GetPixel(imgX, imgY);
            }
            // Otherwise return a fully transparent pixel
            return dColor.FromArgb(0);
        }
        /// <summary>
        /// Generates a matrix to preview the MagicPNG merging of two images
        /// </summary>
        /// <param name="lightColor">The first backgorund color</param>
        /// <param name="darkColor">The second background color</param>
        /// <param name="invert">Specifies if the image should be inverted before the transformation</param>
        /// <returns>The matrix to preview the MagicPNG transformation</returns>
        private static ColorMatrix getPreviewColorMatrix(mColor lightColor, mColor darkColor, bool invert)
        {
            // Matrix that converts an image to gray scale
            ColorMatrix rawGrayScale = new ColorMatrix(
               new float[][]
               {
                  new float[] { 1f / 3, 1f / 3, 1f / 3, 0, 0},
                  new float[] { 1f / 3, 1f / 3, 1f / 3, 0, 0},
                  new float[] { 1f / 3, 1f / 3, 1f / 3, 0, 0},
                  new float[] { 0, 0, 0, 1, 0},
                  new float[] {0, 0, 0, 0, 1}
               });
            // Matrix that inverts an image iff invert == true
            // Starts as the identity matrix
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
                // Is changed to an inverting matrix iff invert == true
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
            // Gets the values of second background color
            int R2 = darkColor.R;
            int G2 = darkColor.G;
            int B2 = darkColor.B;
            // Gets the increase in values
            int RI = lightColor.R - R2;
            int GI = lightColor.G - G2;
            int BI = lightColor.B - B2;
            // Matrix that convers a grayscale image to a image where white is replaced with the first background color and
            // black is replaced with the second background color and gray fall in between
            ColorMatrix lumToColor = new ColorMatrix(
               new float[][]
               {
                  new float[] { RI / 255f, 0, 0, 0, 0},
                  new float[] { 0, GI/255f, 0, 0, 0},
                  new float[] { 0, 0, BI / 255f, 0, 0},
                  new float[] { 0, 0, 0, 1, 0},
                  new float[] { R2 / 255f, G2 / 255f, B2 / 255f, 0, 1 }
               });
            // Returns the product of the matrices
            return MatrixMultiply(MatrixMultiply(lumToColor, invertMatrix), rawGrayScale);
        }
        /// <summary>
        /// Computes a weighted average of two bytes.  If both weights are 0, the arithmetic average is returned.
        /// </summary>
        /// <param name="val1">The first byte to average</param>
        /// <param name="weight1">The non-negative weight of the first byte</param>
        /// <param name="val2">The second byte to average</param>
        /// <param name="weight2">The non-negative weight of the second byte</param>
        /// <returns>The weighted average of val1 and val2, or the arithmetic mean if both weights are 0</returns>
        private static byte weightedAverage(byte val1, byte weight1, byte val2, byte weight2)
        {
            // If both weights are 0...
            if (weight1 + weight2 == 0)
            {
                // Return the arithmetic average
                return (byte)((val1 + val2) / 2);
            }
            // Otherwise return the weighted average
            return (byte)((val1 * weight1 + val2 * weight2) / (weight1 + weight2));
        }
        /// <summary>
        /// Retruns the weighted average of two colors.  If both weights are 0, a neutral gray is returned.  The result has no alpha
        /// </summary>
        /// <param name="val1"></param>
        /// <param name="weight1"></param>
        /// <param name="val2"></param>
        /// <param name="weight2"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Creates the merged MagicPNG from the two source images with the specified settings
        /// </summary>
        /// <param name="file1">The path to the first file</param>
        /// <param name="file2">The path to the second file</param>
        /// <param name="output">The path the result should be saved to</param>
        /// <param name="lightColor">The firstbackground color</param>
        /// <param name="darkColor">The second background color</param>
        /// <param name="invert1">Whether the first image's lumination should be inverted</param>
        /// <param name="invertBg1">Whether the background of the first image should be inverted</param>
        /// <param name="invert2">Whether the second image's lumination should be inverted</param>
        /// <param name="invertBg2">Whether the background of the second image should be inverted</param>
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
        /// <summary>
        /// Computes the lumination of a pixel
        /// </summary>
        /// <param name="pixel">The pixel</param>
        /// <param name="invert">Whether the pixel's lumination should be inverted</param>
        /// <param name="invertBg">Whether the background of the pixel should be inverted</param>
        /// <returns>The lumination of the pixel</returns>
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
        /// <summary>
        /// Performs matrix multiplication
        /// </summary>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <returns>The product of the two matrices</returns>
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
