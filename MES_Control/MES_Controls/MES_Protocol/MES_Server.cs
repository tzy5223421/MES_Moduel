using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MES_Controls.MES_Protocol
{
    public class MES_Server
    {
        #region 常量
        private string ServerIP;
        private int ServerPort;
        private Socket ServerSocket;
        private Thread ServerThread;
        public event EventHandler<ProtocolPackEventArgs> ProtocolPackEvent;
        public event EventHandler<ProtocolClientEventArgs> ProtocolClientEvent;
        public List<Client_Hash> client_Hashes = new List<Client_Hash>();
        #endregion
        public MES_Server()
        {
        }
        public MES_Server(string ip, int port)
        {
            this.ServerIP = ip;
            this.ServerPort = port;
            ServerThread = new Thread(ServerListen);
            ServerThread.IsBackground = true;
            ServerThread.Start();
        }
        private void ServerListen()
        {
            try
            {
                while (true)
                {
                    ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    ServerSocket.Bind(new IPEndPoint(IPAddress.Parse(ServerIP), ServerPort));
                    ServerSocket.Listen(10);
                    while (true)
                    {
                        Socket clientSocket = ServerSocket.Accept();
                        IPEndPoint remoteEndPoint = (IPEndPoint)clientSocket.RemoteEndPoint;
                        Client_Hash client = new Client_Hash();
                        client.Client_IP = remoteEndPoint.Address.ToString();
                        client.Client_Port = remoteEndPoint.Port;
                        client.ClientSocket = clientSocket;
                        client_Hashes.Add(client);
                        ProtocolClientEvent(this, new ProtocolClientEventArgs(client_Hashes));
                        ThreadPool.QueueUserWorkItem(new WaitCallback(MES_ServerAccpetData), client);
                    }
                }
            }
            catch (Exception ex) { }
        }

        private void MES_ServerAccpetData(object state)
        {
            // throw new NotImplementedException();
            Client_Hash client = (Client_Hash)state;
            try
            {
                while (true)
                {
                    byte[] recBuffer = new byte[1024 * 1024 * 1000];
                    int length = client.ClientSocket.Receive(recBuffer, SocketFlags.None);
                    if (length == 0)
                    {
                        client_Hashes.Remove(client);
                        client.ClientSocket.Close();
                        ProtocolClientEvent(this, new ProtocolClientEventArgs(client_Hashes));
                        break;
                    }
                    ParseAccpetData(recBuffer, length);
                }
            }
            catch (Exception)
            {

                // throw;
            }
        }

        private void ParseAccpetData(byte[] recBuffer, int length)
        {
            // throw new NotImplementedException();
            ProtocolPack protocolPack = new ProtocolPack();
            protocolPack.Bytes = new byte[length];
            Array.Copy(recBuffer, protocolPack.Bytes, length);
            ProtocolPackEvent(this, new ProtocolPackEventArgs(protocolPack));
        }
    }
}
