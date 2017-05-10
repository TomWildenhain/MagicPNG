using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using ColorDialog = System.Windows.Forms.ColorDialog;

namespace MagicPng
{
    class ColorChooser : RectControl
    {
        const int highlightThickness = 3;

        Color selectedColor;
        ColorDialog colorDialog;
        public delegate void UpdateFunction();
        UpdateFunction updateFunction = null;

        public ColorChooser(Rectangle colorRect, Color initialColor) : base(colorRect, initialColor)
        {
            colorDialog = new ColorDialog();
            colorDialog.FullOpen = true;
            colorDialog.Color = colorToDrawingColor(initialColor);
            selectedColor = initialColor;
        }
        public ColorChooser(Rectangle colorRect, Color initialColor, UpdateFunction updateFunction) : this(colorRect, initialColor)
        {
            this.updateFunction = updateFunction;
        }
        private static System.Drawing.Color colorToDrawingColor(Color color)
        {
            return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        }
        private static Color drawingColorToColor(System.Drawing.Color color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }
        protected override void setColor(Color newColor)
        {
            selectedColor = newColor;
            base.setColor(newColor);
        }
        protected override void MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                setColor(drawingColorToColor(colorDialog.Color));
                updateFunction?.Invoke();
            }
            else
            {
                colorDialog.Color = colorToDrawingColor(selectedColor);
            }
        }
        public Color getColor()
        {
            return selectedColor;
        }
        public void changeColor(Color color)
        {
            setColor(color);
        }
    }
}
