using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MES_Controls.MES_Protocol;

namespace Test
{
    public partial class ChatFrm : Form
    {
        MES_TCPServer mes_Server;
        public ChatFrm()
        {
            InitializeComponent();
        }
        public ChatFrm(MES_TCPServer mes_Server)
        {
            InitializeComponent();
        }
        public ChatFrm(MES_TCPServer mes_Server, string title)
        {
            InitializeComponent();
            this.Text = title;
            this.mes_Server = mes_Server;
            this.mes_Server.ProtocolPackEvent += Mes_Server_ProtocolPackEvent;
        }

        private void Mes_Server_ProtocolPackEvent(object sender, ProtocolPackEventArgs e)
        {
            MES_Controls.MES_Protocol.ProtocolPack protocolPack = e.protocolPack;
            string text = Encoding.Default.GetString(protocolPack.Bytes);
            //throw new NotImplementedException();
            this.richTextBox1.Invoke(new Action(() =>
            {
                this.richTextBox1.SelectionAlignment = HorizontalAlignment.Left;
                this.richTextBox1.AppendText(DateTime.Now + ":" + text + '\r');
            }));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            byte[] bytes = Encoding.Default.GetBytes(this.richTextBox2.Text);
            string[] str = this.Text.Split(':');
            this.richTextBox1.Invoke(new Action(() =>
            {
                this.richTextBox1.SelectionAlignment = HorizontalAlignment.Right;
                this.richTextBox1.AppendText(DateTime.Now + ":" + this.richTextBox2.Text + '\r');
            }));
            mes_Server.SendData(bytes, str[0], Convert.ToInt32(str[1]));
        }

        private void ChatFrm_FormClosed(object sender, FormClosedEventArgs e)
        {
            mes_Server.ProtocolPackEvent -= Mes_Server_ProtocolPackEvent;
        }
    }
}
