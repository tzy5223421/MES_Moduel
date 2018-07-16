using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_Controls.MES_Protocol
{
    public class ProtocolCommad
    {
        public int PROTOCOL_FILETRAN = 0x01;

        public int PROTOCOL_PLC_DATA = 0x02;

        public int PROTOCOL_SEVER_DATA = 0x03;

        public int PROTOCOL_ERROR_DATA = 0x04;
    }
}
