using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SevenWonders
{
    // Holds the arguments for the StatusChanged event
    public class StatusChangedEventArgs : EventArgs
    {
        // The argument we're interested in is a message describing the event
        public string nickname { get; set; }
        public string message { get; set; }


        // Constructor for setting the event message
        public StatusChangedEventArgs(string nickname, string message)
        {
            this.nickname = nickname;
            this.message = message;
        }
    }

    // This delegate is needed to specify the parameters we're passing with our event
    public delegate void StatusChangedEventHandler(object sender, StatusChangedEventArgs e);
}
