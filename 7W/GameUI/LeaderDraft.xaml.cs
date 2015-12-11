using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
using System.Windows.Shapes;

namespace SevenWonders
{
    /// <summary>
    /// Interaction logic for LeaderDraft.xaml
    /// </summary>
    public partial class LeaderDraft : Window
    {
        Coordinator coordinator;

        public LeaderDraft(Coordinator c)
        {
            InitializeComponent();

            coordinator = c;
        }

        public void UpdateUI(NameValueCollection cards)
        {
            hand.Items.Clear();

            foreach (string cardName in cards.Keys)
            {
                BitmapImage bmpImg = new BitmapImage();
                bmpImg.BeginInit();
                //Item1 of the id_buildable array of Tuples represents the id image
                bmpImg.UriSource = new Uri("pack://application:,,,/7W;component/Resources/Images/cards/" + cardName + ".jpg");
                bmpImg.EndInit();

                Image img = new Image();
                img.Source = bmpImg;

                ListBoxItem entry = new ListBoxItem();
                entry.Name = cardName;
                entry.Content = img;

                hand.Items.Add(entry);
            }

            btnDraft.IsEnabled = false;
        }

        private void hand_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnDraft.IsEnabled = true;
        }

        private void btnDraft_Click(object sender, RoutedEventArgs e)
        {
            RecruitedLeaders.Items.Add(hand.SelectedItem);

            coordinator.sendToHost(string.Format("BldStrct&Structure={0}", ((ListBoxItem)hand.SelectedItem).Name));
            coordinator.endTurn();
        }
    }
}
