using MES_Controls.MES_Protocol;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Test
{
    public partial class Form1 : Form
    {
        MES_Controls.MES_Protocol.MES_TCPServer server;
        MES_TCPClient client;
        public Form1()
        {
            InitializeComponent();
            server = new MES_Controls.MES_Protocol.MES_TCPServer("127.0.0.1", 5000, true);
            server.ProtocolPackEvent += Server_ProtocolPackEvent;
            server.ProtocolClientEvent += Server_ProtocolClientEvent;
        }

        private void Server_ProtocolClientEvent(object sender, MES_Controls.MES_Protocol.ProtocolClientEventArgs e)
        {
            // throw new NotImplementedException();
            this.listBox1.Invoke(new Action(() =>
            {
                this.listBox1.Items.Clear();
                for (int i = 0; i < e.client_Hash.Count; i++)
                {
                    listBox1.Items.Add(e.client_Hash[i].Client_IP + ":" + e.client_Hash[i].Client_Port);
                }
            }));
        }

        private void Server_ProtocolPackEvent(object sender, MES_Controls.MES_Protocol.ProtocolPackEventArgs e)
        {
            // throw new NotImplementedException();
            MES_Controls.MES_Protocol.ProtocolPack protocolPack = e.protocolPack;
            string text = Encoding.Default.GetString(protocolPack.Bytes);
            // this.richTextBox1.Invoke(new Action(() => { this.richTextBox1.AppendText(text + '\r'); }));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            client = new MES_TCPClient("127.0.0.1", 5000);
            client.ClientConnectToServer();
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ChatFrm cf = new ChatFrm(server, this.listBox1.Text.ToString());
            cf.Show();
        }
    }
}
