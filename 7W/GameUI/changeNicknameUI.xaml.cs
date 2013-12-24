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
    /// Interaction logic for changeNicknameUI.xaml
    /// </summary>
    public partial class changeNicknameUI : Window
    {
        Coordinator coordinator;

        public changeNicknameUI(Coordinator c)
        {
            coordinator = c;
            InitializeComponent();
            ShowDialog();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (nickText.Text.Length > 0)
            {
                coordinator.sendToHost("n" + nickText.Text);
                Close();
            }
        }
    }
}
