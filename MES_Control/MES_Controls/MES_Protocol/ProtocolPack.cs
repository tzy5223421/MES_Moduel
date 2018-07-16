using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_Controls.MES_Protocol
{
    public class ProtocolPack
    {
        public byte[] bytes;
        public Client_Hash client_Hash;
        public byte[] Bytes
        {
            get { return bytes; }
            set { this.bytes = value; }
        }
        public Client_Hash Client_Hashs
        {
            get { return client_Hash; }
            set { this.client_Hash = value; }
        }
    }
}
