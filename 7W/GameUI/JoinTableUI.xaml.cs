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
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace SevenWonders
{
    /// <summary>
    /// Join Table UI
    /// </summary>
    public partial class JoinTableUI : Window
    {
        Coordinator coordinator;

        public JoinTableUI(Coordinator c)
        {
            InitializeComponent();
            coordinator = c;
        }

        /*
         * Parse the information in the Table UI
         * UC-02 R03
         */
        public void button1_Click(object sender, RoutedEventArgs e)
        {
            IPAddress ip;

            //attempt to parse the IP address in field
            if (IPAddress.TryParse(ipAddressText.Text, out ip))
            {
                ip = IPAddress.Parse(ipAddressText.Text);
            }
            else
            {
                MessageBox.Show("Invalid IP");
                return;
            }

            //tell the coordinator to join the game
            // coordinator.joinGame(textUser.Text, IPAddress.Parse(ipAddressText.Text));

            //close the Join Button window
            Close();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
