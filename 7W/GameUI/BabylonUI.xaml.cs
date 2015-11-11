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
    /// Interaction logic for BabylonUI.xaml
    /// </summary>
    public partial class BabylonUI : Window
    {
		Coordinator coordinator;
		int id;
        bool closeButton = true;
        char build, stage;
	
        public BabylonUI(Coordinator c, String information)
        {
			coordinator = c;

            InitializeComponent();

            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.Fant);

			//information of the form
			//A_(id)_TT
			//get the ID
            int idlength = 0;
			for(int i = 2; information[i] != '_'; i++){
				idlength++;
			}
			
			//currently at the second _
			id = int.Parse(information.Substring(2, idlength));

            //set the image
            String currentPath = Environment.CurrentDirectory;

            BitmapImage cardImageSource = new BitmapImage();
            cardImageSource.BeginInit();
            cardImageSource.UriSource = new Uri(currentPath + @"\Resources\cards\" + id + ".jpg");
            cardImageSource.EndInit();
            image1.Source = cardImageSource;
			
			//move 1 array element past the _, grab the build information
            build = information.Substring(2 + idlength + 1)[0];
            stage = information.Substring(2 + idlength + 1)[1];

            //do the build structure buttons
            if (build == 'T')
            {
                buildStructureButton.Content = "Build Structure";
            }
            else if (build == 'C')
            {
                buildStructureButton.Content = "Commerce";
            }
            else
            {
                buildStructureButton.Content = "Build Structure";
                buildStructureButton.IsEnabled = false;
            }

            //do the build stage structure buttons
            if (stage == 'T')
            {
                buildStageButton.Content = "Build Stage";
            }
            else if (stage == 'C')
            {
                buildStageButton.Content = "Commerce";
            }
            else
            {
                buildStageButton.Content = "Build Stage";
                buildStageButton.IsEnabled = false;
            }

            //do the discard button
            discardButton.Content = "Discard Card";
        }

        private void buildStructureButton_Click(object sender, RoutedEventArgs e)
        {
            //card is buildable
            if (build == 'T')
            {
                //send the instruction for building the card
                //B(id)
                coordinator.sendToHost("B" + id);
                closeButton = false;
                //end the turn
                coordinator.endTurn();
                Close();
            }
            else if (build == 'C')
            {
                //use commerce
                //closeButton = false;
                coordinator.sendToHost("Cb" + id);
                Close();
            }
        }

        private void buildStageButton_Click(object sender, RoutedEventArgs e)
        {
            //send the instruction for building the stage
            //S(id)
            //card is buildable
            if (build == 'T')
            {
                coordinator.sendToHost("S" + id);
                closeButton = false;
                //end the turn
                coordinator.endTurn();
                Close();
            }
            else if (build == 'C')
            {
                //use commerce
                //closeButton = false;
                coordinator.sendToHost("Cs" + id);
                Close();
            }
            
        }

        private void discardButton_Click(object sender, RoutedEventArgs e)
        {
            //send the instruction for discarding the card
            //D(id)
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (closeButton)
            {
                //default action: discard card
                coordinator.sendToHost("D" + id);
                //end the turn
                coordinator.endTurn();
            }
        }

        

    }
}