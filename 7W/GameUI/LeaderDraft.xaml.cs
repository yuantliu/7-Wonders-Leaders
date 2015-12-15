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

            //make graphics better
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.Fant);

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
                img.Height = hand.Height;

                ListBoxItem entry = new ListBoxItem();
                entry.Name = cardName;
                entry.Content = img;

                hand.Items.Add(entry);
            }

            // btnDraft.IsEnabled = false;
        }

        private void hand_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (hand.SelectedItem != null)
            {
                // btnDraft.IsEnabled = true;
                LeaderDescription.Text = coordinator.FindCard(((ListBoxItem)hand.SelectedItem).Name).description;
            }
            else
            {
                // btnDraft.IsEnabled = false;
                LeaderDescription.Text = null;
            }
        }

        private void btnDraft_Click(object sender, RoutedEventArgs e)
        {
            ListBoxItem entry = hand.SelectedItem as ListBoxItem;

            hand.Items.Remove(entry);
            RecruitedLeaders.Items.Add(entry);

            coordinator.sendToHost(string.Format("BldStrct&Structure={0}", entry.Name));
            coordinator.endTurn();

            if (hand.Items.Count == 0)
            {
                // if this was the 4th leader to be drafted, close the dialog box.
                Close();
            }
        }

        private void RecruitedLeaders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RecruitedLeaders.SelectedItem != null)
            {
                DraftedLeaderDescription.Text = coordinator.FindCard(((ListBoxItem)RecruitedLeaders.SelectedItem).Name).description;
            }
            else
            {
                DraftedLeaderDescription.Text = null;
            }
        }

        private void hand_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem entry = hand.SelectedItem as ListBoxItem;

            hand.Items.Remove(entry);
            RecruitedLeaders.Items.Add(entry);

            coordinator.sendToHost(string.Format("BldStrct&Structure={0}", entry.Name));
            coordinator.endTurn();

            if (hand.Items.Count == 0)
            {
                // if this was the 4th leader to be drafted, close the dialog box.
                Close();
            }
        }
    }
}
