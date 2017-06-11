using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Drawing.Imaging;

namespace MagicPng
{
    /// A button for choosing an image
    class ChooseButton
    {
        Button button;
        OpenFileDialog openDialog;
        string imagePath = "";
        public delegate void UpdateFunction();
        UpdateFunction updateFunction = null;

        /// <summary>
        /// Generates a ChooseButton from a Button element
        /// </summary>
        /// <param name="button">The button that should trigger the image selection dialog</param>
        public ChooseButton(Button button)
        {
            this.button = button;
            button.Click += Button_Click;
            makeOpenDialog();
        }

        /// <summary>
        /// Generates a ChooseButton that calls a function each time an image is selected
        /// </summary>
        /// <param name="button">The button that should trigger the image selection dialog</param>
        /// <param name="updateFunction">The function to call when a new image is selected</param>
        public ChooseButton(Button button, UpdateFunction updateFunction) : this(button)
        {
            this.updateFunction = updateFunction;
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // If an image was selected...
            if (openDialog.ShowDialog() == true)
            {
                // Store the path to the image
                imagePath = openDialog.FileName;
                // And invoke the update function if it exists
                updateFunction?.Invoke();
            }
        }
        private void makeOpenDialog()
        {
            // Make a new OpenFileDialog
            openDialog = new OpenFileDialog();
            openDialog.CheckFileExists = true;
            openDialog.Filter = "";

            //Get a list of image file types
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            string sep = "";
            string imageExt = "";

            // Only allow image files to be selected
            foreach(ImageCodecInfo c in codecs)
            {
                imageExt += sep + c.FilenameExtension;
                sep = ";";
            }
            openDialog.Filter = string.Format("{0} ({1})|{1}", "Image Files", imageExt);
            foreach (ImageCodecInfo c in codecs) {
                sep = "|";
                string codecName = c.CodecName.Substring(8).Replace("Codec", "Files").Trim();
                openDialog.Filter = string.Format("{0}{1}{2} ({3})|{3}", openDialog.Filter, sep, codecName, c.FilenameExtension);
            }
        }

        /// <summary>
        /// Gets the image that was selected
        /// </summary>
        /// <returns>The path to the currently selected image or empty string if none has been selected</returns>
        public string getFile()
        {
            return imagePath;
        }
    }
}
