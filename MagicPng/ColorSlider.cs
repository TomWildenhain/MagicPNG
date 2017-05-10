using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows.Media;

namespace MagicPng
{
    class ColorSlider : RectControl
    {
        private Color colorA;
        private Color colorB;
        private double ratioAB;

        public delegate void UpdateFunction();
        UpdateFunction updateFunction = null;

        public ColorSlider(Rectangle colorRect, Color colorA, Color colorB, double ratio) : base(colorRect, averageColors(colorA,colorB,ratio))
        {
            this.colorA = colorA;
            this.colorB = colorB;
            ratioAB = ratio;
        }
        public ColorSlider(Rectangle colorRect, Color colorA, Color colorB) : this(colorRect, colorA, colorB, 0.5) { }
        public ColorSlider(Rectangle colorRect, Color colorA, Color colorB, UpdateFunction updateFunction) : this(colorRect, colorA, colorB)
        {
            this.updateFunction = updateFunction;
        }
        public ColorSlider(Rectangle colorRect, Color colorA, Color colorB, double ratio, UpdateFunction updateFunction) : this(colorRect, colorA, colorB, ratio)
        {
            this.updateFunction = updateFunction;
        }

        private static Color averageColors(Color colorA, Color colorB, double ratio)
        {
            byte A = (byte)Math.Round(colorA.A * ratio + colorB.A * (1 - ratio));
            byte R = (byte)Math.Round(colorA.R * ratio + colorB.R * (1 - ratio));
            byte G = (byte)Math.Round(colorA.G * ratio + colorB.G * (1 - ratio));
            byte B = (byte)Math.Round(colorA.B * ratio + colorB.B * (1 - ratio));
            return Color.FromArgb(A, R, G, B);
        }
        public Color getColor()
        {
            return averageColors(colorA, colorB, ratioAB);
        }
    }
}
