using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_Controls.MES_Protocol
{
    public class ProtocolPackEventArgs : EventArgs
    {
        public ProtocolPack protocolPack;
        public ProtocolPackEventArgs(ProtocolPack protocolPack)
        {
            this.protocolPack = protocolPack;
        }
    }
}
