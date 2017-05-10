using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows.Media;

namespace MagicPng
{
    class RectControl
    {
        const int highlightThickness = 3;

        Rectangle colorRect;
        Color currentColor;
        bool highlighted = false;
        double originalWidth;
        double originalHeight;

        public RectControl(Rectangle colorRect, Color initialColor)
        {
            this.colorRect = colorRect;
            originalWidth = colorRect.Width;
            originalHeight = colorRect.Height;
            setColor(initialColor);
            unhighlight();
            addHandlers();
        }
        private void highlight()
        {
            colorRect.StrokeThickness = highlightThickness;
            colorRect.Margin = new System.Windows.Thickness(0);
            colorRect.Width = originalWidth + highlightThickness * 2;
            colorRect.Height = originalHeight + highlightThickness * 2;
            highlighted = true;
        }
        private void unhighlight()
        {
            colorRect.StrokeThickness = 0;
            colorRect.Margin = new System.Windows.Thickness(highlightThickness);
            colorRect.Width = originalWidth;
            colorRect.Height = originalHeight;
            highlighted = false;
        }
        virtual protected void setColor(Color newColor)
        {
            currentColor = newColor;
            colorRect.Fill = new SolidColorBrush(newColor);
            colorRect.Stroke = new SolidColorBrush(newColor);
        }
        private void addHandlers()
        {
            colorRect.MouseEnter += MouseEnter;
            colorRect.MouseLeave += MouseLeave;
            colorRect.MouseUp += MouseUp;
        }
        virtual protected void MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        }
        virtual protected void MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            highlight();
        }
        virtual protected void MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            unhighlight();
        }
    }
}
