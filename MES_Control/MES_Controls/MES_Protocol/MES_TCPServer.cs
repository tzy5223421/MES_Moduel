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
    public class MES_TCPServer
    {
        #region 常量
        private string ServerIP;
        private int ServerPort;
        private Socket ServerSocket;
        private Thread ServerThread;
        public event EventHandler<ProtocolPackEventArgs> ProtocolPackEvent;
        public event EventHandler<ProtocolClientEventArgs> ProtocolClientEvent;
        private List<Client_Hash> client_Hashes = new List<Client_Hash>();
        private bool isKeepAlive;

        #endregion
        public MES_TCPServer()
        {
        }
        public MES_TCPServer(string ip, int port)
        {
            this.ServerIP = ip;
            this.ServerPort = port;
            ServerThread = new Thread(ServerListen);
            ServerThread.IsBackground = true;
            ServerThread.Start();
        }
        public MES_TCPServer(string ip, int port, bool isKeepAlive)
        {
            this.ServerIP = ip;
            this.ServerPort = port;
            ServerThread = new Thread(ServerListen);
            ServerThread.IsBackground = true;
            ServerThread.Start();
            this.isKeepAlive = isKeepAlive;
        }

        private void KeepAlive(object state)
        {
            // throw new NotImplementedException();
            Client_Hash client = (Client_Hash)state;
            client.CurrentCount = 0;
            try
            {
                while (true)
                {
                    client.CurrentCount++;
                    if (client.CurrentCount >= 10)
                    {
                        client.IsRead = false;
                        byte[] send = Encoding.Default.GetBytes("isKeepAlive");
                        client.ClientSocket.Send(send);
                        client.ClientSocket.ReceiveTimeout = 10000;
                        byte[] bytes = new byte[1024];
                        int length = client.ClientSocket.Receive(bytes, SocketFlags.None);
                        if (length != 0)
                        {
                            client.CurrentCount = 0;
                            bytes = null;
                            client.IsRead = true;
                        }
                        bytes = null;
                    }
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                if (client.ClientSocket != null)
                {
                    client.ClientSocket.Close();
                    client.ClientSocket = null;
                    client.TokenSource.Cancel();
                    client_Hashes.Remove(client);
                    client.CurrentCount = 0;
                    client.IsRead = false;
                    ProtocolClientEvent(this, new ProtocolClientEventArgs(client_Hashes));
                }
                //       throw;
            }
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
                        client.CurrentCount = 0;
                        client.IsRead = true;
                        client.TokenSource = new CancellationTokenSource();
                        client_Hashes.Add(client);
                        ProtocolClientEvent(this, new ProtocolClientEventArgs(client_Hashes));
                        ThreadPool.QueueUserWorkItem((o) =>
                        {
                            MES_ServerAccpetData(client.TokenSource.Token, client, o);
                        });
                        if (isKeepAlive)
                        {
                            ParameterizedThreadStart pts = new ParameterizedThreadStart(KeepAlive);
                            Thread thread = new Thread(pts);
                            thread.IsBackground = true;
                            thread.Start(client);
                        }
                    }
                }
            }
            catch (Exception ex) { }
        }
        byte[] recBuffer = new byte[1024 * 1024];
        private void MES_ServerAccpetData(CancellationToken token, Client_Hash client, object state)
        {

            try
            {
                while (true)
                {
                    if (!client.IsRead)
                    {
                        continue;
                    }
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }
                    if (client.ClientSocket.Poll(1000000, SelectMode.SelectRead) && client.IsRead)
                    {
                        int length = client.ClientSocket.Receive(recBuffer, SocketFlags.None);
                        string result = Encoding.Default.GetString(recBuffer, 0, length);
                        if (result == "isKeepAlive")
                        {
                            client.ClientSocket.Send(recBuffer, 0, length, SocketFlags.None);
                        }
                        client.CurrentCount = 0;
                        if (length != 0)
                        {
                            ParseAccpetData(recBuffer, length, client);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (client.ClientSocket != null)
                {
                    client.ClientSocket.Close();
                    client.ClientSocket = null;
                    client.TokenSource.Cancel();
                    client_Hashes.Remove(client);
                    client.CurrentCount = 0;
                    client.IsRead = false;
                    ProtocolClientEvent(this, new ProtocolClientEventArgs(client_Hashes));
                }
            }
        }

        private void ParseAccpetData(byte[] recBuffer, int length, Client_Hash client_Hash)
        {
            // throw new NotImplementedException();
            ProtocolPack protocolPack = new ProtocolPack();
            protocolPack.Bytes = new byte[length];
            Array.Copy(recBuffer, protocolPack.Bytes, length);
            protocolPack.Client = client_Hash;
            ProtocolPackEvent(this, new ProtocolPackEventArgs(protocolPack));
        }

        /// <summary>
        /// 指定客户端数据发送
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public void SendData(byte[] bytes, string ip, int port)
        {
            try
            {
                foreach (var client in client_Hashes)
                {
                    if (ip == client.Client_IP && port == client.Client_Port)
                    {
                        client.CurrentCount = 0;
                        client.ClientSocket.Send(bytes, SocketFlags.None);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// 广播式发送
        /// </summary>
        /// <param name="bytes"></param>
        public void SendData(byte[] bytes)
        {
            try
            {
                foreach (var client in client_Hashes)

                {
                    client.ClientSocket.Send(bytes, SocketFlags.None);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
