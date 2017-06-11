using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;

namespace MagicPng
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ColorChooser colorAChooser;
        ColorChooser colorCChooser;
        ChooseButton imageChooser1;
        ChooseButton imageChooser2;
        Random random = new Random();
        bool invert1 = false;
        bool invertBg1 = false;
        bool invert2 = false;
        bool invertBg2 = false;
        public MainWindow()
        {
            InitializeComponent();
            colorAChooser = new ColorChooser(ColorARect, Colors.White, update);
            colorCChooser = new ColorChooser(ColorCRect, Colors.Black, update);
            imageChooser1 = new ChooseButton(ChooseBn1, update);
            imageChooser2 = new ChooseButton(ChooseBn2, update);
            SaveBn.Click += SaveBn_Click;
            InvertBgBn1.Click += InvertBgBn1_Click;
            InvertBgBn2.Click += InvertBgBn2_Click;
            InvertImgBn1.Click += InvertImgBn1_Click;
            InvertImgBn2.Click += InvertImgBn2_Click;
            SwapBn.Click += SwapBn_Click;
        }

        private void SwapBn_Click(object sender, RoutedEventArgs e)
        {
            Color colorA = colorAChooser.getColor();
            Color colorC = colorCChooser.getColor();
            colorAChooser.changeColor(colorC);
            colorCChooser.changeColor(colorA);
            update();
        }

        private void SaveBn_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveDilog = new SaveFileDialog();
            saveDilog.DefaultExt = ".png";
            saveDilog.AddExtension = true;
            saveDilog.OverwritePrompt = true;
            if (saveDilog.ShowDialog() == true)
            {
                mergeImages(saveDilog.FileName);
            }
        }

        private void InvertImgBn2_Click(object sender, RoutedEventArgs e)
        {
            invert2 = !invert2;
            previewImg2();
        }

        private void InvertImgBn1_Click(object sender, RoutedEventArgs e)
        {
            invert1 = !invert1;
            previewImg1();
        }

        private void InvertBgBn2_Click(object sender, RoutedEventArgs e)
        {
            invertBg2 = !invertBg2;
            previewImg2();
        }

        private void InvertBgBn1_Click(object sender, RoutedEventArgs e)
        {
            invertBg1 = !invertBg1;
            previewImg1();
        }

        private void update()
        {
            previewImg1();
            previewImg2();
        }
        private void previewImg1()
        {
            string output = getTempFile();
            string img1 = imageChooser1.getFile();
            if (img1 != "")
            {
                string img2 = imageChooser2.getFile();
                Color lightColor = colorAChooser.getColor();
                Color darkColor = colorCChooser.getColor();
                darkColor = ImageTransformer.weightedAverage(lightColor, 127, darkColor, 127);
                ImageTransformer.previewTransform(img1, img2, output, darkColor, lightColor, invert1, invertBg1);
                Image1.Source = new BitmapImage(new Uri(output));
            }
        }
        private void previewImg2()
        {
            string output = getTempFile();
            string img2 = imageChooser2.getFile();
            if (img2 != "")
            {
                string img1 = imageChooser1.getFile();
                Color lightColor = colorAChooser.getColor();
                Color darkColor = colorCChooser.getColor();
                lightColor = ImageTransformer.weightedAverage(lightColor, 127, darkColor, 127);
                ImageTransformer.previewTransform(img2, img1, output, lightColor, darkColor, invert2, invertBg2);
                Image2.Source = new BitmapImage(new Uri(output));
            }
        }
        private void mergeImages(string output)
        {
            string img1 = imageChooser1.getFile();
            string img2 = imageChooser2.getFile();
            if (img1 != "" && img2 != "")
            {
                Color lightColor = colorAChooser.getColor();
                Color darkColor = colorCChooser.getColor();
                ImageTransformer.mergeImages(img1, img2, output, lightColor, darkColor, invert1, invertBg1, invert2, invertBg2);
            }
        }
        private string getTempFile()
        {
            string temp = System.IO.Path.GetTempPath();
            if (!Directory.Exists(temp + "MagicPNG"))
            {
                Directory.CreateDirectory(temp + "MagicPNG");
            }
            return temp + "MagicPNG\\" + random.Next() + ".png";
        }
    }
}
