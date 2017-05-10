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
    class ChooseButton
    {
        Button button;
        OpenFileDialog openDialog;
        string imagePath = "";
        public delegate void UpdateFunction();
        UpdateFunction updateFunction = null;

        public ChooseButton(Button button)
        {
            this.button = button;
            button.Click += Button_Click;
            makeOpenDialog();
        }
        public ChooseButton(Button button, UpdateFunction updateFunction) : this(button)
        {
            this.updateFunction = updateFunction;
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (openDialog.ShowDialog() == true)
            {
                imagePath = openDialog.FileName;
                updateFunction?.Invoke();
            }
        }
        private void makeOpenDialog()
        {
            openDialog = new OpenFileDialog();
            openDialog.CheckFileExists = true;
            openDialog.Filter = "";
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            string sep = "";
            string imageExt = "";
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
        public string getFile()
        {
            return imagePath;
        }
    }
}
