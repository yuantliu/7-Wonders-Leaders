using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Windows.Controls;

namespace SevenWonders
{

    public class Server
    {
        // This hash table stores users and connections (browsable by user)
        public Hashtable htUsers = new Hashtable(7);

        // This hash table stores connections and users (browsable by connection)
        public Hashtable htConnections = new Hashtable(7);

        // Will store the IP address passed to it
        private IPAddress ipAddress;
        
        private TcpClient tcpClient;

        private int numberOFAI;

        // The thread that will hold the connection listener
        private Thread thrListener;

        // The TCP object that listens for connections
        private TcpListener tcpListener;

        private StreamWriter swSender { get; set; }
        private StreamReader swReader { get; set; }

        // The event and its argument will notify the form when a user has connected, disconnected, send message, etc.
       
        public event StatusChangedEventHandler StatusChanged;
        public StatusChangedEventArgs e;
       
        // Will tell the while loop to keep monitoring for connections
        bool serverRunning = false;

        public bool acceptClient { get; set; }

        // The constructor sets the IP
        public Server()
        {
            acceptClient = true;
            numberOFAI = 0;
            ipAddress = localIP();
        }

        public void updateAIPlayer(bool shouldAdd) 
        {
            if (shouldAdd) numberOFAI++;
            else numberOFAI--;
        }

        /// <summary>
        /// Start the server. This is called by the GMCoordinator
        /// </summary>
        public void StartServer()
        {
            // Create the TCP listener object using the IP of the server and the specified port
            //tcpListener = new TcpListener(ipAddress, 1986);

            // The while loop will check for true in this before checking for connections
            serverRunning = true;

            // Start the new tread that hosts the listener
            thrListener = new Thread(keepListeningForNewRequests);
            thrListener.Start();
        }

        /// <summary>
        /// Have the server keep listening for request. Create a new Connection everytime it receives a TcpClient
        /// </summary>
        private void keepListeningForNewRequests()
        {
            // Create the TCP listener object using the IP of the server and the specified port
            tcpListener = new TcpListener(ipAddress, 1986);

            // Start the TCP listener and listen for connections
            tcpListener.Start();

            // While the server is running
            while (serverRunning == true)
            {
                // Accept a pending connection
                tcpClient = tcpListener.AcceptTcpClient();

                //make the connection
                Connection newConnection = new Connection(tcpClient, numberOFAI, this);
            }

            tcpListener.Stop();
        }

        /// <summary>
        /// Stop the server from listening
        /// </summary>
        public void stopListening()
        {
            serverRunning = false;
            tcpListener.Stop();
            thrListener.Abort();
        }

//////////////////////////////////////////////////////////////////////////////////////////////////////
    //Utility functions

        // Add the user to the hash tables
        public  void AddUser(TcpClient tcpUser, string strUsername)
        {
            // add the username and associated connection to both hash tables
            htUsers.Add(strUsername, tcpUser);
            htConnections.Add(tcpUser, strUsername);
        }

        // Remove the user from the hash tables
        public void RemoveUser(TcpClient tcpUser)
        {
            // If the user is there
            if (htConnections[tcpUser] != null)
            {
                // Remove the user from the hash table
                htUsers.Remove(htConnections[tcpUser]);
                htConnections.Remove(tcpUser);
            }
        }

        /// <summary>
        /// Return the local IP Address
        /// </summary>
        /// <returns></returns>
        private IPAddress localIP()
        {
            IPAddress localIP = null;
            IPHostEntry host;

            host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (IPAddress ip in host.AddressList)
                if (ip.AddressFamily == AddressFamily.InterNetwork) localIP = ip;

            return localIP;
        }

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Receive a message from a User. Take the message and give it to GMCoordinator
        /// </summary>
        /// <param name="e"></param>
        public void OnStatusChanged(StatusChangedEventArgs e)
        {
            StatusChangedEventHandler statusHandler = StatusChanged;

            if (statusHandler != null)
            {
                // Invoke the delegate
                statusHandler(null, e);
            }
        }

        /// <summary>
        /// Receive the message s from the user
        /// Pass this information to the GMCoordinator by making a new Event
        /// </summary>
        /// <param name="user"></param>
        /// <param name="s"></param>
        public void receiveMessageFromConnection(String user, String s)
        {
            e = new StatusChangedEventArgs(user, s);
            OnStatusChanged(e);
        }

        /// <summary>
        /// Send a message to a User's client object.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="Message"></param>
        public  void sendMessageToUser(String userName, String Message)
        {
            StreamWriter sw;
            TcpClient a;

            foreach (DictionaryEntry de in htUsers)
            {
                if ((string)de.Key == userName)
                {
                    a = (TcpClient)de.Value;
                    sw = new StreamWriter(a.GetStream()); //getTheClient's stream to send a message to that client
                    sw.WriteLine(Message); //write the message to the client
                    sw.Flush();
                    sw = null;
                    
                    return;
                }
            }
        }

        public void sendMessageToAll(String Message)
        {
            StreamWriter sw;
            TcpClient a;

            foreach (DictionaryEntry de in htUsers)
            {
                a = (TcpClient)de.Value;
                sw = new StreamWriter(a.GetStream()); //getTheClient's stream to send a message to that client
                sw.WriteLine(Message); //write the message to the client
                sw.Flush();
                sw = null;
            }
        }



        public void handleNewNickName(String oldNickName, String newNickName)
        {
            TcpClient temp = null;
            String tempName = "";

            foreach (DictionaryEntry de in htUsers)
            {
            
                temp = (TcpClient)de.Value;
                tempName = (String)de.Key;

                if (oldNickName == tempName) break;
            }

            RemoveUser(temp);
            AddUser(temp, newNickName);
        }
    }

}
