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
    /// Interaction logic for AIStrategy.xaml
    /// </summary>
    public partial class AIStrategy : Window
    {
        int index = 0;
        Coordinator coordinator;

        public AIStrategy(char mode, Coordinator c)
        {
            InitializeComponent();

            //there are two modes (so far)
            //'V' for vanilla
            //'L' for leaders

            //fill the available AI list

            //Add vanilla strats to AIStrategy UI combobox
            if (mode == 'V')
            {
            }
            else if (mode == 'L')
            {
                ComboBoxItem item0 = new ComboBoxItem();
                item0.Content = "Discard random card (Leaders)";
                strategyBox.Items.Add(item0);

                ComboBoxItem item1 = new ComboBoxItem();
                item1.Content = "Build anything that's buildable (Leaders)";
                strategyBox.Items.Add(item1);

                ComboBoxItem item2 = new ComboBoxItem();
                item2.Content = "Prefer Victory Points (Leaders)";
                strategyBox.Items.Add(item2);

                ComboBoxItem item3 = new ComboBoxItem();
                item3.Content = "Prefer Military (Leaders)";
                strategyBox.Items.Add(item3);

                ComboBoxItem item4 = new ComboBoxItem();
                item4.Content = "\"Difficult\" (Leaders)";
                strategyBox.Items.Add(item4);
            }

            coordinator = c;
        }

        private void strategyBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            index = strategyBox.SelectedIndex;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            coordinator.sendToHost("aa" + index);
            Close();
        }

    }
}
