using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;

namespace MES_Controls.MES_Protocol
{
    public class Client_Hash
    {
        private string client_IP;
        private int client_Port;
        private string client_name;
        private Socket socket;
        private CancellationTokenSource tokenSource;
        private int currentCount;
        private bool isRead = true;

        public string Client_IP
        {
            get { return client_IP; }
            set { this.client_IP = value; }
        }
        public int Client_Port
        {
            get { return client_Port; }
            set { this.client_Port = value; }
        }
        public string Client_Name
        {
            get { return client_name; }
            set { this.client_name = value; }
        }
        public Socket ClientSocket
        {
            get { return socket; }
            set { this.socket = value; }
        }
        public CancellationTokenSource TokenSource
        {
            get { return tokenSource; }
            set { this.tokenSource = value; }
        }
        public int CurrentCount
        {
            get
            {
                return currentCount;
            }
            set { currentCount = value; }
        }
        public bool IsRead
        {
            get { return isRead; }
            set { isRead = value; }
        }
    }
}
