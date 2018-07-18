using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace MES_Controls.MES_Protocol
{
    public class MES_TCPClient
    {
        private string ClientIP;
        private int ClientPort;
        private Socket ClientSocket;
        private Thread clientThread;
        private Thread KeepAliveThread;
        private int currentCount = 0;
        private bool isRead = true;
        public MES_TCPClient(string ip, int Port)
        {
            this.ClientIP = ip;
            this.ClientPort = Port;
        }
        public MES_TCPClient()
        {
        }
        public bool ClientConnectToServer()
        {
            try
            {
                if (ClientSocket != null && ClientSocket.Connected)
                {
                    ClientSocket.Close();
                    ClientSocket = null;
                    clientThread.Abort();
                    KeepAliveThread.Abort();
                }
                ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                ClientSocket.Connect(new IPEndPoint(IPAddress.Parse(ClientIP), ClientPort));
                ClientSocket.ReceiveBufferSize = 1024 * 1024 * 1000;
                clientThread = new Thread(clientAccpetData);
                clientThread.IsBackground = true;
                clientThread.Start();
                KeepAliveThread = new Thread(Keeplive);
                KeepAliveThread.IsBackground = true;
                KeepAliveThread.Start();
                return true;
            }
            catch (Exception ex)
            {

                ///throw;
                return false;
            }

        }
        private void Keeplive()
        {
            // throw new NotImplementedException();
            while (true)
            {
                try
                {
                    currentCount++;
                    if (currentCount >= 10)
                    {
                        isRead = false;
                        byte[] bytes = new byte[1024];
                        byte[] data = Encoding.Default.GetBytes("isKeepAlive");
                        ClientSocket.Send(data);
                        ClientSocket.ReceiveTimeout = 10000;
                        int length = ClientSocket.Receive(bytes, SocketFlags.None);
                        if (length != 0)
                        {
                            currentCount = 0;
                            isRead = true;
                            bytes = null;
                        }
                    }
                    Thread.Sleep(1000);
                }
                catch (Exception ex)
                {
                    if (ClientSocket != null)
                    {
                        currentCount = 0;
                        ClientSocket.Close();
                        ClientSocket = null;
                        MessageBox.Show("连接超时!客户端已经关闭！");
                    }
                }
            }
        }
        byte[] recBuffer = new byte[1024 * 1024];
        private void clientAccpetData()
        {
            // throw new NotImplementedException();
            while (true)
            {
                try
                {
                    if (!isRead)
                    {
                        continue;
                    }
                    if (ClientSocket.Poll(10000, SelectMode.SelectRead))
                    {

                        int length = ClientSocket.Receive(recBuffer, SocketFlags.None);
                        string result = Encoding.Default.GetString(recBuffer, 0, length);
                        if (result == "isKeepAlive")
                        {
                            ClientSocket.Send(recBuffer, 0, length, SocketFlags.None);
                            Console.WriteLine(DateTime.Now.ToString() + result);
                            currentCount = 0;
                            continue;
                        }
                        currentCount = 0;
                        if (length != 0)
                        {
                            Console.WriteLine(result);
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (ClientSocket != null)
                    {
                        currentCount = 0;
                        ClientSocket.Close();
                        ClientSocket = null;
                        MessageBox.Show("连接超时!客户端已经关闭！");
                    }
                }
            }
        }
    }
}
