using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_Controls.MES_Protocol
{
    public class ProtocolClientEventArgs : EventArgs
    {
        public List<Client_Hash> client_Hash;
        public ProtocolClientEventArgs(List<Client_Hash> client_Hash)
        {
            this.client_Hash = client_Hash;
        }
    }
}
