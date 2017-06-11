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
    /// A color selection button
    class ColorChooser : RectControl
    {
        const int highlightThickness = 3;

        Color selectedColor;
        ColorDialog colorDialog;
        public delegate void UpdateFunction();
        UpdateFunction updateFunction = null;

        /// <summary>
        /// Creates a color chooser from a rectangle with the specified initial color
        /// </summary>
        /// <param name="colorRect">The rectangle the user must click to open the chooser</param>
        /// <param name="initialColor">The initial selected color</param>
        public ColorChooser(Rectangle colorRect, Color initialColor) : base(colorRect, initialColor)
        {
            // Create the color selection dialog
            colorDialog = new ColorDialog();
            colorDialog.FullOpen = true;
            colorDialog.Color = colorToDrawingColor(initialColor);

            // Store the current color
            selectedColor = initialColor;
        }

        /// <summary>
        /// Creates a color chooser that calls the specified function when a new color is selected
        /// </summary>
        /// <param name="colorRect">The rectangle the user must click to open the chooser</param>
        /// <param name="initialColor">The initial selected color</param>
        /// <param name="updateFunction">The function to call when a color is selected</param>
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
        /// <summary>
        /// Gets the currently selected color
        /// </summary>
        /// <returns>The currently selected color</returns>
        public Color getColor()
        {
            return selectedColor;
        }
        /// <summary>
        /// Modifies the currently selected color
        /// </summary>
        /// <param name="color">The new selected color</param>
        public void changeColor(Color color)
        {
            setColor(color);
        }
    }
}
