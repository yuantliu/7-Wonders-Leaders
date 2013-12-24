using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SevenWonders
{
    /// <summary>
    /// Interaction logic for Help.xaml
    /// </summary>
    public partial class Help : Window
    {
        //Current directory
        String currentPath = Environment.CurrentDirectory;
        int index = 1;
       


        public Help()
        {
            InitializeComponent();

            slideShowImage.Source = new BitmapImage(new Uri(currentPath + "\\Images\\HelpSlides\\" + index + ".png"));
            ShowDialog();
        }

 
        private void Previous_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (index != 1)
            {
            slideShowImage.Source = new BitmapImage(new Uri(currentPath + "\\Images\\HelpSlides\\" + (--index) + ".png"));
            }
        }

        private void Next_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (index != 8)
            {
                slideShowImage.Source = new BitmapImage(new Uri(currentPath + "\\Images\\HelpSlides\\" + (++index) + ".png"));
            }
        }
    }
}
