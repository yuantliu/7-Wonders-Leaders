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
    /// Interaction logic for BilkisUI.xaml
    /// </summary>
    public partial class BilkisUI : Window
    {
        Coordinator c;

        public BilkisUI(Coordinator c)
        {
            this.c = c;
            InitializeComponent();
        }

        //(0 is nothing, 1 is ore, 2 is stone, 3 is glass, 4 is papyrus, 5 is loom, 6 is wood, 7 is brick

        private void brickButton_Click(object sender, RoutedEventArgs e)
        {
            c.sendToHost("k" + 7);
            Close();
        }

        private void woodButton_Click(object sender, RoutedEventArgs e)
        {
            c.sendToHost("k" + 6);
            Close();
        }

        private void loomButton_Click(object sender, RoutedEventArgs e)
        {
            c.sendToHost("k" + 5);
            Close();
        }

        private void papyrusButton_Click(object sender, RoutedEventArgs e)
        {
            c.sendToHost("k" + 4);
            Close();
        }

        private void glassButton_Click(object sender, RoutedEventArgs e)
        {
            c.sendToHost("k" + 3);
            Close();
        }

        private void stoneButton_Click(object sender, RoutedEventArgs e)
        {
            c.sendToHost("k" + 2);
            Close();
        }

        private void oreButton_Click(object sender, RoutedEventArgs e)
        {
            c.sendToHost("k" + 1);
            Close();
        }
    }
}
